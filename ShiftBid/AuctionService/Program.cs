using AuctionService.Data;
using AuctionService.Middleware;
using Contracts;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Wolverine;
using Wolverine.EntityFrameworkCore;
using Wolverine.Postgresql;
using Wolverine.RabbitMQ;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

var connString = builder.Configuration.GetConnectionString("DefaultConnection");
if (string.IsNullOrWhiteSpace(connString))
    throw new Exception("Connection string is empty");

builder.Services.AddDbContextWithWolverineIntegration<AuctionDbContext>(options =>
{
    options.UseNpgsql(connString);
});

// Configure Mapster mappings
TypeAdapterConfig.GlobalSettings.Scan(typeof(Program).Assembly);

builder.Host.UseWolverine(opts =>
{
    opts.UseRabbitMq(rabbit =>
    {
        rabbit.HostName = builder.Configuration["RabbitMQ:Host"] ?? "localhost";
        rabbit.UserName = builder.Configuration["RabbitMQ:Username"] ?? "guest";
        rabbit.Password = builder.Configuration["RabbitMQ:Password"] ?? "guest";
    })
    .DeclareExchange("auction-created", ex => ex.ExchangeType = ExchangeType.Fanout)
    .DeclareExchange("auction-updated", ex => ex.ExchangeType = ExchangeType.Fanout)
    .DeclareExchange("auction-deleted", ex => ex.ExchangeType = ExchangeType.Fanout)
    .AutoProvision();

    opts.PublishMessage<AuctionCreated>()
    .ToRabbitExchange("auction-created");

    opts.PublishMessage<AuctionUpdated>()
    .ToRabbitExchange("auction-updated");

    opts.PublishMessage<AuctionDeleted>()
    .ToRabbitExchange("auction-deleted");

    opts.PersistMessagesWithPostgresql(connString, "auctions_rmq");

    opts.UseEntityFrameworkCoreTransactions();

    opts.Policies.UseDurableOutboxOnAllSendingEndpoints();

    opts.ListenToRabbitQueue("wolverine-dead-letter-queue");
});

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseExceptionHandler();

app.UseAuthorization();

app.MapControllers();

// Initialize and seed database
try
{
    DbInitializer.InitDb(app);
}
catch (Exception ex)
{
    var logger = app.Services.GetRequiredService<ILogger<Program>>();
    logger.LogError(ex, "An error occurred while initializing or seeding the database.");
}

app.Run();
