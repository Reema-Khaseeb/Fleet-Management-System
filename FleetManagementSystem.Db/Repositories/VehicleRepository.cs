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
}
