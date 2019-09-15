using MailKit.Net.Smtp;
using MimeKit;
using MimeKit.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectManagement.Utilities
{
    public class EmailService: IEmailService
    {

        private readonly IEmailConfiguration _emailConfiguration;
        public EmailService(IEmailConfiguration emailConfiguration)
        {
            _emailConfiguration = emailConfiguration;
        }

        
        public async Task Send(EmailMessage emailMessage)
        {
            var message = new MimeMessage();

            // Prepare the email object settings [to, cc, from]
            message.To.AddRange(emailMessage.ToAddresses.Select(x => new MailboxAddress(x.Name, x.Address)));

            if (emailMessage.CcAddresses != null && emailMessage.CcAddresses.Count > 0)
            {
                message.Cc.AddRange(emailMessage.CcAddresses.Select(x => new MailboxAddress(x.Name, x.Address)));
            }

            message.From.Add(new MailboxAddress(_emailConfiguration.FromEmail, _emailConfiguration.FromEmail));

            // Prepare email subject
            message.Subject = emailMessage.Subject;

            // Prepare email content
            message.Body = new TextPart(TextFormat.Html)
            {
                Text = emailMessage.Content
            };

            // Authenticate then send email
            using (var emailClient = new SmtpClient())
            {
                // Connect to server with account credentials and ssl settings
                emailClient.Connect(_emailConfiguration.SmtpServer, _emailConfiguration.SmtpPort, _emailConfiguration.Ssl);

                //Remove any OAuth functionality as we won't be using it. 
                emailClient.AuthenticationMechanisms.Remove("XOAUTH2");

                // Authenticate the connection to server
                emailClient.Authenticate(_emailConfiguration.SmtpUsername, _emailConfiguration.SmtpPassword);

                // Send email
                await emailClient.SendAsync(message);

                // Disconnect the object
                emailClient.Disconnect(true);
            }
        }
    }
}
