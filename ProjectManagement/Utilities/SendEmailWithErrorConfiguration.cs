using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectManagement.Utilities
{
    public class SendEmailWithErrorConfiguration: ISendEmailWithErrorConfiguration
    {
        public bool AllowSend { get; set; }
        public string ToEmails { get; set; }
    }
}
