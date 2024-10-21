using GenericMessagingService.Services;
using GenericMessagingService.WebApi;

var builder = WebApplication.CreateBuilder(args);

var gmsSettings = new GenericMessagingServiceSettings
{
    BindingType = BindingType.Web,
    ConfigPath = "Config/defaultConfig.json",
    ApiKey = "testapikey",
};

// Add services to the container.
builder.Services.AddWebApi(gmsSettings);

// Build the application
var app = builder.Build();

app.UseWebApi(gmsSettings);
app.UseStaticFiles();
// Configure the HTTP request pipeline.
app.UseHttpsRedirection();

app.Run();