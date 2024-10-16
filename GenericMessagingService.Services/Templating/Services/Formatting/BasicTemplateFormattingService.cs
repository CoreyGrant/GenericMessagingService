using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace GenericMessagingService.Services.Templating.Services.Formatting
{
    internal class BasicTemplateFormattingService : ITemplateFormattingService
    {
        public async Task<string> FormatTemplate(string template, IDictionary<string, string> data)
        {
            foreach(var (k, v) in data)
            {
                template = template.Replace("{" + k + "}", v);
            }
            return template;
        }
    }
}
