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

// Register TicketService
builder.Services.AddSingleton<ITicketService, TicketService>();

// Configure CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.SetIsOriginAllowed(origin => true)
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

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

app.UseCors("AllowFrontend");

app.UseAuthorization();

app.MapControllers();

// File upload validation helpers
bool ValidateFileSize(IFormFile file, long maxSizeInBytes = 5242880)
{
    return file.Length <= maxSizeInBytes;
}

bool ValidateFileType(IFormFile file)
{
    var allowedContentTypes = new[] { "image/jpeg", "image/png", "image/gif" };
    var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
    
    var contentType = file.ContentType.ToLowerInvariant();
    var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
    
    return allowedContentTypes.Contains(contentType) && allowedExtensions.Contains(extension);
}

// File storage logic
async Task<string> SaveUploadedFileAsync(IFormFile file)
{
    // Ensure App_Data/uploads directory exists
    var uploadsDirectory = Path.Combine("App_Data", "uploads");
    if (!Directory.Exists(uploadsDirectory))
    {
        Directory.CreateDirectory(uploadsDirectory);
    }
    
    // Generate unique filename with preserved extension
    var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
    var uniqueFileName = $"{Guid.NewGuid()}{extension}";
    var filePath = Path.Combine(uploadsDirectory, uniqueFileName);
    
    // Save file using FileStream
    using (var fileStream = new FileStream(filePath, FileMode.Create))
    {
        await file.CopyToAsync(fileStream);
    }
    
    return uniqueFileName;
}

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

// Ticket submission endpoint
app.MapPost("/api/tickets", async (
    HttpContext httpContext,
    ITicketService ticketService) =>
{
    try
    {
        var form = await httpContext.Request.ReadFormAsync();
        
        // Extract form fields
        var fullName = form["fullName"].ToString();
        var email = form["email"].ToString();
        var issueDescription = form["issueDescription"].ToString();
        var imageFile = form.Files.GetFile("image");
        
        // Validate request fields
        var validationErrors = new Dictionary<string, string>();
        
        // Validate fullName: not null/empty and at least 2 characters
        if (string.IsNullOrWhiteSpace(fullName))
        {
            validationErrors["fullName"] = "Full name is required";
        }
        else if (fullName.Trim().Length < 2)
        {
            validationErrors["fullName"] = "Full name must be at least 2 characters";
        }
        
        // Validate email: not null/empty and matches email format
        if (string.IsNullOrWhiteSpace(email))
        {
            validationErrors["email"] = "Email is required";
        }
        else if (!System.Text.RegularExpressions.Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
        {
            validationErrors["email"] = "Invalid email format";
        }
        
        // Validate issueDescription: not null/empty and at least 10 characters
        if (string.IsNullOrWhiteSpace(issueDescription))
        {
            validationErrors["issueDescription"] = "Issue description is required";
        }
        else if (issueDescription.Trim().Length < 10)
        {
            validationErrors["issueDescription"] = "Issue description must be at least 10 characters";
        }
        
        // Validate image file if present
        if (imageFile != null)
        {
            // Validate file size (max 5MB)
            if (!ValidateFileSize(imageFile))
            {
                validationErrors["image"] = "Image file size must not exceed 5MB";
            }
            
            // Validate file type (JPEG, PNG, GIF)
            if (!ValidateFileType(imageFile))
            {
                validationErrors["image"] = "Image file must be JPEG, PNG, or GIF";
            }
        }
        
        // Return HTTP 400 with structured error details if validation fails
        if (validationErrors.Count > 0)
        {
            return Results.BadRequest(new
            {
                error = "Validation failed",
                details = validationErrors
            });
        }
        
        // Handle file upload if image is present
        string? imageFileName = null;
        if (imageFile != null)
        {
            try
            {
                imageFileName = await SaveUploadedFileAsync(imageFile);
            }
            catch (Exception ex)
            {
                // Return HTTP 500 for server errors during file storage
                Console.Error.WriteLine($"File upload error: {ex.Message}");
                return Results.Problem(new
                {
                    error = "An error occurred while uploading the image"
                }.ToString(), statusCode: 500);
            }
        }
        
        // Create ticket with ITicketService
        var ticket = await ticketService.CreateTicketAsync(fullName, email, issueDescription, imageFileName);
        
        // Return HTTP 201 with created ticket data on success (with or without image)
        return Results.Created($"/api/tickets/{ticket.Id}", ticket);
    }
    catch (Exception ex)
    {
        // Return HTTP 500 with error message for server errors
        Console.Error.WriteLine($"Ticket submission error: {ex.Message}");
        return Results.Problem(new
        {
            error = "An error occurred while processing your request"
        }.ToString(), statusCode: 500);
    }
})
.RequireCors("AllowFrontend");

// Get all tickets endpoint with authentication and authorization
app.MapGet("/api/tickets", async (
    HttpContext httpContext,
    ITicketService ticketService,
    IConfiguration configuration) =>
{
    try
    {
        // Sub-task 4.1: Extract JWT token from AuthToken cookie
        var cookieName = configuration.GetValue<string>("Authentication:CookieName") ?? "AuthToken";
        var token = httpContext.Request.Cookies[cookieName];
        
        if (string.IsNullOrWhiteSpace(token))
        {
            return Results.Unauthorized();
        }
        
        // Sub-task 4.2: Implement JWT token validation logic
        var secretKey = configuration.GetValue<string>("Authentication:SecretKey");
        if (string.IsNullOrWhiteSpace(secretKey))
        {
            Console.Error.WriteLine("Authentication:SecretKey is not configured");
            return Results.Problem("Authentication configuration error", statusCode: 500);
        }
        
        var tokenHandler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
        var key = System.Text.Encoding.UTF8.GetBytes(secretKey);
        
        var validationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(key),
            ValidateIssuer = true,
            ValidIssuer = "TicketSystem",
            ValidateAudience = true,
            ValidAudience = "TicketSystemUsers",
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
        
        ClaimsPrincipal? claimsPrincipal;
        try
        {
            claimsPrincipal = tokenHandler.ValidateToken(token, validationParameters, out _);
        }
        catch (Exception)
        {
            // Token is invalid or expired
            return Results.Unauthorized();
        }
        
        // Sub-task 4.3: Implement role-based authorization logic
        var userTypeClaim = claimsPrincipal.FindFirst("userType");
        
        if (userTypeClaim == null)
        {
            return Results.Forbid();
        }
        
        if (!int.TryParse(userTypeClaim.Value, out int userType))
        {
            return Results.Forbid();
        }
        
        // Allow access only for User (1) or Admin (2)
        if (userType != 1 && userType != 2)
        {
            return Results.Forbid();
        }
        
        // Sub-task 4.4: Implement ticket retrieval and response handling
        var tickets = await ticketService.GetAllTicketsAsync();
        
        return Results.Ok(tickets);
    }
    catch (Exception ex)
    {
        // Sub-task 4.5: Implement error handling for exceptions
        Console.Error.WriteLine($"Error retrieving tickets: {ex.Message}");
        return Results.Problem("An error occurred while retrieving tickets", statusCode: 500);
    }
})
.RequireCors("AllowFrontend"); // Sub-task 4.6: Add CORS policy to endpoint

app.MapFallbackToFile("/index.html");

app.Run();
