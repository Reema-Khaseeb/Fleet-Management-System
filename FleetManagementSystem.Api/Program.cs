using Serilog;
using FleetManagementSystem.Db;
using FleetManagementSystem.Db.Interfaces;
using FleetManagementSystem.Services;
using FleetManagementSystem.Services.Services;
using FleetManagementSystem.Services.utils;

namespace FleetManagementSystem.Api;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Ensure log directory exists
        var logDirectory = Path.Combine(Directory.GetCurrentDirectory(), "Logs");
        if (!Directory.Exists(logDirectory))
        {
            Directory.CreateDirectory(logDirectory);
        }

        // Configure Serilog
        builder.Host.UseSerilog((context, services, configuration) => configuration
            .ReadFrom.Configuration(context.Configuration)
            .Enrich.FromLogContext()
            .WriteTo.Console()
            .WriteTo.File(Path.Combine(logDirectory, "log-.txt"), 
            rollingInterval: RollingInterval.Day)); // roll over every day. To manage file sizes and makes logs easier to search through.

        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowAll",
                builder =>
                {
                    builder.AllowAnyOrigin()
                           .AllowAnyMethod()
                           .AllowAnyHeader();
                });
        });

        // Add services to the container.
        builder.Services.AddSingleton<IDatabaseConnection, DatabaseConnection>();
        builder.Services.AddScoped<VehicleService>();
        builder.Services.AddScoped<VehicleInformationService>();
        builder.Services.AddScoped<DriverService>();
        builder.Services.AddScoped<RouteHistoryService>();
        builder.Services.AddScoped<GeofenceService>();

        // Add controllers to the services container.
        builder.Services.AddControllers().AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.Converters.Add(new DataTableConverter());
            options.JsonSerializerOptions.PropertyNamingPolicy = null;
        });

        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        var app = builder.Build();

        app.UseCors("AllowAll");

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
            app.UseDeveloperExceptionPage();
        }

        app.UseRouting();

        app.UseCors("AllowAll");

        app.UseHttpsRedirection();

        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}
