using Microsoft.AspNetCore.Mvc;
using Saferide.Services;
using Saferide.Models;

namespace Saferide.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MatchController : ControllerBase
    {
        private readonly MatchMaking _matchMaking;
        private readonly Authentication _authentication;
        public MatchController(MatchMaking matchMaking, Authentication authentication)
        {
            _matchMaking = matchMaking;
            _authentication = authentication;
        }

        // ================= DRIVER =================

        // POST: api/match/driver/online
        [HttpPost("driver/online")]
        public IActionResult DriverGoOnline([FromBody] DriverOnlineRequest request)
        {
            Driver driver = new Driver(
                request.FirstName,
                request.LastName,
                request.Email,
                request.PasswordHash
            );

            driver.GoOnline(request.Address, request.Lat, request.Lon);

            _matchMaking.AddDriver(driver);

            return Ok("Driver is now online.");
        }

        // POST: api/match/driver/offline
        [HttpPost("driver/offline")]
        public IActionResult DriverGoOffline([FromBody] DriverOfflineRequest request)
        {
            // Not implemented yet
            return Ok("Driver offline endpoint not fully implemented yet.");
        }

        // ================= RIDE =================

        // POST: api/match/ride/request
        [HttpPost("ride/request")]
        public async Task<IActionResult> RequestRide([FromBody] RideRequest request)
        {
            var tempRider = _authentication.GetRiderBySessionId(request.SessionId);
            if (tempRider == null)
            {
                return Unauthorized("Invalid or expired session.");
            }
            Ride newRide = tempRider.RequestRide(request.VehicleId, request.PickupAddress, request.PickupLat, request.PickupLon,
                                                   request.DropoffAddress, request.DropoffLat, request.DropoffLon, request.Notes);
            var result = await _matchMaking.AddRide(newRide);

            if (result == null)
            {
                return NotFound("No driver or route found.");
            }

            return Ok(result);
        }

        // ================= VEHICLE =================

        // POST: api/match/vehicle/add
        [HttpPost("vehicle/add")]
        public IActionResult AddVehicle([FromBody] AddVehicleRequest request)
        {
            var rider = _authentication.GetRiderBySessionId(request.SessionId);

            if (rider == null)
            {
                return Unauthorized("Invalid or expired session.");
            }

            rider.AddVehicle(
                request.Make,
                request.Model,
                request.Color,
                request.Plate,
                request.Notes
            );

            return Ok(rider.GetVehicles().Select(v => new {
                id = v.GetVehicleId(),
                make = request.Make,
                model = request.Model
            }));
        }

        // GET: api/match/myvehicles?sessionId=[sessionID]
        [HttpGet("myvehicles")]
        public IActionResult GetMyVehicles([FromQuery] string sessionId)
        {
            Rider? rider = _authentication.GetRiderBySessionId(sessionId);

            if (rider == null)
            {
                return Unauthorized("Invalid session.");
            }

            var vehicles = rider.GetVehicles();

            return Ok(vehicles);
        }
    }

    // ================= DTOs =================

    public class DriverOnlineRequest
    {
        public string FirstName { get; set; } = "";
        public string LastName { get; set; } = "";
        public string Email { get; set; } = "";
        public string PasswordHash { get; set; } = "";

        public string Address { get; set; } = "";
        public double Lat { get; set; }
        public double Lon { get; set; }
    }

    public class DriverOfflineRequest
    {
        public string Email { get; set; } = "";
    }

    public class RideRequest
    {
        public string SessionId { get; set; } = "";
        public string PickupAddress { get; set; } = "";
        public double PickupLat { get; set; }
        public double PickupLon { get; set; }

        public string DropoffAddress { get; set; } = "";
        public double DropoffLat { get; set; }
        public double DropoffLon { get; set; }

        public string Notes { get; set; } = "";
        public int VehicleId { get; set; }
    }

    public class AddVehicleRequest
    {
        public string SessionId { get; set; } = "";

        public string Make { get; set; } = "";
        public string Model { get; set; } = "";
        public string Color { get; set; } = "";
        public string Plate { get; set; } = "";
        public string Notes { get; set; } = "";
    }
}