using System.Net;
using System.Net.Mail;
using System.Net.Http.Json;

namespace WebApplication2.Services
{
    public class EmailService
    {
        private readonly IConfiguration _config; 
        private readonly HttpClient _http; 
        public EmailService(IConfiguration config, HttpClient http) {
            _config = config; _http = http; 
        }
        public async Task SendConfirmationEmailAsync(string email, string token)
        {
            var baseUrl = _config["BASE_URL"]; var confirmLink = $"{baseUrl}/Auth/Confirm?token={token}"; var html = $@" <h2>Подтверждение регистрации</h2> <p>Для активации аккаунта нажмите:</p> <a href='{confirmLink}'>Подтвердить Email</a> "; var payload = new
            {
                From = "1227721@mtp.by",
                To = email, 
                Subject = "Подтверждение регистрации", 
                HtmlBody = html 
            }; 
            _http.DefaultRequestHeaders.Clear(); 
            _http.DefaultRequestHeaders.Add("X-Postmark-Server-Token", 
                _config["POSTMARK_API_TOKEN"]); 
            var response = await _http.PostAsJsonAsync("https://api.postmarkapp.com/email", payload); 
            if (!response.IsSuccessStatusCode) { 
                var error = await response.Content.ReadAsStringAsync(); 
                Console.WriteLine("POSTMARK ERROR: " + error); 
                throw new Exception("Postmark email failed: " + error); 
            } 
        }

    }
}
