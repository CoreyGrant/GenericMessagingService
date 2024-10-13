using GenericMessagingService.Services.Sms;
using GenericMessagingService.Services.Templating;
using GenericMessagingService.Types.Email;
using GenericMessagingService.Types.Shared;
using GenericMessagingService.Types.Sms;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GenericMessagingService.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SmsController : ControllerBase
    {
        private readonly ISmsSenderService smsSenderService;

        public SmsController(ISmsSenderService smsSenderService)
        {
            this.smsSenderService = smsSenderService;
        }

        [HttpPost("/")]
        public async Task<ApiResponse> SendEmail(SmsRequest smsRequest)
        {
            try
            {
                await this.smsSenderService.SendSmsAsync(smsRequest);
                return new ApiResponse(true);
            }
            catch (Exception ex)
            {
                return new ApiResponse(ex.Message);
            }
        }
    }
}
