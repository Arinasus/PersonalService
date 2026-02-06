using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace WebApplication2.Services
{
    public class EmailService { 
        private readonly string _smtpEmail; 
        private readonly string _smtpPassword; 
        private readonly string _smtpHost; 
        private readonly int _smtpPort; 
        public EmailService() { 
            _smtpEmail = Environment.GetEnvironmentVariable("SMTP_EMAIL"); 
            _smtpPassword = Environment.GetEnvironmentVariable("SMTP_PASSWORD");
            _smtpHost = Environment.GetEnvironmentVariable("SMTP_HOST") ?? "smtp.gmail.com"; 
            _smtpPort = int.TryParse(Environment.GetEnvironmentVariable("SMTP_PORT"), out var port) ? port : 587; 
        } 
        public async Task SendEmailAsync(string to, string subject, string body) { 
            if (string.IsNullOrEmpty(_smtpEmail) || string.IsNullOrEmpty(_smtpPassword)) throw new InvalidOperationException("SMTP credentials are not configured."); 
            using (var smtp = new SmtpClient(_smtpHost, _smtpPort)) { 
                smtp.Credentials = new NetworkCredential(_smtpEmail, _smtpPassword); 
                smtp.EnableSsl = true; var message = new MailMessage(_smtpEmail, to, subject, body) { 
                    IsBodyHtml = true }; 
                await smtp.SendMailAsync(message); 
            } 
        } 
    }
}
