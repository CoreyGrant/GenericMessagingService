using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenericMessagingService.IntegrationTests
{
    public class IntegrationTestSettings
    {
        public bool All { get; set; }
        public string[]? Names { get; set; }
        public string HostUrl { get; set; }
        public string ApiKey { get; set; }
    }
}
