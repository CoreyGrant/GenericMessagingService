using GenericMessagingService.Client;
using GenericMessagingService.WebApi;
using Microsoft.AspNetCore.Builder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenericMessagingService.IntegrationTests.Helpers
{
    public static class ServerHelper
    {
        public static Server StartServer(GenericMessagingServiceSettings settings, string hostUrl)
        {
            return new Server(settings, hostUrl);
        }
    }

    public class Server : IAsyncDisposable
    {
        private readonly WebApplication app;

        public Server(GenericMessagingServiceSettings settings, string hostUrl)
        {
            var builder = WebApplication.CreateBuilder();
            
            // Add services to the container.
            builder.Services.AddWebApi(settings);

            // Build the application
            app = builder.Build();

            app.Urls.Add(hostUrl);

            app.UseWebApi(settings);
            app.UseStaticFiles();
            // Configure the HTTP request pipeline.
            app.UseHttpsRedirection();
            app.RunAsync();
        }

        public async ValueTask DisposeAsync()
        {
            await app.StopAsync();
            await app.DisposeAsync();
        }
    }
}
