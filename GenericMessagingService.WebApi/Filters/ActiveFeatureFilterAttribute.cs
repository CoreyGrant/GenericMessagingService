using GenericMessagingService.Types.Config;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Net;

namespace GenericMessagingService.WebApi.Filters
{
    public class ActiveFeatureFilterAttribute : Attribute, IAuthorizationFilter
    {
        private readonly string name;

        public ActiveFeatureFilterAttribute(string name) 
        {
            this.name = name;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var requestServices = context.HttpContext.RequestServices;
            var config = requestServices.GetRequiredService<AppSettings>();
            if((name == "email" && config.Email == null) 
                || (name == "template" && config.Template == null && config.ComboTemplate == null)
                || (name == "sms" && config.Sms == null))
            {
                context.Result = new BadRequestResult();
            }
        }
    }
}
