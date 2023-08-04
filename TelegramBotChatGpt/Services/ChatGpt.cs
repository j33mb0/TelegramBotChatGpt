using System.Net.Http.Json;
using TelegramBotChatGpt.Models;

namespace TelegramBotChatGpt.Services
{
    
    internal class ChatGpt
    {
        string apiKey = "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";
        string endpoint = "https://api.openai.com/v1/chat/completions";
        HttpClient httpClient = new HttpClient();
        List<Message> messages = new List<Message>();
        public async Task<string> GetRequestFrmGpt(string text)
        {
            if (!httpClient.DefaultRequestHeaders.Contains("Authorization"))
                httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");
            var message = new Message() { Role = "user", Content = text };
            messages.Add(message);
            var requestData = new Request()
            {
                ModelId = "gpt-3.5-turbo",
                Messages = messages
            };
            using var response = await httpClient.PostAsJsonAsync(endpoint, requestData);
            Console.WriteLine(response.StatusCode + "" + response.RequestMessage);
            ResponseData? responseData = await response.Content.ReadFromJsonAsync<ResponseData>();
            var choices = responseData?.Choices ?? new List<Choice>();
            if (choices.Count == 0)
            {
                return "No choices were returned by the API";
            }
            var choice = choices[0];
            var responseMessage = choice.Message;
            var responseText = responseMessage.Content.Trim();
            return $"ChatGPT: {responseText}";
        }
    }
}
