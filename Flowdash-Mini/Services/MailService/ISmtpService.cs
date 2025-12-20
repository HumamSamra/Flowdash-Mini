using Flowdash_Mini.Models;

namespace Flowdash_Mini.Services.MailService
{
    public interface ISmtpService
    {
        SmtpResult SendMail(string subject, string body, List<string> to);
    }
}
