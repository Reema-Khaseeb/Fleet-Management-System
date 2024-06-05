using FleetManagementSystem.Services.Interfaces;
using FleetManagementSystem.Services.utils;
using Microsoft.AspNetCore.Mvc;

namespace FleetManagementSystem.Api.Controllers;

[ApiController]
[Route("api/1/vehicles")]
public class VehiclesController : ControllerBase
{
    private readonly IVehicleService _vehicleService;

    public VehiclesController(IVehicleService vehicleService)
    {
        _vehicleService = vehicleService;
    }

    [HttpPost]
    public async Task<IActionResult> AddVehicle([FromBody] GVAR gvar, CancellationToken cancellationToken)
    {
        var gvarResponse = await _vehicleService.AddVehicleAsync(gvar, cancellationToken);
        return gvarResponse.DicOfDic["Tags"]["STS"] == "1" ? Ok(gvarResponse) : BadRequest(gvarResponse);
    }

    [HttpDelete]
    public IActionResult DeleteVehicle([FromBody] GVAR gvar)
    {
        var gvarResponse = _vehicleService.DeleteVehicle(gvar);
        return gvarResponse.DicOfDic["Tags"]["STS"] == "1" ? Ok(gvarResponse) : BadRequest(gvarResponse);
    }

    [HttpPatch]
    public IActionResult UpdateVehicle([FromBody] GVAR gvar)
    {
        var gvarResponse = _vehicleService.UpdateVehicle(gvar);
        return gvarResponse.DicOfDic["Tags"]["STS"] == "1" ? Ok(gvarResponse) : BadRequest(gvarResponse);
    }

    [HttpGet("{vehicleID}")]
    public IActionResult GetVehicleDetails(long vehicleID)
    {
        var gvarResponse = _vehicleService.GetVehicleDetails(vehicleID);
        return gvarResponse.DicOfDic["Tags"]["STS"] == "1" ? Ok(gvarResponse) : BadRequest(gvarResponse);
    }

    [HttpGet]
    public IActionResult GetAllVehicles()
    {
        var gvarResponse = _vehicleService.GetVehicles();
        return gvarResponse.DicOfDic["Tags"]["STS"] == "1" ? Ok(gvarResponse) : BadRequest(gvarResponse);
    }

    [HttpGet("{vehicleID}/routehistory")]
    public IActionResult GetVehicleRouteHistory(long vehicleID, [FromQuery] long startEpoch, [FromQuery] long endEpoch)
    {
        var gvarResponse = _vehicleService.GetVehicleRouteHistory(vehicleID, startEpoch, endEpoch);
        return gvarResponse.DicOfDic["Tags"]["STS"] == "1" ? Ok(gvarResponse) : BadRequest(gvarResponse);
    }
}
