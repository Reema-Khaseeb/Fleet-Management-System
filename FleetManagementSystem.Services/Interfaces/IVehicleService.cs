using FleetManagementSystem.Services.utils;

namespace FleetManagementSystem.Services.Interfaces;

public interface IVehicleService
{
    Task<GVAR> AddVehicleAsync(GVAR gvar, CancellationToken cancellationToken);
    Task<GVAR> DeleteVehicleAsync(GVAR gvar, CancellationToken cancellationToken);
    GVAR UpdateVehicle(GVAR gvar);
    GVAR GetVehicleDetails(long vehicleID);
    GVAR GetVehicles();
    GVAR GetVehicleRouteHistory(long vehicleID, long startEpoch, long endEpoch);
}
