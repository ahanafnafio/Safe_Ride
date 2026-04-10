/*using Saferide.Services;

var builder = WebApplication.CreateBuilder(args);

// CORS configuration to allow requests from the frontend
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend",
        policy =>
        {
            policy.WithOrigins("http://127.0.0.1:5500")
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Register Authentication as a service
builder.Services.AddSingleton<Authentication>();

var app = builder.Build();

// CORS middleware
app.UseCors("AllowFrontend");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Keep this for later
app.UseAuthorization();

app.MapControllers();

app.Run();

//Adding this temporarily to see if the hashing works
{
    var testAuth = new Saferide.Services.Authentication();
    var testUser = testAuth.Register("Test", "User", "hashcheck@email.com", "1234", "Rider");
    if (testUser != null)
    {
        Console.WriteLine("\n[HASH TEST] User registered successfully.");
        Console.WriteLine($"[HASH TEST] Stored BCrypt Hash: {testUser.GetPasswordHash()}\n");
    }
}
//(Impplemented the password hashing by including the BCrypt.Net-Next library and also add a print statement in program.cs to see the hashing)
*/
using Saferide.Models;
using Saferide.Services;

class Program
{
    static void Main(string[] args)
    {
        // Create matchmaking system
        MatchMaking matchmaking = new MatchMaking();

        // Create drivers
        Driver d1 = new Driver("John", "Doe", "john@test.com", "hash");
        Driver d2 = new Driver("Jane", "Smith", "jane@test.com", "hash");
        Driver d3 = new Driver("Ron", "Johnson", "ron@test.com", "hash");

        // Put drivers online with locations
        d1.GoOnline("Location A", 33.0, -97.0); // Denton-ish
        d2.GoOnline("Location B", 32.7, -96.8); // Dallas-ish
        d3.GoOnline("Location C", 32.8, -96.9);

        // Add drivers to matchmaking
        matchmaking.AddDriver(d1);
        matchmaking.AddDriver(d2);
        matchmaking.AddDriver(d3);

        // Create a rider and ride
        Rider rider = new Rider("Bob", "Lee", "bob@test.com", "hash");

        Ride ride = rider.RequestRide(
            "Pickup", 32.75, -96.8,   // near Dallas
            "Dropoff", 33.0, -97.0,
            "Test ride"
        );

        // Add ride (this triggers matchmaking)
        matchmaking.AddRide(ride);

        // Check result
        Console.WriteLine("Ride Status: " + ride.GetStatus());
    }
}

