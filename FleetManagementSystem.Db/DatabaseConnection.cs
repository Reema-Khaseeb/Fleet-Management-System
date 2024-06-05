using FleetManagementSystem.Db.Interfaces;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace FleetManagementSystem.Db;

public class DatabaseConnection : IDatabaseConnection
{
    private readonly IConfiguration _configuration;

    public DatabaseConnection(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public NpgsqlConnection GetConnection()
    {
        return new NpgsqlConnection(_configuration.GetConnectionString("DefaultConnection"));
    }

    public async Task<NpgsqlConnection> GetConnectionAsync(CancellationToken cancellationToken)
    {
        var connection = new NpgsqlConnection(_configuration.GetConnectionString("DefaultConnection"));
        await connection.OpenAsync(cancellationToken);
        return connection;
    }
}
