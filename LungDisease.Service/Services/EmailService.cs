using Inventra.Service_Abstraction;
using Microsoft.Extensions.Options;
using MimeKit;
using MailKit.Net.Smtp;
using static MailKit.Telemetry;
using LungDisease.Shared.Common_Result;
using LungDisease.Service.EmailSettings;

namespace LungDisease.Service.Service
{
    public class EmailService : IEmailService
    {
        private readonly IOptions<EmailSetting> _settings;

        public EmailService(IOptions<EmailSetting> setting)
        {
            _settings = setting;
        }

        public async Task<Result> SendAsync(string to, string subject, string body)
        {
            using var smtp = new MailKit.Net.Smtp.SmtpClient();

            try
            {
                await smtp.ConnectAsync(
                    _settings.Value.Host,
                    _settings.Value.Port,
                    MailKit.Security.SecureSocketOptions.StartTls
                );

                await smtp.AuthenticateAsync(
                    _settings.Value.UserName,
                    _settings.Value.Password
                );

                // delay بسيط يقلل الضغط (اختياري)
                await Task.Delay(500);

                var email = new MimeMessage();
                email.From.Add(MailboxAddress.Parse(_settings.Value.FromEmail));
                email.To.Add(MailboxAddress.Parse(to));
                email.Subject = subject;
                email.Body = new TextPart("html") { Text = body };

                await smtp.SendAsync(email);

                await smtp.DisconnectAsync(true); // مهم جدًا

                return Result.Ok();
            }
            catch (Exception ex)
            {
                return Result.Fail(Error.Failure("Email.Failure", ex.Message));
            }
        }
    }

    
}
