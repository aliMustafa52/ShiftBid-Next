using Meilisearch;

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

var app = builder.Build();

// Configure the HTTP request pipeline.

app.Run();
