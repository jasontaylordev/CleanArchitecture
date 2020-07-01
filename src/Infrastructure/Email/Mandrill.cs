using Mandrill;
using Mandrill.Model;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CleanArchitecture.Application.Common.Helpers;

namespace CleanArchitecture.Infrastructure.Email
{
    class Mandrill
    {
        private static readonly string MANDRILL_KEY = "ServiceKeys:Mandrill";
        private static volatile Mandrill instance;
        private static readonly object syncRoot = new object();
        private readonly string APIKey;
        private readonly IConfiguration configuration;
        public static Mandrill GetInstance(IConfiguration configuration)
        {
            if (instance == null)
            {
                lock (syncRoot)
                {
                    if (instance == null)
                    {
                        instance = new Mandrill(configuration);
                    }
                }
            }

            return instance;
        }

        private Mandrill(IConfiguration configuration)
        {
            this.configuration = configuration;
            APIKey = configuration[MANDRILL_KEY];
        }


        public async Task<string> SendViaMandrill(string subject, string htmlBody, List<string> toEmail, List<string> ccEmail = null, List<string> bccEmail = null, List<MandrillAttachment> attachments = null, bool isPreserve = true, string fromEmail = "", string fromName = "")
        {
            var api = new MandrillApi(APIKey);

            string defaultEmail = configuration["EmailSettings:FromEmail"];
            string defaultName = configuration["EmailSettings:Name"];

            var message = new MandrillMessage
            {
                FromEmail = fromEmail.ConvertNullOrEmptyTo(defaultEmail),
                FromName = fromName.ConvertNullOrEmptyTo(defaultName)
            };

            message.Html = htmlBody;
            message.Subject = subject;
            var recipients = new List<MandrillMailAddress>();

            foreach (var email in toEmail)
            {
                if (!string.IsNullOrEmpty(email))
                {
                    var to = new MandrillMailAddress
                    {
                        Type = MandrillMailAddressType.To,
                        Email = email
                    };
                    recipients.Add(to);
                }
            }

            if (ccEmail != null && ccEmail.Any())
            {
                foreach (var e in ccEmail)
                {
                    if (!string.IsNullOrEmpty(e))
                    {
                        var cc = new MandrillMailAddress
                        {
                            Type = MandrillMailAddressType.Cc,
                            Email = e
                        };
                        recipients.Add(cc);
                    }
                }
            }

            if (bccEmail != null && bccEmail.Any())
            {
                foreach (var bcc in bccEmail)
                {
                    if (!string.IsNullOrEmpty(bcc))
                    {
                        recipients.Add(new MandrillMailAddress
                        {
                            Type = MandrillMailAddressType.Bcc,
                            Email = bcc
                        });
                    }
                }
            }

            message.PreserveRecipients = isPreserve;
            message.To = recipients;

            if (attachments != null && attachments.Any())
            {
                message.Attachments = attachments;
            }

            var result = await api.Messages.SendAsync(message);
            var statuses = (result != null && result.Any()) ? result.Select(x => x.Status).ToList() : null;
            return statuses != null ? string.Join(',', statuses) : "Unknown";
        }
    }
}
