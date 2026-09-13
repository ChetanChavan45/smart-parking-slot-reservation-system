using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Options;
using PametniParkingSistem.Services.Interfaces;
using PametniParkingSistem.Settings;

namespace PametniParkingSistem.Services
{
    public class EmailSenderService : IEmailSenderService
    {
        private readonly EmailSettings _settings;

        public EmailSenderService(IOptions<EmailSettings> settings)
        {
            _settings = settings.Value;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            var message = new MailMessage
            {
                From = new MailAddress(_settings.SenderEmail, _settings.SenderName),
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            };

            message.To.Add(toEmail);

            using var client = new SmtpClient(_settings.SmtpServer, _settings.Port)
            {
                Credentials = new NetworkCredential(_settings.SenderEmail, _settings.Password),
                EnableSsl = true
            };

            try
            {
                await client.SendMailAsync(message);
            }
            catch (Exception ex)
            {
                Console.WriteLine("EMAIL GRESKA:");
                Console.WriteLine(ex.ToString());

                if (ex.InnerException != null)
                {
                    Console.WriteLine("INNER:");
                    Console.WriteLine(ex.InnerException.ToString());
                }

                throw;
            }
        }
    }
}