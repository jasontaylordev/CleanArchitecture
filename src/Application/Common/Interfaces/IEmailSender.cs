using CleanArchitecture.Application.Common.Models;
using System.Collections.Generic;

namespace CleanArchitecture.Application.Common.Interfaces
{
    public interface IEmailSender
    {
        string SendEmail(string subject, string emailBody, List<string> toEmail, List<string> ccEmail = null, List<string> bccEmail = null, List<AttachmentModel> attachmentModels = null, bool isPreserveRecipients = true, string fromEmail = "", string fromName = "");
    }
}
