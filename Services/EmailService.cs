using System.Net;
using System.Net.Mail;
using System.Net.Http.Json;

namespace WebApplication2.Services
{
    public class EmailService { private readonly IConfiguration _config; private readonly HttpClient _http; 
        public EmailService(IConfiguration config, HttpClient http) { 
            _config = config; _http = http; } 
        public async Task SendConfirmationEmailAsync(string email, string token) { 
            var baseUrl = _config["BASE_URL"]; var confirmLink = $"{baseUrl}/Auth/Confirm?token={token}"; 
            var html = $"<h2>Подтверждение</h2><a href='{confirmLink}'>Подтвердить</a>"; var payload = new { 
                from = "noreply@yourdomain.com", to = email, subject = "Подтверждение регистрации", html = html }; 
            _http.DefaultRequestHeaders.Add("Authorization", $"Bearer {_config["RESEND_API_KEY"]}"); 
            await _http.PostAsJsonAsync("https://api.resend.com/emails", payload); } }
}


// Credentials = new NetworkCredential("allyut12zk@gmail.com", "zuudhqoclgydusxx"), EnableSsl = true 
