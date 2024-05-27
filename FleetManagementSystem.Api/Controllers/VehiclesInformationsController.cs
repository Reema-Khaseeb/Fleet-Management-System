using FleetManagementSystem.Services.Services;
using FleetManagementSystem.Services.utils;
using Microsoft.AspNetCore.Mvc;

namespace FleetManagementSystem.Api.Controllers;

[ApiController]
[Route("api/1/vehicles-Informations")]
public class VehiclesInformationsController : ControllerBase
{
    private readonly VehicleInformationService _vehicleInfoService;

    public VehiclesInformationsController(VehicleInformationService vehicleInfoService)
    {
        _vehicleInfoService = vehicleInfoService;
    }

    [HttpPost]
    public IActionResult AddVehicleInfo([FromBody] GVAR gvar)
    {
        var gvarResponse = _vehicleInfoService.AddVehicleInfo(gvar);
        return gvarResponse.DicOfDic["Tags"]["STS"] == "1" ? Ok(gvarResponse) : BadRequest(gvarResponse);
    }

    [HttpGet]
    public IActionResult GetVehiclesInfo()
    {
        var gvarResponse = _vehicleInfoService.GetVehiclesInformations();
        return gvarResponse.DicOfDic["Tags"]["STS"] == "1" ? Ok(gvarResponse) : BadRequest(gvarResponse);
    }

    [HttpDelete]
    public IActionResult DeleteVehicleInfo([FromBody] GVAR gvar)
    {
        var gvarResponse = _vehicleInfoService.DeleteVehicleInfo(gvar);
        return gvarResponse.DicOfDic["Tags"]["STS"] == "1" ? Ok(gvarResponse) : BadRequest(gvarResponse);
    }

    [HttpPatch]
    public IActionResult UpdateVehicleInfo([FromBody] GVAR gvar)
    {
        var gvarResponse = _vehicleInfoService.UpdateVehicleInfo(gvar);
        return gvarResponse.DicOfDic["Tags"]["STS"] == "1" ? Ok(gvarResponse) : BadRequest(gvarResponse);
    }
}
