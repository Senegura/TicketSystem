using TicketSystem.BL;
using TicketSystem.DAL;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Register CryptoService as singleton
builder.Services.AddSingleton<ICryptoService, CryptoService>();

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

app.MapFallbackToFile("/index.html");

app.Run();
