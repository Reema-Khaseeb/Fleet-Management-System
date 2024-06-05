namespace FleetManagementSystem.Db.Interfaces;

public interface IVehicleRepository
{
    Task AddVehicleAsync(long vehicleNumber, string vehicleType, CancellationToken cancellationToken);
    Task DeleteVehicleAsync(long vehicleId, CancellationToken cancellationToken);
}