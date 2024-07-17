using GeoLocateX.Data;
using GeoLocateX.Domain.Configuration;
using GeoLocateX.Domain.Interfaces;
using GeoLocateX.Services;
using GeoLocateX.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.SqlServer.Storage.Internal;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "GeoLocateX", Version = "v1" });

    c.IgnoreObsoleteProperties();
    c.IgnoreObsoleteActions();
    c.CustomSchemaIds(type => type.FullName);
});

var connectionString = builder.Configuration.GetConnectionString("SqlServerConnection") ?? throw new ArgumentNullException(nameof(SqlServerConnection)); ;

builder.Services.AddDbContext<ApplicationContext>(options =>
{
    options.UseSqlServer(connectionString);
});

builder.Services.AddHealthChecks()
    .AddSqlServer(
            connectionString: connectionString,
            healthQuery: "SELECT 1;",
            name: "sql",
            failureStatus: HealthStatus.Degraded,
            tags: new string[] { "db", "sql", "sqlserver" });

builder.Services.AddHttpClient<IGeoIpService, GeoIpService>();

builder.Services.AddSingleton<IIPBaseClient, IPBaseClient>();
builder.Services.AddSingleton<IQueueService, QueueService>();
builder.Services.AddScoped<IGeoIpService, GeoIpService>();

builder.Services.AddScoped<IBatchProcessRepository, BatchProcessRepository>();
builder.Services.AddScoped<IBatchProcessItemRepository, BatchProcessItemRepository>();
builder.Services.AddScoped<IBatchProcessItemResponseRepository, BatchProcessItemResponseRepository>();


builder.Services.AddScoped<GeoIpBackgroundService>();
builder.Services.AddHostedService(provider =>
{
    using var scope = provider.CreateScope();
    return scope.ServiceProvider.GetRequiredService<GeoIpBackgroundService>();
});

builder.Services.AddControllers();

builder.Services.Configure<IpBaseConfig>(builder.Configuration.GetSection("IpBaseConfig"));


builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.MapHealthChecks("/health");

app.Run();
