using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenericMessagingService.Types.Attributes
{
    public enum ServiceType
    {
        Default = 1,
        Email = 2,
        Template = 3,
        Sms = 4,
        Pdf = 5
    }

    public abstract class InjectAttribute : Attribute
    {
        public readonly ServiceType type;

        public InjectAttribute(ServiceType type = ServiceType.Default)
        {
            this.type = type;
        }
    }

    public class InjectTransientAttribute : InjectAttribute
    {
        public InjectTransientAttribute(ServiceType type = ServiceType.Default) : base(type)
        {
        }
    }

    public class InjectSingletonAttribute : InjectAttribute
    {
        public InjectSingletonAttribute(ServiceType type = ServiceType.Default) : base(type)
        {
        }
    }
}
