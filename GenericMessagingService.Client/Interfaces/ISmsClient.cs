using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenericMessagingService.Client.Interfaces
{
    public interface ISmsClient : IBaseClient
    {
        Task SendPlainSms(
            string to,
            string message,
            string? from = null);
        Task SendPlainSms(
            IList<string> to,
            string message,
            string? from = null);
        Task SendTemplateSms(
            string to,
            string templateName,
            IDictionary<string, string> data,
            string? from = null);
        Task SendTemplateSms<T>(string to, string templateName, T data, string? from = null) where T : class;
        Task SendTemplateSms(
            IList<string> to,
            string templateName,
            IDictionary<string, string> data,
            string? from = null);
        Task SendTemplateSms<T>(
            IList<string> to,
            string templateName,
            T data,
            string? from = null) where T : class;
        Task SendTemplateSms(
            IList<string> to,
            string templateName,
            IDictionary<string, IDictionary<string, string>> toData,
            string? from = null);
        Task SendTemplateSms<T>(
            IList<string> to,
            string templateName,
            IDictionary<string, T> data,
            string? from = null) where T : class;
    }
}
