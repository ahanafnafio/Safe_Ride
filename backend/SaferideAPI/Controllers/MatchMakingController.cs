using Microsoft.AspNetCore.Mvc;
using Saferide.Services;
using Saferide.Models;


[ApiController]
[Route("api/[controller]")]
public class MatchMakingController : ControllerBase
{
    private readonly MatchMaking _matchMaking;

    public MatchMakingController(MatchMaking matchMaking)
    {
        _matchMaking = matchMaking;
    }

    [HttpGet("test")]
    public async Task<IActionResult> Test()
    {
        var driver1 = new Driver("Alice", "Smith", "alice@test.com", "hash");
        driver1.GoOnline("Driver 1", 33.2050, -97.1400);

        var driver2 = new Driver("Bob", "Jones", "bob@test.com", "hash");
        driver2.GoOnline("Driver 2", 33.2300, -97.1200);

        _matchMaking.AddDriver(driver1);
        _matchMaking.AddDriver(driver2);

        var pickup = new Location("UNT", 33.2107, -97.1504);
        var dropoff = new Location("TWU", 33.2257, -97.1290);
        var ride = new Ride(pickup, dropoff, "Test ride", -1);

        var result = await _matchMaking.AddRide(ride);

        if (result == null)
        {
            return NotFound("No driver or route found.");
        }

        return Ok(result);
    }
}