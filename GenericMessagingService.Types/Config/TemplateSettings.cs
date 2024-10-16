using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenericMessagingService.Types.Config
{
    public class ComboTemplateSettings
    {
        public string Strategy { get; set; }
        public Dictionary<string, TemplateSettings> Combo { get; set; }
    }

    public class TemplateSettings
    {
        public TemplateLocationSettings Location { get; set; }
        public TemplateFormattingSettings Formatting { get; set; }
    }

    public class TemplateLocationSettings
    {
        public string? Strategy { get; set; }
        public FolderTemplateLocationSettings? Folder { get; set; }
        public RemoteTemplateLocationSettings? Remote { get; set; }
        public DatabaseTemplateLocationSettings? Database { get; set; }
    }

    #region Location

    public class FolderTemplateLocationSettings
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

    public class RemoteTemplateLocationSettings
    {

    }

    public class DatabaseTemplateLocationSettings
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

    #endregion

    #region Formatting

    public class TemplateFormattingSettings
    {
        public RazorTemplateFormattingSettings? Razor { get; set; }
        public BasicTemplateFormattingSettings? Basic { get; set; }
        public HandlebarsTemplateFormattingSettings? Handlebars { get; set; }
    }

    public class RazorTemplateFormattingSettings
    {
    }

    public class HandlebarsTemplateFormattingSettings
    {

    }

    public class BasicTemplateFormattingSettings
    {

    }

    #endregion
}
