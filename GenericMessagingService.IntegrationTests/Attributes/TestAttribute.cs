using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenericMessagingService.IntegrationTests.Attributes
{
    internal class TestFixtureAttribute : Attribute
    {
        public TestFixtureAttribute(string serverName, string testFixtureName = null)
        {
            ServerName = serverName;
            TestFixtureName = testFixtureName;
        }
        public string ServerName { get; }
        public string TestFixtureName { get; }
    }

    internal class TestAttribute : Attribute
    {
        public string TestName { get; }
        public TestAttribute(string testName = null)
        {
            TestName = testName;
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
