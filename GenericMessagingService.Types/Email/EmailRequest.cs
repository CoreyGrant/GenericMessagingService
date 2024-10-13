﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenericMessagingService.Types.Email
{
    public class EmailRequest
    {
        public string? Template { get; set; }
        public string? Subject { get; set; }
        public string? TemplateName { get; set; }
        public List<string> To { get; set; }
        public string From { get; set; }
        public Dictionary<string, string> Data { get; set; }
    }
}
