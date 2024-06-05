using Npgsql;

namespace FleetManagementSystem.Db.Interfaces
{
    public interface IDatabaseConnection
    {
        NpgsqlConnection GetConnection();
        Task<NpgsqlConnection> GetConnectionAsync(CancellationToken cancellationToken);
    }
}