using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectManagement.Utilities
{
    public class EmailMessage
    {
        public EmailMessage()
        {
            ToAddresses = new List<EmailAddress>();
        }

        public List<EmailAddress> ToAddresses { get; set; }
        public List<EmailAddress> CcAddresses { get; set; }
        public string Subject { get; set; }
        public string Content { get; set; }
    }

    public enum EmailTemplate
    {
        RequestedTemplate = 0,
        SubmittedTemplate = 1,
        ApprovedTemplate = 2,
        RejectedTemplate = 3,
        RevokedTemplate = 4
    }
}
