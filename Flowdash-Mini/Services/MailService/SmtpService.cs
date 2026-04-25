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
        private readonly IConfiguration _config;
        public SmtpService(IUnitOfWork unitOfWork, IConfiguration config)
        {
            _unitOfWork = unitOfWork;
            _config = config;
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
                    Text = $@"
                            <!DOCTYPE html>
                            <html>
                            <body style='font-family: Roboto,Raleway,Arial,Helvetica,sans-serif; background:#f5f5f5; padding:20px;'>
                              <div style='max-width:600px; margin:auto; background:#fff; padding:30px; border-radius:8px;'>
    
                                <div style='text-align:center;'>
                                  <img src='{_config["WebDomain"]}images/logo.png'
                                       alt='Flowdash'
                                       style='width:60px; margin-bottom:10px;' />
                                  <h1 style='margin:0; color:#333;'>Flowdash</h1>
                                </div>

                                <hr style='margin:20px 0;' />
                                {body}
                                <hr style='margin:20px 0;' />
                                <table width='100%' cellpadding='0' cellspacing='0' style='font-family: Roboto,Raleway,Arial,Helvetica,sans-serif;'>
                                  <tr>
                                    <td align='left' style='color:#555; font-size:14px;'>
                                      Flowdash Team
                                    </td>
                                    <td align='right' style='font-size:14px;'>
                                      <a href='{_config["WebDomain"]}'
                                         style='color:#4f46e5; text-decoration:none;'>
                                        Home
                                      </a>
                                    </td>
                                  </tr>
                                </table>
                                <div style='text-align:center;color:#a5a5a5;'>
                                <span style='color:#a5a5a5;'>Flowdash - All rights are reserved © 2025</span>
                                </div>
                              </div>
                            </body>
                            </html>",
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
