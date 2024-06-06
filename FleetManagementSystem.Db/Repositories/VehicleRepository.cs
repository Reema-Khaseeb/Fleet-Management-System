using FleetManagementSystem.Db.Interfaces;
using Npgsql;
using Serilog;

namespace FleetManagementSystem.Db.Repositories;

public class VehicleRepository : IVehicleRepository
{
    private readonly IDatabaseConnection _databaseConnection;
    private readonly ILogger _logger;

    public VehicleRepository(IDatabaseConnection databaseConnection, ILogger logger)
    {
        _databaseConnection = databaseConnection;
        _logger = logger;
    }

    public async Task AddVehicleAsync(long vehicleNumber, string vehicleType, CancellationToken cancellationToken)
    {
        await using var connection = await _databaseConnection.GetConnectionAsync(cancellationToken);
        await using var transaction = await connection.BeginTransactionAsync(cancellationToken);
        try
        {
            var query = "INSERT INTO \"Vehicles\" (\"VehicleNumber\", \"VehicleType\") VALUES (@VehicleNumber, @VehicleType)";
            await using var command = new NpgsqlCommand(query, connection, transaction);
            command.Parameters.AddWithValue("@VehicleNumber", vehicleNumber);
            command.Parameters.AddWithValue("@VehicleType", vehicleType);
            await command.ExecuteNonQueryAsync(cancellationToken);

            await transaction.CommitAsync(cancellationToken);
            _logger.Information($"Vehicle with number {vehicleNumber} and type {vehicleType} inserted into database");
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync(cancellationToken);
            _logger.Error(ex, "Error occurred while inserting vehicle to database, transaction rolled back");
            throw;
        }
    }

    public async Task DeleteVehicleAsync(long vehicleId, CancellationToken cancellationToken)
    {
        await using var connection = await _databaseConnection.GetConnectionAsync(cancellationToken);
        await using var transaction = await connection.BeginTransactionAsync(cancellationToken);
        try
        {
            var query = "DELETE FROM \"Vehicles\" WHERE \"VehicleID\" = @VehicleID";
            await using var command = new NpgsqlCommand(query, connection, transaction);
            command.Parameters.AddWithValue("@VehicleID", vehicleId);
            int rowsAffected = await command.ExecuteNonQueryAsync(cancellationToken);

            if (rowsAffected == 0)
            {
                _logger.Warning($"No vehicle found with ID: {vehicleId}");
                throw new Exception($"No vehicle found with ID: {vehicleId}");
            }

            await transaction.CommitAsync(cancellationToken);
            _logger.Information($"Vehicle with ID {vehicleId} deleted from database");
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync(cancellationToken);
            _logger.Error(ex, "Error occurred while deleting vehicle from database, transaction rolled back");
            throw;
        }
    }

    public async Task UpdateVehicleAsync(long vehicleId,
        Dictionary<string, object> updateData, CancellationToken cancellationToken)
    {
        await using var connection = await _databaseConnection.GetConnectionAsync(cancellationToken);
        await using var transaction = await connection.BeginTransactionAsync(cancellationToken);

        try
        {
            var updateParts = updateData.Select(kv => $"\"{kv.Key}\" = @{kv.Key}").ToList();
            string updateQuery = $"UPDATE \"Vehicles\" SET {string.Join(", ", updateParts)} WHERE \"VehicleID\" = @VehicleID";

            await using var command = new NpgsqlCommand(updateQuery, connection, transaction);
            foreach (var param in updateData)
            {
                command.Parameters.AddWithValue("@" + param.Key, param.Value);
            }
            command.Parameters.AddWithValue("@VehicleID", vehicleId);

            int rowsAffected = await command.ExecuteNonQueryAsync(cancellationToken);
            if (rowsAffected == 0)
            {
                _logger.Warning($"No rows were updated for vehicle ID {vehicleId}");
            }

            await transaction.CommitAsync(cancellationToken);
            _logger.Information($"Vehicle with ID {vehicleId} updated in database");
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync(cancellationToken);
            _logger.Error(ex, "Error occurred while updating vehicle in database, transaction rolled back");
            throw;
        }
    }

    public async Task<bool> IsVehicleNumberExistsAsync(long vehicleNumber,
        long excludedVehicleId, CancellationToken cancellationToken)
    {
        await using var connection = await _databaseConnection.GetConnectionAsync(cancellationToken);
        var query = "SELECT EXISTS (SELECT 1 FROM \"Vehicles\" WHERE \"VehicleNumber\" = @VehicleNumber AND \"VehicleID\" <> @VehicleID)";

        await using var command = new NpgsqlCommand(query, connection);
        command.Parameters.AddWithValue("@VehicleNumber", vehicleNumber);
        command.Parameters.AddWithValue("@VehicleID", excludedVehicleId);

        return (bool)await command.ExecuteScalarAsync(cancellationToken);
    }
}
