using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace TelegramBotChatGpt.Services
{
    internal class DownloaderMp3Files
    {
        public async Task DownloadMp3(string url, string fileName)
        {
            using (var client = new WebClient())
            {
                await client.DownloadFileTaskAsync(new Uri(url), $"..\\..\\..\\Files\\{fileName}.mp3");
            }
        }
    }

}
