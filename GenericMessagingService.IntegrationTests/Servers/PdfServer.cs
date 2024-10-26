using GenericMessagingService.Client;
using GenericMessagingService.IntegrationTests.Attributes;
using GenericMessagingService.Types.Config;
using GenericMessagingService.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenericMessagingService.IntegrationTests.Servers
{
    [Server("Pdf")]
    internal class PdfServer : BaseServer
    {
        protected override AppSettings AppSettings => GetAppSettings("pdfServer.json");
    }
}
