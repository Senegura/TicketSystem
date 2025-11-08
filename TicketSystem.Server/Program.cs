using System.Security.Claims;
using TicketSystem.BL;
using TicketSystem.DAL;
using TicketSystem.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Register CryptoService as singleton
builder.Services.AddSingleton<ICryptoService, CryptoService>();

// Register JwtService as singleton
builder.Services.AddSingleton<IJwtService, JwtService>();

// Register TicketDal with configured file path
var ticketFilePath = builder.Configuration["TicketStorage:FilePath"] ?? "App_Data/tickets.json";
builder.Services.AddSingleton<ITicketDal>(sp => new TicketDal(ticketFilePath));

// Register UserDal with configured database path
var userDbPath = builder.Configuration["UserStorage:DatabasePath"] ?? "App_Data/users.db";
builder.Services.AddSingleton<IUserDal>(sp => new UserDal(userDbPath));

// Register UserService
builder.Services.AddSingleton<IUserService, UserService>();

var app = builder.Build();

app.UseDefaultFiles();
app.UseStaticFiles();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

// Login endpoint
app.MapPost("/api/auth/login", async (
    LoginRequest? request,
    IUserService userService,
    IJwtService jwtService,
    HttpContext httpContext,
    IConfiguration configuration) =>
{
    try
    {
        // Sub-task 4.2: Request validation
        if (request == null || string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
        {
            return Results.BadRequest(new { error = "Username and password are required" });
        }

        // Sub-task 4.3: Authentication logic
        var userLogin = new UserLogin
        {
            Username = request.Username,
            Password = request.Password
        };

        var loginResult = await userService.LoginAsync(userLogin);

        if (!loginResult.Success)
        {
            return Results.Unauthorized();
        }

        // Sub-task 4.4: JWT token generation
        var claims = new[]
        {
            new Claim("userId", loginResult.UserId.ToString()),
            new Claim("userType", loginResult.UserType.ToString())
        };

        var expirationMinutes = configuration.GetValue<int>("Authentication:ExpirationMinutes", 1440);
        var token = jwtService.SignToken(claims, expirationMinutes);

        // Sub-task 4.5: Response with token and cookie
        var response = new LoginResponse
        {
            Token = token,
            UserId = loginResult.UserId,
            UserType = loginResult.UserType
        };

        var cookieName = configuration.GetValue<string>("Authentication:CookieName") ?? "AuthToken";
        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Expires = DateTimeOffset.UtcNow.AddMinutes(expirationMinutes)
        };

        httpContext.Response.Cookies.Append(cookieName, token, cookieOptions);

        return Results.Ok(response);
    }
    catch (Exception ex)
    {
        // Sub-task 4.6: Error handling for unexpected exceptions
        // Log the exception (in production, use proper logging)
        Console.Error.WriteLine($"Login error: {ex.Message}");
        return Results.Problem("An error occurred during authentication", statusCode: 500);
    }
});

app.MapFallbackToFile("/index.html");

app.Run();
