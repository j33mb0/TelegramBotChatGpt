using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelegramBotChatGpt.Models
{
    internal class Model
    {
        public string text { get; set; } = default!;
        public object? entities { get; set; } = default!;
        public object[]? intents { get; set; } = default!;
        public SubModel? speech { get; set; } = default!;
        public object? traits { get; set; } = default!;
    }
}
