using FleetManagementSystem.Services.utils;

namespace FleetManagementSystem.Services.Interfaces;

public interface IVehicleService
{
    Task<GVAR> AddVehicleAsync(GVAR gvar, CancellationToken cancellationToken);
    GVAR DeleteVehicle(GVAR gvar);
    GVAR UpdateVehicle(GVAR gvar);
    GVAR GetVehicleDetails(long vehicleID);
    GVAR GetVehicles();
    GVAR GetVehicleRouteHistory(long vehicleID, long startEpoch, long endEpoch);
}
