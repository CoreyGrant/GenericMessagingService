using GenericMessagingService.Types.Email;
using GenericMessagingService.Types.Sms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenericMessagingService.Client
{
    public interface IEmailClient : IBaseClient
    {
        Task SendEmail(EmailRequest emailRequest);
    }

    internal class EmailClient : BaseClient, IEmailClient
    {
        public EmailClient(ClientSettings settings) : base(settings)
        {
        }

        public async Task SendEmail(EmailRequest emailRequest)
        {
            await Post("/email/", emailRequest);
        }
    }
}
