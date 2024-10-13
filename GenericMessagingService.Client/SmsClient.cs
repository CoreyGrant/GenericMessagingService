using GenericMessagingService.Types.Sms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenericMessagingService.Client
{
    public interface ISmsClient : IBaseClient
    {
        Task SendSms(SmsRequest smsRequest);
    }

    internal class SmsClient : BaseClient, ISmsClient
    {
        public SmsClient(ClientSettings settings) : base(settings)
        {
        }

        public async Task SendSms(SmsRequest smsRequest)
        {
            await Post("/sms/", smsRequest);
        }
    }
}
