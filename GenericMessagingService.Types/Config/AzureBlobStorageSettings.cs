using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenericMessagingService.Types.Config
{
    public class AzureBlobStorageSettings
    {
        public string ConnectionString { get; set; }
        public string ContainerName { get; set; }
        public string FolderPath { get; set; }
    }
}
