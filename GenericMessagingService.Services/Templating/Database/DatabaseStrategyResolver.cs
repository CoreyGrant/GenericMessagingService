﻿using GenericMessagingService.Types.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenericMessagingService.Services.Templating.Database
{
    public interface IDatabaseStrategyResolver
    {
        IDatabaseService Resolve(string? type);
    }

    /// <summary>
    /// Different databases can be configured
    /// </summary>
    public class DatabaseStrategyResolver : IDatabaseStrategyResolver
    {
        private readonly DatabaseTemplateLocationSettings settings;

        public DatabaseStrategyResolver(DatabaseTemplateLocationSettings settings) 
        {
            this.settings = settings;
        }

        public IDatabaseService Resolve(string? type)
        {
            if(!string.IsNullOrEmpty(type) || type.ToLower() == "sqlserver")
            {
                return new SqlServerService(settings);
            }
            throw new Exception("Could not resolve database of type: " + type);
        }


    }
}
