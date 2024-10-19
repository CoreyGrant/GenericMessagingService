using GenericMessagingService.Services.Cache;
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
        private readonly ICacheManager cacheManager;
        private readonly ITemplateFormattingService templateFormattingService;
        private readonly ITemplateLocationService templateLocationService;

        public TemplateService(
            TemplateSettings templateSettings,
            ITemplateFormattingServiceResolver templateFormattingServiceResolver,
            ITemplateLocationServiceResolver templateLocationServiceResolver,
            ICacheManager cacheManager)
        {
            this.templateSettings = templateSettings;
            this.cacheManager = cacheManager;
            this.templateFormattingService = templateFormattingServiceResolver.Resolve(templateSettings);
            this.templateLocationService = templateLocationServiceResolver.Resolve(templateSettings.Location);
        }

        public async Task<(string Body, string? Subject)?> GetTemplate(TemplateRequest request)
        {
            if (templateSettings.Cache)
            {
                var cachedBody = await cacheManager.Get<string>("template-body", request.TemplateName);
                var cachedSubject = await cacheManager.Get<string>("template-subject", request.TemplateName);
                if (!string.IsNullOrEmpty(cachedBody))
                {
                    return (cachedBody, cachedSubject);
                }
            }
            var (template, subject) = await templateLocationService.LocateTemplateAsync(request.TemplateName);
            if(template == null)
            {
                return null;
            }
            if (templateSettings.Cache)
            {
                await cacheManager.Set("template-body", request.TemplateName, template);
                await cacheManager.Set("template-subject", request.TemplateName, subject);
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
        private readonly ICacheManager cacheManager;

        public TemplateServiceFactory(ITemplateLocationServiceResolver templateLocationServiceResolver,
            ITemplateFormattingServiceResolver templateFormattingServiceResolver, ICacheManager cacheManager)
        {
            this.templateLocationServiceResolver = templateLocationServiceResolver;
            this.templateFormattingServiceResolver = templateFormattingServiceResolver;
            this.cacheManager = cacheManager;
        }
        public ITemplateService CreateTemplateService(TemplateSettings templateSettings)
        {
            return new TemplateService(templateSettings, templateFormattingServiceResolver, templateLocationServiceResolver, cacheManager);
        }
    }
}
