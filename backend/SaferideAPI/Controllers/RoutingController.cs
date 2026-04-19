using Microsoft.AspNetCore.Mvc;
using Saferide.Services;
using Saferide.Models;

namespace Saferide.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RoutingController : ControllerBase
    {
        private readonly RoutingService _routingService;

        public RoutingController(RoutingService routingService)
        {
            _routingService = routingService;
        }


        // Endpoint frontend will call
        [HttpPost("route")]
        public async Task<IActionResult> ComputeRoute([FromBody] RouteRequest request)
        {
            // Make sure request body was sent
            if (request == null)
            {
                return BadRequest("Request body is required.");
            }

            // Convert frontend JSON into Location objects
            var origin = new Location(request.OriginAddress, request.OriginLat, request.OriginLon);

            var destination = new Location(request.DestinationAddress, request.DestinationLat, request.DestinationLon);

            // RoutingService call  calls Google Routes API
            var result = await _routingService.ComputeRouteAsync(origin, destination);

            // If route not found
            if (result == null)
            {
                return NotFound("No route found.");
            }

            // Return route to frontend
            return Ok(result);
        }
    }
}