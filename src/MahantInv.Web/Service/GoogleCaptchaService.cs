using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace MahantInv.Web.Service
{
    public class GoogleCaptchaService
    {
        private readonly HttpClient _httpClient;

        public GoogleCaptchaService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<CaptchaResponse> VerifyCaptchaAsync(string token)
        {
            var secretKey = "6Lc0gKAqAAAAAAmPHfk5GiqhnDwXw9D-MAOkxxUX";
            var response = await _httpClient.PostAsync($"https://www.google.com/recaptcha/api/siteverify?secret={secretKey}&response={token}", null);
            var responseString = await response.Content.ReadAsStringAsync();

            return JsonSerializer.Deserialize<CaptchaResponse>(responseString);
        }
    }
    public class CaptchaResponse
    {
        public bool Success { get; set; }
        public double Score { get; set; } // Score for reCAPTCHA v3
        public string Action { get; set; }
        public string Challenge_ts { get; set; }
        public string Hostname { get; set; }
    }
}
