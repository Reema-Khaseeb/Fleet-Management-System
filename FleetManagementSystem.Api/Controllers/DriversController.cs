using FleetManagementSystem.Services;
using FleetManagementSystem.Services.utils;
using Microsoft.AspNetCore.Mvc;

namespace FleetManagementSystem.Api.Controllers;

[ApiController]
[Route("api/1/drivers")]
public class DriversController : ControllerBase
{
    private readonly DriverService _driverService;

    public DriversController(DriverService driverService)
    {
        _driverService = driverService;
    }

    [HttpPost]
    public IActionResult AddDriver([FromBody] GVAR gvar)
    {
        var gvarResponse = _driverService.AddDriver(gvar);
        return gvarResponse.DicOfDic["Tags"]["STS"] == "1" ? Ok(gvarResponse) : BadRequest(gvarResponse);
    }

    [HttpPatch]
    public IActionResult UpdateDriver([FromBody] GVAR gvar)
    {
        var gvarResponse = _driverService.UpdateDriver(gvar);
        return gvarResponse.DicOfDic["Tags"]["STS"] == "1" ? Ok(gvarResponse) : BadRequest(gvarResponse);
    }

    [HttpGet]
    public IActionResult GetDrivers()
    {
        var gvarResponse = _driverService.GetDrivers();
        return gvarResponse.DicOfDic["Tags"]["STS"] == "1" ? Ok(gvarResponse) : BadRequest(gvarResponse);
    }

    [HttpDelete]
    public IActionResult DeleteDriver([FromBody] GVAR gvar)
    {
        var gvarResponse = _driverService.DeleteDriver(gvar);
        return gvarResponse.DicOfDic["Tags"]["STS"] == "1" ? Ok(gvarResponse) : BadRequest(gvarResponse);
    }
}
