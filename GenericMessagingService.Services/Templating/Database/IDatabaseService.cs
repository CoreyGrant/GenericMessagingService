using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenericMessagingService.Services.Templating.Database
{
    public interface IDatabaseService
    {
        Task<(string Template, string? Subject)?> GetTemplate(string templateName);
        Task<List<string>> GetTemplateNames();
    }
}
