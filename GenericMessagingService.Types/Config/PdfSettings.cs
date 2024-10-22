using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenericMessagingService.Types.Config
{
    public class PdfSettings
    {
        // need a way to specify where to store the output
        public FolderSettings? Folder { get; set; }
        public AzureBlobStorageSettings? AzureBlobStorage { get; set; }
    }
}
