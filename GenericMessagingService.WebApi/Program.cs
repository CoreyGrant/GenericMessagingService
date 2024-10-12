using GenericMessagingService.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddMessagingServices();

// Build the application
var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseHttpsRedirection();

app.Run();