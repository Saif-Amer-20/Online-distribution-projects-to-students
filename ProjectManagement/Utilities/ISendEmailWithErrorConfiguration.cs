using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectManagement.Utilities
{
    public interface ISendEmailWithErrorConfiguration
    {
        bool AllowSend { get; set; }
        string ToEmails { get; set; }
    }
}
