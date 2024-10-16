﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenericMessagingService.Services.Templating.Services.Location
{
    internal interface ITemplateLocationService
    {
        Task<(string?, string?)> LocateTemplateAsync(string templateName);
    }
}