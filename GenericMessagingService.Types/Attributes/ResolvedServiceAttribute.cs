using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenericMessagingService.Types.Attributes
{
    // Service resolvers choose which service to use based on which config exists
    // The service should specify which config they depend upon
    public class ResolvedServiceAttribute : Attribute
    {
        public Type RootConfigType { get; }
        public Type ConfigType { get; }
        public ResolvedServiceAttribute(Type rootConfigType, Type configType)
        {
            RootConfigType = rootConfigType;
            ConfigType = configType;
        }
    }
}
