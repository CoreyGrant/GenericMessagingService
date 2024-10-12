using GenericMessagingService.Types.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenericMessagingService.Services.Templating.Database
{
    internal interface IDatabaseStrategyResolver
    {
        IDatabaseService Resolve(string? type);
    }

    /// <summary>
    /// Different databases can be configured
    /// </summary>
    internal class DatabaseStrategyResolver : IDatabaseStrategyResolver
    {
        private readonly EmailSettings emailSettings;

        public DatabaseStrategyResolver(EmailSettings emailSettings) 
        {
            this.emailSettings = emailSettings;
        }

        public IDatabaseService Resolve(string? type)
        {
            if(!string.IsNullOrEmpty(type) || type.ToLower() == "sqlserver")
            {
                return new SqlServerService(emailSettings);
            }
            throw new Exception("Could not resolve database of type: " + type);
        }


    }
}
