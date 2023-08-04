using System.Net;

namespace TelegramBotChatGpt.Services
{
    /// <summary>
    /// i think it finished
    /// </summary>
    internal class ConverterAudioToText
    {
        private static readonly string APIToken = "WKOKAL6N5WFKGYYCEPPWGX5QPHU3TYGO";
        public async Task<string> ConvertToText(string path)
        {
            using (Stream stream = File.OpenRead(path))
            {
                string result = "";
                var ms = new MemoryStream();
                stream.CopyTo(ms);
                var arr = ms.ToArray();
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://api.wit.ai/speech");
                request.SendChunked = true;
                request.Method = "POST";
                request.Headers["Authorization"] = "Bearer " + APIToken;
                request.ContentType = "audio/mpeg3";
                request.ContentLength = arr.Length;
                var st = request.GetRequestStream();
                st.Write(arr, 0, arr.Length);
                st.Close();
                try
                {
                    HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        StreamReader response_stream = new StreamReader(response.GetResponseStream());
                        string res = response_stream.ReadToEnd();
                        return TrimResult(res);
                    }

                }
                catch (Exception ex)
                {
                    return string.Empty;
                }
                return string.Empty;
            }
        }
        private string TrimResult(string text)
        {
            string[] jsons = text.Split("}\r\n{");
            string lj = jsons.Last();
            int start = lj.IndexOf("\"text\":");
            string pres = lj.Substring(start + 9);
            int end = pres.IndexOf("\"");
            string result = pres.Substring(0, end);
            return result;
        }
    }
}
