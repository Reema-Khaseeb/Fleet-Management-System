using FleetManagementSystem.Services.Services;
using Microsoft.AspNetCore.Mvc;

namespace FleetManagementSystem.Api.Controllers;

[ApiController]
[Route("api/1/geofences")]
public class GeofencesController : ControllerBase
{
    private readonly GeofenceService _geofenceService;

    public GeofencesController(GeofenceService geofenceService)
    {
        _geofenceService = geofenceService;
    }

    [HttpGet]
    public IActionResult GetAllGeofences()
    {
        var gvarResponse = _geofenceService.GetAllGeofences();
        return gvarResponse.DicOfDic["Tags"]["STS"] == "1" ? Ok(gvarResponse) : BadRequest(gvarResponse);
    }

    [HttpGet("circular")]
    public IActionResult GetCircularGeofences()
    {
        var gvarResponse = _geofenceService.GetCircularGeofences();
        return gvarResponse.DicOfDic["Tags"]["STS"] == "1" ? Ok(gvarResponse) : BadRequest(gvarResponse);
    }

    [HttpGet("rectangular")]
    public IActionResult GetRectangularGeofences()
    {
        var gvarResponse = _geofenceService.GetRectangularGeofences();
        return gvarResponse.DicOfDic["Tags"]["STS"] == "1" ? Ok(gvarResponse) : BadRequest(gvarResponse);
    }

    [HttpGet("polygon")]
    public IActionResult GetPolygonGeofences()
    {
        var gvarResponse = _geofenceService.GetPolygonGeofences();
        return gvarResponse.DicOfDic["Tags"]["STS"] == "1" ? Ok(gvarResponse) : BadRequest(gvarResponse);
    }
}
