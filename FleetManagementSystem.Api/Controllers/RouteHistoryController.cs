using FleetManagementSystem.Services.Services;
using FleetManagementSystem.Services.utils;
using Microsoft.AspNetCore.Mvc;

namespace FleetManagementSystem.Api.Controllers
{
    [ApiController]
    [Route("api/1/route-history")]
    public class RouteHistoryController : ControllerBase
    {
        private readonly RouteHistoryService _routeHistoryService;

        public RouteHistoryController(RouteHistoryService routeHistoryService)
        {
            _routeHistoryService = routeHistoryService;
        }

        [HttpPost]
        public IActionResult AddHistoricalData([FromBody] GVAR gvar)
        {
            var gvarResponse = _routeHistoryService.AddHistoricalData(gvar);
            return gvarResponse.DicOfDic["Tags"]["STS"] == "1" ? Ok(gvarResponse) : BadRequest(gvarResponse);
        }
    }
}

