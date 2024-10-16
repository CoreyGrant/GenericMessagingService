using GenericMessagingService.Services.Templating.Services.Formatting;
using GenericMessagingService.Services.Templating.Services.Location;
using GenericMessagingService.Types.Config;
using GenericMessagingService.Types.Template;
using System.Threading.Tasks;

namespace GenericMessagingService.Services.Templating.Services
{
    public interface ITemplateService
    {
        Task<(string Body, string? Subject)?> GetTemplate(TemplateRequest request);
    }

    public class TemplateService : ITemplateService
    {
        private readonly TemplateSettings templateSettings;
        private readonly ITemplateFormattingService templateFormattingService;
        private readonly ITemplateLocationService templateLocationService;

        public TemplateService(
            TemplateSettings templateSettings,
            ITemplateFormattingServiceResolver templateFormattingServiceResolver,
            ITemplateLocationServiceResolver templateLocationServiceResolver)
        {
            this.templateSettings = templateSettings;
            this.templateFormattingService = templateFormattingServiceResolver.Resolve(templateSettings.Formatting);
            this.templateLocationService = templateLocationServiceResolver.Resolve(templateSettings.Location);
        }

        public async Task<(string Body, string? Subject)?> GetTemplate(TemplateRequest request)
        {
            var (template, subject) = await templateLocationService.LocateTemplateAsync(request.TemplateName);
            if(template == null)
            {
                return null;
            }
            var body = await templateFormattingService.FormatTemplate(template, request.Data);
            subject = await templateFormattingService.FormatTemplate(subject, request.Data);
            return (body, subject);
        }
    }

    public interface ITemplateServiceFactory
    {
        ITemplateService CreateTemplateService(TemplateSettings templateSettings);
    }

    public class TemplateServiceFactory : ITemplateServiceFactory
    {
        private readonly ITemplateLocationServiceResolver templateLocationServiceResolver;
        private readonly ITemplateFormattingServiceResolver templateFormattingServiceResolver;

        public TemplateServiceFactory(ITemplateLocationServiceResolver templateLocationServiceResolver,
            ITemplateFormattingServiceResolver templateFormattingServiceResolver)
        {
            this.templateLocationServiceResolver = templateLocationServiceResolver;
            this.templateFormattingServiceResolver = templateFormattingServiceResolver;
        }
        public ITemplateService CreateTemplateService(TemplateSettings templateSettings)
        {
            return new TemplateService(templateSettings, templateFormattingServiceResolver, templateLocationServiceResolver);
        }
    }
}
