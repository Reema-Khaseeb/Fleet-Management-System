namespace FleetManagementSystem.Db.Interfaces;

public interface IVehicleRepository
{
    Task AddVehicleAsync(long vehicleNumber, string vehicleType, CancellationToken cancellationToken);
    Task DeleteVehicleAsync(long vehicleId, CancellationToken cancellationToken);
    Task UpdateVehicleAsync(long vehicleId, Dictionary<string, object> updateData, CancellationToken cancellationToken);
    Task<bool> IsVehicleNumberExistsAsync(long vehicleNumber, long excludedVehicleId, CancellationToken cancellationToken);
}