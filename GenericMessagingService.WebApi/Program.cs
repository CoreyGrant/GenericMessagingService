using GenericMessagingService.Services;
using GenericMessagingService.WebApi;

var builder = WebApplication.CreateBuilder(args);

var gmsSettings = new GenericMessagingServiceSettings
{
    BindingType = BindingType.Web,
    ConfigPath = "Config/defaultConfig.json"
};

// Add services to the container.
builder.Services.AddWebApi(gmsSettings);

// Build the application
var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseHttpsRedirection();

app.Run();