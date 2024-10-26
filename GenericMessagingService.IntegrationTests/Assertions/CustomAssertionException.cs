using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenericMessagingService.IntegrationTests.Assertions
{
    public class CustomAssertionException : Exception
    {
        public CustomAssertionException(string message): base(message) { }
    }
}
