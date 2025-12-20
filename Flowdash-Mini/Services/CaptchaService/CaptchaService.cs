using Flowdash_Mini.Models;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Flowdash_Mini.Services.CaptchaService
{
    public class CaptchaService : ICaptchaService
    {
        private readonly IOptions<CaptchaOptions> _captcha;
        public CaptchaService(IOptions<CaptchaOptions> captcha)
        {
            _captcha = captcha;
        }

        public async Task<bool> VerifyAsync(string token)
        {
            var secretKey = _captcha.Value.SecretKey;
            if (!string.IsNullOrWhiteSpace(secretKey) && !string.IsNullOrWhiteSpace(token))
            {
                try
                {
                    using (var httpClient = new HttpClient())
                    {
                        var content = new Dictionary<string, string>()
                        {
                            { "secret", secretKey },
                            { "response", token }
                        };

                        var response = await httpClient.PostAsync("https://www.google.com/recaptcha/api/siteverify",
                            new FormUrlEncodedContent(content));

                        var result = await response.Content.ReadAsStringAsync();
                        var json = JsonConvert.DeserializeObject<JObject>(result);
                        return (bool?)json?["success"] == true;
                    }
                }
                catch
                {
                    return false;
                }
            }
            return false;
        }
    }
}
