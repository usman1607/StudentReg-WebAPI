using Application.Dtos.Common;
using Application.Dtos.RequestDto;
using Application.Services.Contracts;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace Infrastructure.Services
{
    public class MailService : IMailService
    {
        private readonly ILogger<MailService> _logger;
        private readonly MailConfig _mailConfig;
        

        public MailService(ILogger<MailService> logger, IOptions<MailConfig> mailConfig)
        {
            _logger = logger;
            _mailConfig = mailConfig.Value;
        }

        public async Task<Response> SendEmailAsync(MailRequest request)
        {
            
            var apiKey = _mailConfig.ApiKey;
            var client = new SendGridClient(apiKey);
            var from = new EmailAddress(request.From, request.SenderName);
            var to = new EmailAddress(request.To, request.ReceiverName);
            var plainTextContent = request.Body;
            var htmlContent = $"<strong>{request.Body}</strong>";
            var msg = MailHelper.CreateSingleEmail(from, to, request.Subject, plainTextContent, htmlContent);

            var attachments = new List<Attachment>();
            foreach (var attach in request.Attachments)
            {
                if (attach != null && attach.Length > 0)
                {
                    string data = string.Empty;
                    var fileName = attach.ContentType.Split('/')[0];
                    string contentType = attach.ContentType.Split('/')[1];

                    using (var fs = new MemoryStream())
                    {
                        attach.CopyTo(fs);
                        var fileBytes = fs.ToArray();
                        data = Convert.ToBase64String(fileBytes);
                    }

                    var attachment = new Attachment()
                    {
                        Content = data,
                        Type = contentType,
                        Filename = $"{fileName}.{contentType}",
                        Disposition = "inline",
                        ContentId = "Banner"
                    };
                    attachments.Add(attachment);
                }
            }
            msg.AddAttachments(attachments);

            var response = await client.SendEmailAsync(msg);

            return response;
        }
    }
    
}
