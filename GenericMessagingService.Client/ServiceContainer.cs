using System.ComponentModel.Design;

namespace GenericMessagingService.Client
{
    public static class ServiceContainer
    {
        public static IServiceContainer AddMessagingClient(this IServiceContainer container)
        {

            return container;
        }
    }
}