using Saferide.Models;
using Saferide.Services;

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

// Allows RoutingService
builder.Services.AddHttpClient<RoutingService>();
builder.Services.AddSingleton<MatchMaking>();

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

Authentication authentication = app.Services.GetRequiredService<Authentication>();
MatchMaking matchMaking = app.Services.GetRequiredService<MatchMaking>();

Driver? driver1 = authentication.Register("John", "Renaldo", "JohnRenaldo@yahoo.com", "Pass123", "Driver") as Driver;
Driver? driver2 = authentication.Register("Ron", "Donald", "RonDonald@yahoo.com", "Pass123", "Driver") as Driver;
Driver? driver3 = authentication.Register("Don", "Donald", "DonDonald@yahoo.com", "Pass123", "Driver") as Driver;

Session? sessionIdD1 = authentication.Login("JohnRenaldo@yahoo.com", "Pass123");
Session? sessionIdD2 = authentication.Login("RonDonald@yahoo.com", "Pass123");
Session? sessionIdD3 = authentication.Login("DonDonald@yahoo.com", "Pass123");

if (driver1 != null)
{
    driver1.GoOnline("University of North Texas, Denton, TX", 33.2109, -97.1506);
    matchMaking.AddDriver(driver1);
}

if (driver2 != null)
{
    driver2.GoOnline("Denton Square, Denton, TX", 33.2148, -97.1331);
    matchMaking.AddDriver(driver2);
}

if (driver3 != null)
{
    driver3.GoOnline("Golden Triangle Mall, Denton, TX", 33.1887, -97.1067);
    matchMaking.AddDriver(driver3);
}


app.Run();


