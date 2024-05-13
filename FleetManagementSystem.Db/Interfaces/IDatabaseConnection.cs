//using Microsoft.Data.SqlClient;
using Npgsql;

namespace FleetManagementSystem.Db.Interfaces
{
    public interface IDatabaseConnection
    {
        NpgsqlConnection GetConnection();
    }
}