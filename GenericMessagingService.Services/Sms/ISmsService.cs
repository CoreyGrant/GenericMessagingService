﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenericMessagingService.Services.Sms
{
    public interface ISmsService
    {
        Task SendSms(string message, IEnumerable<string> to, string from);
    }
}
