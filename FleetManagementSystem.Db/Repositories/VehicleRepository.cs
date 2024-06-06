using FleetManagementSystem.Db.Interfaces;
using Npgsql;
using Serilog;
using static FleetManagementSystem.Common.DatabaseConstants;

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
            var query = $"INSERT INTO \"{Tables.Vehicles}\" (\"{VehiclesFields.VehicleNumber}\", \"{VehiclesFields.VehicleType}\") VALUES (@{VehiclesFields.VehicleNumber}, @{VehiclesFields.VehicleType})";
            await using var command = new NpgsqlCommand(query, connection, transaction);
            command.Parameters.AddWithValue($"@{VehiclesFields.VehicleNumber}", vehicleNumber);
            command.Parameters.AddWithValue($"@{VehiclesFields.VehicleType}", vehicleType);
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
            var query = $"DELETE FROM \"{Tables.Vehicles}\" WHERE \"{VehiclesFields.VehicleID}\" = @{VehiclesFields.VehicleID}";
            await using var command = new NpgsqlCommand(query, connection, transaction);
            command.Parameters.AddWithValue($"@{VehiclesFields.VehicleID}", vehicleId);
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

    public async Task UpdateVehicleAsync(long vehicleId, Dictionary<string, object> updateData, CancellationToken cancellationToken)
    {
        await using var connection = await _databaseConnection.GetConnectionAsync(cancellationToken);
        await using var transaction = await connection.BeginTransactionAsync(cancellationToken);

        try
        {
            var updateParts = new List<string>();
            foreach (var key in updateData.Keys)
            {
                updateParts.Add($"\"{key}\" = @{key}");
            }

            string updateQuery = $"UPDATE \"{Tables.Vehicles}\" SET {string.Join(", ", updateParts)} WHERE \"{VehiclesFields.VehicleID}\" = @{VehiclesFields.VehicleID}";

            await using var command = new NpgsqlCommand(updateQuery, connection, transaction);
            foreach (var param in updateData)
            {
                command.Parameters.AddWithValue("@" + param.Key, param.Value);
            }
            command.Parameters.AddWithValue($"@{VehiclesFields.VehicleID}", vehicleId);

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

    public async Task<bool> IsVehicleNumberExistsAsync(long vehicleNumber, long excludedVehicleId, CancellationToken cancellationToken)
    {
        await using var connection = await _databaseConnection.GetConnectionAsync(cancellationToken);
        var query = $"SELECT EXISTS (SELECT 1 FROM \"{Tables.Vehicles}\" WHERE \"{VehiclesFields.VehicleNumber}\" = @{VehiclesFields.VehicleNumber} AND \"{VehiclesFields.VehicleID}\" <> @{VehiclesFields.VehicleID})";

        await using var command = new NpgsqlCommand(query, connection);
        command.Parameters.AddWithValue($"@{VehiclesFields.VehicleNumber}", vehicleNumber);
        command.Parameters.AddWithValue($"@{VehiclesFields.VehicleID}", excludedVehicleId);

        return (bool)await command.ExecuteScalarAsync(cancellationToken);
    }
}
