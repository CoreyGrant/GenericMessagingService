using GenericMessagingService.Client.Utils;
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
        /// <summary>
        /// Send a plain email to a single address
        /// </summary>
        /// <param name="to">The email address</param>
        /// <param name="body">The email body</param>
        /// <param name="subject">The email subject</param>
        /// <param name="from">The from address</param>
        /// <returns></returns>
        Task SendPlainEmail(string to, string body, string subject, string? from = null);
        /// <summary>
        /// Send a plain email to multiple email addresses
        /// </summary>
        /// <param name="to">The email addresses</param>
        /// <param name="body">The email body</param>
        /// <param name="subject">The email subject</param>
        /// <param name="from">The from address</param>
        /// <returns></returns>
        Task SendPlainEmail(IList<string> to, string body, string subject, string? from = null);
        /// <summary>
        /// Send a templated email to a single address
        /// </summary>
        /// <param name="to">The email address</param>
        /// <param name="templateName">The name of the template for the email body</param>
        /// <param name="data">The data for templating</param>
        /// <param name="subject">The email subject</param>
        /// <param name="from">The from address</param>
        /// <returns></returns>
        Task SendTemplateEmail(string to, string templateName, IDictionary<string, string> data, string? subject = null, string? from = null);
        /// <summary>
        /// Send a templated email to a single address
        /// </summary>
        /// <typeparam name="T">The data type, which will be dictionary converted</typeparam>
        /// <param name="to">The email address</param>
        /// <param name="templateName">The name of the teplate for the email body</param>
        /// <param name="data">The data for templating</param>
        /// <param name="subject">The email subject</param>
        /// <param name="from">The from address</param>
        /// <returns></returns>
        Task SendTemplateEmail<T>(string to, string templateName, T data, string? subject, string? from) where T : class;
        Task SendTemplateEmail(IList<string> to, string templateName, IDictionary<string, string> data, string? subject = null, string? from = null);
        Task SendTemplateEmail<T>(IList<string> to, string templateName, T data, string? subject = null, string? from = null) where T : class;
        Task SendTemplateEmail(IList<string> to, string templateName, IDictionary<string, IDictionary<string, string>> toData, string? subject = null, string? from = null);
        Task SendTemplateEmail<T>(IList<string> to, string templateName, IDictionary<string, T> data, string? subject = null, string? from = null) where T : class;
    }

    internal class EmailClient : BaseClient, IEmailClient
    {
        public EmailClient(ClientSettings settings, IClassToDictionaryConverter converter) : base(settings, converter){}

        public async Task SendPlainEmail(
            string to,
            string body, 
            string subject, 
            string? from = null)
        {
            await SendEmail(new EmailRequest
            {
                To = new[] { to },
                Template = body,
                Subject = subject,
                From = from,
            });
        }

        public async Task SendPlainEmail(
            IList<string> to,
            string body,
            string subject,
            string? from = null)
        {
            await SendEmail(new EmailRequest
            {
                To = to,
                Template = body,
                Subject = subject,
                From = from
            });
        }

        public async Task SendTemplateEmail(
            string to,
            string templateName,
            IDictionary<string, string> data,
            string? subject = null,
            string? from = null)
        {
            await SendEmail(new EmailRequest
            {
                To = new[] { to },
                TemplateName = templateName,
                Subject = subject,
                Data = data,
                From = from,
            });
        }

        public async Task SendTemplateEmail<T>(
            string to,
            string templateName,
            T data,
            string? subject,
            string? from) where T : class
        {
            await SendTemplateEmail(to, templateName, converter.Convert<T>(data), subject, from);
        }

        public async Task SendTemplateEmail(
            IList<string> to,
            string templateName,
            IDictionary<string, string> data,
            string? subject = null,
            string? from = null)
        {
            await SendEmail(new EmailRequest
            {
                To = to,
                TemplateName = templateName,
                Data = data,
                Subject = subject,
                From = from
            });
        }

        public async Task SendTemplateEmail<T>(
            IList<string> to,
            string templateName,
            T data,
            string? subject = null,
            string? from = null) where T : class
        {
            await SendTemplateEmail(to, templateName, converter.Convert(data), subject, from);
        }

        public async Task SendTemplateEmail(
            IList<string> to,
            string templateName,
            IDictionary<string, IDictionary<string, string>> toData,
            string? subject = null,
            string? from = null)
        {
            await SendEmail(new EmailRequest
            {
                To = to,
                TemplateName = templateName,
                ToData = toData,
                Subject = subject,
                From = from
            });
        }

        public async Task SendTemplateEmail<T>(
            IList<string> to,
            string templateName,
            IDictionary<string, T> data,
            string? subject = null,
            string? from = null) where T: class
        {
            await SendTemplateEmail(to, templateName, data.ToDictionary(x => x.Key, x => converter.Convert(x.Value)), subject, from);
        }

        private async Task SendEmail(EmailRequest emailRequest)
        {
            await Post("/email/", emailRequest);
        }
    }
}
