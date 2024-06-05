using Serilog;
using FleetManagementSystem.Db;
using FleetManagementSystem.Db.Interfaces;
using FleetManagementSystem.Services;
using FleetManagementSystem.Services.Services;
using FleetManagementSystem.Services.utils;
using FleetManagementSystem.Services.Interfaces;
using FleetManagementSystem.Db.Repositories;

namespace FleetManagementSystem.Api;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        ConfigureLogging(builder);
        ConfigureServices(builder.Services, builder.Configuration);
        Configure(builder);

        var app = builder.Build();

        ConfigureMiddleware(app);

        app.Run();
    }

    private static void ConfigureLogging(WebApplicationBuilder builder)
    {
        var logDirectory = Path.Combine(Directory.GetCurrentDirectory(), "Logs");
        if (!Directory.Exists(logDirectory))
        {
            Directory.CreateDirectory(logDirectory);
        }

        builder.Host.UseSerilog((context, services, configuration) => configuration
            .ReadFrom.Configuration(context.Configuration)
            .Enrich.FromLogContext()
            .WriteTo.Console()
            .WriteTo.File(Path.Combine(logDirectory, "log-.txt"),
                rollingInterval: RollingInterval.Day));
    }

    private static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddCors(options =>
        {
            options.AddPolicy("AllowAll", builder =>
            {
                builder.AllowAnyOrigin()
                       .AllowAnyMethod()
                       .AllowAnyHeader();
            });
        });

        // Add repositories to the container.
        services.AddScoped<IVehicleRepository, VehicleRepository>();

        // Add services to the container.
        services.AddSingleton<IDatabaseConnection, DatabaseConnection>();
        services.AddScoped<IVehicleService, VehicleService>();
        services.AddScoped<VehicleInformationService>();
        services.AddScoped<DriverService>();
        services.AddScoped<RouteHistoryService>();
        services.AddScoped<GeofenceService>();

        services.AddControllers().AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.Converters.Add(new DataTableConverter());
            options.JsonSerializerOptions.PropertyNamingPolicy = null;
        });

        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
    }

    private static void Configure(WebApplicationBuilder builder)
    {
        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowAll", builder =>
            {
                builder.AllowAnyOrigin()
                       .AllowAnyMethod()
                       .AllowAnyHeader();
            });
        });
    }

    private static void ConfigureMiddleware(WebApplication app)
    {
        app.UseCors("AllowAll");

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
            app.UseDeveloperExceptionPage();
        }

        app.UseHttpsRedirection();
        app.UseRouting();
        app.UseAuthorization();
        app.MapControllers();
    }
}
