using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace TelegramBotChatGpt.Services
{
    internal class TextToVoiceConverter
    {
        string apiKey = "94b6078ec6b04a85b8e3efb57b41c8cd";
        string userId = "kJN8E6YekVTcSZOVC4K3JHb7YXV2";

        public async Task<string> GetVoiceUrl(string text)
        {
            using (var httpClient = new HttpClient())
            {
                using (var request = new HttpRequestMessage(new HttpMethod("POST"), "https://play.ht/api/v2/tts"))
                {
                    request.Headers.TryAddWithoutValidation("AUTHORIZATION", $"Bearer {apiKey}");
                    request.Headers.TryAddWithoutValidation("X-USER-ID", userId);
                    request.Headers.TryAddWithoutValidation("accept", "text/event-stream");

                    request.Content = new StringContent("\n{\n  \"text\": \"" + text + "\",\n  \"voice\": \"larry\"\n}\n");
                    request.Content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json");

                    var response = await httpClient.SendAsync(request);
                    string res = await response.Content.ReadAsStringAsync();
                    int start = res.IndexOf("\"url\":\"");
                    res = res.Substring(start);
                    int end = res.IndexOf("\",");
                    res = res.Substring(7, end - 7);
                    return res;
                }
            }
        }
    }
}
