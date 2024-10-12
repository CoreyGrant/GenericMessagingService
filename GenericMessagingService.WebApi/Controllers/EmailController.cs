using GenericMessagingService.Services.Email;
using GenericMessagingService.Services.Templating;
using GenericMessagingService.Types.Email;
using GenericMessagingService.Types.Shared;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GenericMessagingService.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmailController : ControllerBase
    {
        private readonly IEmailSenderService emailSenderService;
        private readonly ITemplateService templateService;

        public EmailController(
            IEmailSenderService emailSenderService,
            ITemplateService templateService)
        {
            this.emailSenderService = emailSenderService;
            this.templateService = templateService;
        }

        [HttpPost("Send")]
        public async Task<ApiResponse> SendEmail<T>(EmailRequest<T> emailRequest) where T : class
        {
            try
            {
                await this.emailSenderService.SendEmailAsync(emailRequest);
                return new ApiResponse(true);
            }
            catch (Exception ex)
            {
                return new ApiResponse(ex.Message);
            }
        }

        [HttpGet("Template")]
        public async Task<ApiResponse<TemplateResponse>> GetTemplate<T>(TemplateRequest request) where T : class
        {
            try
            {
                var result = await this.templateService.GetTemplate(request);
                if (result == null) { return new ApiResponse<TemplateResponse>("Failed to find template"); }
                return new ApiResponse<TemplateResponse>(result);
            } 
            catch(Exception ex)
            {
                return new ApiResponse<TemplateResponse>(ex.Message);
            }
        }
    }
}
