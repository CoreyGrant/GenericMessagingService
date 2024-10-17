using GenericMessagingService.Types.Config;

namespace GenericMessagingService.WebApi
{
    public class GenericMessagingServiceSettings
    {
        public string? ConfigPath { get; set; }
        public AppSettings Config{ get; set; }
        public BindingType BindingType { get; set; }
    }

    [Flags]
    public enum BindingType
    {
        None = 0,
        Web = 1,
        AzureServiceBus = 2,
    } 
}
