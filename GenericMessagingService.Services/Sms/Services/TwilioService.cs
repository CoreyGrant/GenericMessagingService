using GenericMessagingService.Types.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Twilio;
using Twilio.Rest.Api.V2010.Account;

namespace GenericMessagingService.Services.Sms.Services
{
    internal class TwilioService : ISmsService
    {
        private readonly TwilioSettings settings;

        public TwilioService(TwilioSettings settings)
        {
            this.settings = settings;
            TwilioClient.Init(settings.AccountSid, settings.AuthToken);
        }

        public async Task SendSms(string message, IEnumerable<string> to, string from)
        {
            foreach (var toNumber in to)
            {
                await MessageResource.CreateAsync(
                    new Twilio.Types.PhoneNumber(toNumber),
                    from: new Twilio.Types.PhoneNumber(from),
                    body: message);
            }
        }

        public async Task SendSms(Dictionary<string, string> toMessages, string from)
        {
            foreach (var (toNumber, message) in toMessages) 
            {
                await MessageResource.CreateAsync(
                    new Twilio.Types.PhoneNumber(toNumber),
                    from: new Twilio.Types.PhoneNumber(from),
                    body: message);
            }
        }
    }
}
