using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenericMessagingService.Types.Config
{
    public class TemplateSettings
    {
        public string? TemplateStrategy { get; set; }
        public DatabaseTemplateSettings? DatabaseTemplates { get; set; }
        public RazorTemplateSettings? RazorTemplates { get; set; }
    }

    public class DatabaseTemplateSettings
    {
        public string ConnectionString { get; set; }
        /// <summary>
        /// Defaults to SqlServer
        /// Options are SqlServer,
        /// </summary>
        public string? Type { get; set; }
        public string Table { get; set; }
        public string LookupColumn { get; set; }
        public string TemplateColumn { get; set; }
        public string? SubjectColumn { get; set; }
    }

    public class RazorTemplateSettings
    {
        public string? BaseFolder { get; set; }
        /// <summary>
        /// A mapping from template name to folder location. Checked before regex.
        /// </summary>
        public Dictionary<string, string> Fixed { get; set; }

        /// <summary>
        /// A mapping from template name to folder location, using regex with match groups for the tempate name, and a format string for the folder location
        /// </summary>
        public Dictionary<string, string> Regex { get; set; }
    }
}
