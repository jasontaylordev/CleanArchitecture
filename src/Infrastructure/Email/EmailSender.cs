using CleanArchitecture.Application.Common.Interfaces;
using CleanArchitecture.Application.Common.Models;
using Mandrill.Model;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;

namespace CleanArchitecture.Infrastructure.Email
{
    public class EmailSender : IEmailSender
    {
        private readonly IConfiguration configuration;
        public EmailSender(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        /// <summary>
        /// A generic send email method that can accept all standards fields for sending email
        /// </summary>
        /// <param name="subject"></param>
        /// <param name="htmlBody"></param>
        /// <param name="toEmail"></param>
        /// <param name="ccEmail"></param>
        /// <param name="bccEmail"></param>
        /// <param name="attachmentModels"></param>
        /// <param name="isPreserveRecipients"></param>
        /// <returns></returns>
        public string SendEmail(string subject, string htmlBody, List<string> toEmail, List<string> ccEmail = null, List<string> bccEmail = null, List<AttachmentModel> attachmentModels = null, bool isPreserveRecipients = true, string fromEmail = "", string fromName = "")
        {
            List<MandrillAttachment> attachments = new List<MandrillAttachment>();
            if (attachmentModels != null)
            {
                foreach (var att in attachmentModels)
                {
                    attachments.Add(new MandrillAttachment
                    {
                        Content = att.File,
                        Name = att.Name,
                        Type = att.ContentType
                    });
                }
            }


            var sendTask = Mandrill.GetInstance(configuration).SendViaMandrill(subject, htmlBody, toEmail, ccEmail, bccEmail, attachments, isPreserveRecipients, fromEmail, fromName);
            sendTask.Wait();
            return sendTask.Result;
        }
    }
}
