using System.Net;
using System.Net.Mail;
using System.Net.Http.Json;

namespace WebApplication2.Services
{
    public class EmailService { 
        private readonly IConfiguration _config; 
        private readonly HttpClient _http; 
        public EmailService(IConfiguration config, HttpClient http) { 
            _config = config; 
            _http = http; 
        }
        public async Task SendConfirmationEmailAsync(string email, string token)
        {
            var baseUrl = _config["BASE_URL"];
            var confirmLink = $"{baseUrl}/Auth/Confirm?token={token}";

            var html = $"<h2>Подтверждение регистрации</h2><a href='{confirmLink}'>Подтвердить</a>";

            var payload = new
            {
                apiKey = _config["ELASTIC_API_KEY"],
                from = _config["allyut12zk@gmail.com"], 
                to = email,
                subject = "Подтверждение регистрации",
                bodyHtml = html
            };

            var response = await _http.PostAsJsonAsync("https://api.elasticemail.com/v2/email/send", payload);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                Console.WriteLine("ELASTIC ERROR: " + error);
                throw new Exception("Elastic email failed: " + error);
            }
        }

    }
}
