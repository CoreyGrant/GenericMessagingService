using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenericMessagingService.IntegrationTests.Attributes
{
    internal class TestAttribute : Attribute
    {
        public TestAttribute(string serverName) { ServerName = serverName; }
        public string ServerName { get; }
    }

    internal class TestNameAttribute : Attribute
    {
        public string Name { get; }
        public TestNameAttribute(string name)
        {
            Name = name;
        }
    }

    internal class ServerAttribute : Attribute
    {
        public string Name { get; }
        public ServerAttribute(string name)
        {
            Name = name;
        }
    }
}
