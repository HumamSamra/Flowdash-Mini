using Flowdash_Mini.Classes;
using Flowdash_Mini.Enums;
using Flowdash_Mini.Models;
using Flowdash_Mini.Repositories;
using MailKit.Net.Smtp;
using MimeKit;

namespace Flowdash_Mini.Services.MailService
{
    public class SmtpService : ISmtpService
    {
        private readonly IUnitOfWork _unitOfWork;
        public SmtpService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public SmtpResult SendMail(string subject, string body, List<string> to)
        {
            try
            {
                var pwd = _unitOfWork.AppSettings.Get("SmtpPassword")!;
                if (pwd == null)
                {
                    throw new Exception("SMTP password is not set in the application settings.");
                }
                var password = Ciphering.Decrypt(pwd.Value);

                var email = _unitOfWork.AppSettings.Get(nameof(AppSettingKeys.SmtpEmail));
                var name = _unitOfWork.AppSettings.Get(nameof(AppSettingKeys.SmtpName));
                var host = _unitOfWork.AppSettings.Get(nameof(AppSettingKeys.SmtpHost));
                var port = _unitOfWork.AppSettings.Get(nameof(AppSettingKeys.SmtpPort));
                var ssl = _unitOfWork.AppSettings.Get(nameof(AppSettingKeys.SmtpSsl));

                if (email == null || name == null || host == null
                    || port == null || ssl == null)
                {
                    throw new Exception("SMTP information were not found.");
                }

                var message = new MimeMessage();
                message.From.Add(new MailboxAddress(name.Value, email.Value));
                message.To.AddRange(to.Select(e => new MailboxAddress("", e)));

                message.Subject = subject;
                message.Body = new TextPart("html")
                {
                    Text = body,
                };

                using (var client = new SmtpClient())
                {
                    client.Connect(host.Value, Convert.ToInt32(port.Value), Convert.ToBoolean(ssl.Value));
                    client.Authenticate(email.Value, password);

                    var msg = client.Send(message);
                    client.Disconnect(true);
                }
                return new SmtpResult("Email was sent successfully");
            }
            catch (Exception err)
            {
                return new SmtpResult(err.Message, false);
            }
        }
    }
}
