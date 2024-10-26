using GenericMessagingService.Types.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenericMessagingService.Services.Templating.Services
{
    public interface ITemplateStrategyService
    {
        string? TemplateStrategy { get; set; }
    }

    [InjectScoped]
    public class TemplateStrategyService : ITemplateStrategyService
    {
        public string? TemplateStrategy { get; set; }
    }
}
