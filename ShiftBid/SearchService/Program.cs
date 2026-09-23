using Meilisearch;
using SearchService.Data;
using SearchService.Endpoints;
using SearchService.Handlers;
using SearchService.Services;
using Wolverine;
using Wolverine.ErrorHandling;
using Wolverine.RabbitMQ;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddSingleton<MeilisearchClient>(sp => 
{
    var url = builder.Configuration["Meilisearch:Url"] 
        ?? throw new InvalidOperationException("Meilisearch Url is not configured.");
    var key = builder.Configuration["Meilisearch:Key"] 
        ?? throw new InvalidOperationException("Meilisearch Key is not configured.");
        
    return new MeilisearchClient(url, key);
});

builder.Services
    .AddHttpClient<AuctionSvcHttpClient>()
    .AddStandardResilienceHandler(options =>
    {
        options.TotalRequestTimeout.Timeout = TimeSpan.FromMinutes(3);
        options.Retry.MaxRetryAttempts = 5;
        options.Retry.Delay = TimeSpan.FromSeconds(10);
        options.Retry.OnRetry = args =>
        {
            Console.WriteLine($"Auction Svc unavailable. Retry {args.AttemptNumber + 1} / 5");
            return default;
        };
    });

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
    .BindExchange("auction-created").ToQueue("search-auction-created")
    .BindExchange("auction-updated").ToQueue("search-auction-updated")
    .BindExchange("auction-deleted").ToQueue("search-auction-deleted")
    .AutoProvision();

    opts.ListenToRabbitQueue("search-auction-created");
    opts.ListenToRabbitQueue("search-auction-updated");
    opts.ListenToRabbitQueue("search-auction-deleted");

    opts.OnException<TransientSearchException>()
    .RetryWithCooldown(
        TimeSpan.FromMilliseconds(500),
        TimeSpan.FromMilliseconds(500),
        TimeSpan.FromSeconds(1)
        ).Then.MoveToErrorQueue();
});

var app = builder.Build();

try 
{
    await DbInitializer.ConfigureIndex(app);
}
catch (Exception e)
{
    Console.WriteLine($"Failed to initialize Meilisearch DB: {e.Message}");
}

_ = Task.Run(async () =>
{
    try
    {
        await DbInitializer.FetchMissingAuctions(app);
    }
    catch (Exception e)
    {

        Console.WriteLine($"Failed to seed search: {e.Message}");
    }
});

// Configure the HTTP request pipeline.
app.MapSearchEndpoints();

app.Run();
