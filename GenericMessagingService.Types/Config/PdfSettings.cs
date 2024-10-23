using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenericMessagingService.Types.Config
{
    public class PdfSettings
    {
        public string? TemplateStrategy { get; set; }
        public FolderSettings? Folder { get; set; }
        public AzureBlobStorageSettings? AzureBlobStorage { get; set; }
    }
}
