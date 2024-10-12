using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenericMessagingService.Services.Templating.Database
{
    internal interface IDatabaseService
    {
        Task<(string Template, string Subject)?> GetTemplate(string templateName);
    }
}
