using FleetManagementSystem.Services;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Concurrent;

namespace FleetManagementSystem.Api.Controllers;

[ApiController]
[Route("api/1/vehicles")]
public class VehiclesController : ControllerBase
{
    private readonly VehicleService _vehicleService;

    public VehiclesController(VehicleService vehicleService)
    {
        _vehicleService = vehicleService;
    }

    [HttpPost]
    public IActionResult AddVehicle([FromBody] GVAR gvar)
    {
        var result = _vehicleService.AddVehicle(gvar);
        var gvarResponse = new GVAR();

        // Initialize the Tags dictionary to ensure it exists
        gvarResponse.DicOfDic["Tags"] = new ConcurrentDictionary<string, string>();

        if (!result.Success)
        {
            gvarResponse.DicOfDic["Tags"]["STS"] = "0";
            return BadRequest(gvarResponse);
        }
        else
        {
            gvarResponse.DicOfDic["Tags"]["STS"] = "1";
            return Ok(gvarResponse);
        }
    }

    [HttpGet]
    public IActionResult GetVehiclesGvar()
    {
        var gvar = _vehicleService.GetVehiclesGvar();
        return Ok(gvar);
    }

    [HttpDelete]
    public IActionResult DeleteVehicle([FromBody] GVAR gvar)
    {
        var gvarResponse = _vehicleService.DeleteVehicle(gvar);
        if (gvarResponse.DicOfDic["Tags"]["STS"] == "1")
        {
            return Ok(gvarResponse);
        }
        else
        {
            return BadRequest(gvarResponse);
        }
    }

    [HttpPatch]
    public IActionResult UpdateVehicle([FromBody] GVAR gvar)
    {
        var gvarResponse = _vehicleService.UpdateVehicle(gvar);
        if (gvarResponse.DicOfDic["Tags"]["STS"] == "1")
        {
            return Ok(gvarResponse);
        }
        else
        {
            return BadRequest(gvarResponse);
        }
    }
}
