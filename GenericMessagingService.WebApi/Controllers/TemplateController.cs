using GenericMessagingService.Services.Templating.Services;
using GenericMessagingService.Types.Shared;
using GenericMessagingService.Types.Template;
using GenericMessagingService.WebApi.Filters;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GenericMessagingService.WebApi.Controllers
{
    [ApiKeyFilter]
    [ActiveFeatureFilter("template")]
    [Route("api/[controller]")]
    [ApiController]
    public class TemplateController : ControllerBase
    {
        private readonly ITemplateRunnerService templateService;

        public TemplateController(ITemplateRunnerService templateService)
        {
            this.templateService = templateService;
        }

        [HttpPost]
        public async Task<ApiResponse<TemplateResponse>> GetTemplate(TemplateRequest request)
        {
            try
            {
                var result = await this.templateService.RunTemplate(request.TemplateName, request.Data);
                if (result == null) { return new ApiResponse<TemplateResponse>("Failed to find template"); }
                return new ApiResponse<TemplateResponse>(new TemplateResponse { Body = result.Value.Body, Subject = result.Value.Subject});
            }
            catch (Exception ex)
            {
                return new ApiResponse<TemplateResponse>(ex.Message);
            }
        }

        [HttpGet]
        public async Task<ApiResponse<List<string>>> GetTemplateNames()
        {

        }
    }
}
