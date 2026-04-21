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

        public MatchController(MatchMaking matchMaking)
        {
            _matchMaking = matchMaking;
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
            Location pickup = new Location(request.PickupAddress, request.PickupLat, request.PickupLon);
            Location dropoff = new Location(request.DropoffAddress, request.DropoffLat, request.DropoffLon);

            // Should call Rider.RequestRide( all parameters) to verify vehicleId and will return a Ride object
            // Ride ride = Rider.RequestRide( ... )
            // var result = await _matchMaking.AddRide(ride);

            Ride ride = new Ride(
                pickup,
                dropoff,
                request.Notes,
                request.VehicleId
            );

            var result = await _matchMaking.AddRide(ride);

            if (result == null)
            {
                return NotFound("No driver or route found.");
            }

            return Ok(result);
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
        public string PickupAddress { get; set; } = "";
        public double PickupLat { get; set; }
        public double PickupLon { get; set; }

        public string DropoffAddress { get; set; } = "";
        public double DropoffLat { get; set; }
        public double DropoffLon { get; set; }

        public string Notes { get; set; } = "";
        public int VehicleId { get; set; }
    }
}