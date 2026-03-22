using Microsoft.AspNetCore.Http;

namespace Application.Dtos.RequestDto
{
    public class MailRequest
    {
        public string To { get; set; } = default!;
        public string SenderName { get; set; } = default!;
        public string From { get; set; } = default!;
        public string ReceiverName { get; set; } = default!;
        public string Subject { get; set; } = default!;
        public string Body { get; set; } = default!;
        public List<IFormFile> Attachments { get; set; } = new();
    }
}
