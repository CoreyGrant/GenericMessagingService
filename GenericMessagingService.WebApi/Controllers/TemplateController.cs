using GenericMessagingService.Services.Templating.Services;
using GenericMessagingService.Types.Shared;
using GenericMessagingService.Types.Template;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GenericMessagingService.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TemplateController : ControllerBase
    {
        private readonly ITemplateService templateService;

        public TemplateController(ITemplateService templateService)
        {
            this.templateService = templateService;
        }

        [HttpPost("/")]
        public async Task<ApiResponse<TemplateResponse>> GetTemplate(TemplateRequest request)
        {
            try
            {
                var result = await this.templateService.GetTemplate(request);
                if (result == null) { return new ApiResponse<TemplateResponse>("Failed to find template"); }
                return new ApiResponse<TemplateResponse>(result);
            }
            catch (Exception ex)
            {
                return new ApiResponse<TemplateResponse>(ex.Message);
            }
        }
    }
}
