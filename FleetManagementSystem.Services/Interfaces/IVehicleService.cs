using FleetManagementSystem.Services.utils;

namespace FleetManagementSystem.Services.Interfaces;

public interface IVehicleService
{
    Task<GVAR> AddVehicleAsync(GVAR gvar, CancellationToken cancellationToken);
    Task<GVAR> DeleteVehicleAsync(GVAR gvar, CancellationToken cancellationToken);
    Task<GVAR> UpdateVehicleAsync(GVAR gvar, CancellationToken cancellationToken);
    GVAR GetVehicleDetails(long vehicleID);
    GVAR GetVehicles();
    GVAR GetVehicleRouteHistory(long vehicleID, long startEpoch, long endEpoch);
}
