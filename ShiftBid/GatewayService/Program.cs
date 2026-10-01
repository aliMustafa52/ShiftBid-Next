using Microsoft.AspNetCore.Authentication.JwtBearer;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

var yarpConfig = builder.Configuration
    .GetSection("ReverseProxy");

builder.Services
    .AddReverseProxy()
    .LoadFromConfig(yarpConfig);

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = builder.Configuration["IdentityServiceUrl"];
        options.RequireHttpsMetadata = false;
        options.TokenValidationParameters.ValidateAudience = false;
        options.TokenValidationParameters.NameClaimType = "username";
        options.TokenValidationParameters.ValidateIssuer = false;
    });

var app = builder.Build();

// Configure the HTTP request pipeline.

app.MapReverseProxy();


app.Run();
