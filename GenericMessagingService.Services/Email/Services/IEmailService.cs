﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenericMessagingService.Services.Email.Services
{
    internal interface IEmailService
    {
        Task SendEmailAsync(string subject, string body, IEnumerable<string> to, string from);
        Task SendEmailAsync(Dictionary<string, (string, string)> toSubjectBody, string from);
    }
}
