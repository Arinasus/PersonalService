using System.Net;
using System.Net.Mail;

namespace WebApplication2.Services
{
    public class EmailService
    {
        public async Task SendConfirmationEmailAsync(string email, string token) {
            var confirmLink = $"https://localhost:7080/Auth/Confirm?token={token}"; string html = $@" <!DOCTYPE html> <html> <head> <meta charset='UTF-8' /> <style> body {{ font-family: Arial, sans-serif; background: #f5f5f5; padding: 20px; }} .container {{ background: #ffffff; padding: 24px; border-radius: 8px; max-width: 480px; margin: auto; border: 1px solid #e0e0e0; }} h2 {{ margin-top: 0; color: #333333; }} p {{ color: #555555; line-height: 1.5; }} .btn {{ display: inline-block; padding: 12px 20px; background: #86612F; color: #ffffff !important; text-decoration: none; border-radius: 6px; margin-top: 20px; }} .footer {{ margin-top: 20px; font-size: 12px; color: #888888; }} </style> </head> <body> <div class='container'> <h2>Подтверждение регистрации</h2> <p>Здравствуйте!</p> <p> Спасибо за регистрацию. Чтобы активировать ваш аккаунт, нажмите кнопку ниже: </p> <a href='{confirmLink}' class='btn'>Подтвердить Email</a> <p class='footer'> Если вы не регистрировались — просто игнорируйте это письмо. </p> </div> </body> </html>";
            var message = new MailMessage(); 
            message.To.Add(email); 
            message.Subject = "Confirm your account"; 
            message.Body = $"Click the link to confirm your account:\n{confirmLink}"; 
            message.From = new MailAddress("allyut12zk@gmail.com"); 
            using var smtp = new SmtpClient("smtp.gmail.com", 587) 
            { 
                Credentials = new NetworkCredential("allyut12zk@gmail.com", "zuudhqoclgydusxx"), EnableSsl = true 
            }; 
            await smtp.SendMailAsync(message); }
    }
}
