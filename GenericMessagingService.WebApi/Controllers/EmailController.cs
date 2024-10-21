using GenericMessagingService.Services.Email;
using GenericMessagingService.Services.Templating;
using GenericMessagingService.Types.Email;
using GenericMessagingService.Types.Shared;
using GenericMessagingService.WebApi.Filters;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GenericMessagingService.WebApi.Controllers
{
    [ApiKeyFilter]
    [ActiveFeatureFilter("email")]
    [Route("api/[controller]")]
    [ApiController]
    public class EmailController : ControllerBase
    {
        private readonly IEmailSenderService emailSenderService;

        public EmailController(
            IEmailSenderService emailSenderService)
        {
            this.emailSenderService = emailSenderService;
        }

        [HttpPost]
        public async Task<ApiResponse> SendEmail(EmailRequest emailRequest)
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
    }
}
