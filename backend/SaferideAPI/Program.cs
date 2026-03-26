/*var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();*/

/* ****** TESTING ******* */
using Saferide.Models;
using Saferide.Services;

var auth = new Authentication();

Console.WriteLine("=== TESTING AUTH ===");

// Register
var user1 = auth.Register("Keaton", "Morales", "keaton@email.com", "1234");
Console.WriteLine(user1 != null ? "User registered" : "Registration failed");
Console.WriteLine($"User id: {user1?.GetUserId()}");

var user3 = auth.Register("Keaton", "Morales", "keaton@gmail.com", "1234");
Console.WriteLine($"User id: {user3?.GetUserId()}");

// Duplicate register
var user2 = auth.Register("Keaton", "Morales", "keaton@email.com", "1234");
Console.WriteLine(user2 != null ? "Duplicate allowed" : "Duplicate blocked");
// Login success
var session1 = auth.Login("keaton@email.com", "1234");
Console.WriteLine(session1 != null ? "Login success" : "Login failed");

// Login fail
var session2 = auth.Login("keaton@email.com", "wrong");
Console.WriteLine(session2 != null ? "Bad login allowed" : "Bad login blocked");

// Logout test
if (session1 != null)
{
    Console.WriteLine("Session active: " + session1.IsValid());
    auth.Logout(session1.GetSessionId());
    Console.WriteLine("Session active after logout: " + session1.IsValid());
}
