using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenericMessagingService.Client.Interfaces
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

}
