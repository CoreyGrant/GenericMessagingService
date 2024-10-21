using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Net;

namespace GenericMessagingService.WebApi.Filters
{
    public class ApiKeyFilterAttribute : Attribute, IAuthorizationFilter
    {
        private const string XApiKey = "X-API-KEY";
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var requestServices = context.HttpContext.RequestServices;
            var config = requestServices.GetRequiredService<GenericMessagingServiceSettings>();
            var expectedHeader = config.ApiKey;
            var headers = context.HttpContext.Request.Headers;
            var apiKeyHeader = headers.ContainsKey(XApiKey) ? headers.GetCommaSeparatedValues(XApiKey).SingleOrDefault() : null;
            if(apiKeyHeader == null)
            {
                context.Result = new BadRequestResult();
            }
            if(apiKeyHeader != expectedHeader)
            {
                context.Result = new UnauthorizedResult();
            }
        }
    }
}
