using GenericMessagingService.Types.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Twilio;
using Twilio.Rest.Api.V2010.Account;

namespace GenericMessagingService.Services.Sms
{
    internal class TwilioService : ISmsService
    {
        private readonly TwilioSettings settings;

        public TwilioService(TwilioSettings settings)
        {
            this.settings = settings;
        }

        public async Task SendSms(string message, IEnumerable<string> to, string from)
        {
            TwilioClient.Init(settings.AccountSid, settings.AuthToken);
            foreach(var toNumber in to) 
            {
                await MessageResource.CreateAsync(
                    new Twilio.Types.PhoneNumber(toNumber),
                    from: new Twilio.Types.PhoneNumber(from),
                    body: message);
            }
        }
    }
}
