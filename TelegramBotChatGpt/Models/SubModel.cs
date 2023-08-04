using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelegramBotChatGpt.Models
{
    internal class SubModel
    {
        public double confidence { get; set; }
        public Token[] tokens { get; set; } = default!;
    }
}
