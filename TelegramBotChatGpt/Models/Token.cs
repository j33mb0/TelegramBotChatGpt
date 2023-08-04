using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelegramBotChatGpt.Models
{
    internal class Token
    {
        public double confidence { get; set; }
        public int end { get; set; }
        public int start { get; set; }
        public string token { get; set; } = default!;
    }
}
