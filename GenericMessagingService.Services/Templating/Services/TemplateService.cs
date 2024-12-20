﻿using GenericMessagingService.Services.Cache;
using GenericMessagingService.Services.Templating.Services.Formatting;
using GenericMessagingService.Services.Templating.Services.Location;
using GenericMessagingService.Types.Attributes;
using GenericMessagingService.Types.Config;
using GenericMessagingService.Types.Template;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GenericMessagingService.Services.Templating.Services
{
    public interface ITemplateService
    {
        Task<(string Body, string? Subject)?> GetTemplate(TemplateRequest request);
        Task<List<string>> GetTemplateNames();
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
            var (template, subject) = ((string)null, (string)null);
            if (templateSettings.Cache)
            {
                var cachedBody = await cacheManager.Get<string>("template-body", request.TemplateName);
                var cachedSubject = await cacheManager.Get<string>("template-subject", request.TemplateName);
                if (!string.IsNullOrEmpty(cachedBody))
                {
                    template = (string)cachedBody;
                    subject = cachedSubject;
                }
            }
            (template, subject) = await templateLocationService.LocateTemplateAsync(request.TemplateName);
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
            if (subject != null)
            {
                subject = await templateFormattingService.FormatTemplate(subject, request.Data);
            }
            return (body, subject);
        }

        public async Task<List<string>> GetTemplateNames()
        {
            return await templateLocationService.GetTemplateNames();
        }
    }

    public interface ITemplateServiceFactory
    {
        ITemplateService CreateTemplateService(TemplateSettings templateSettings);
    }

    [InjectScoped(ServiceType.Template)]
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
