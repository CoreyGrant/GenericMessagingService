using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenericMessagingService.Services.Templating.Services.Formatting
{
    internal interface ITemplateFormattingService
    {
        Task<string> FormatTemplate(string template, IDictionary<string, string> data);
    }
}
