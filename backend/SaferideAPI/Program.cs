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

