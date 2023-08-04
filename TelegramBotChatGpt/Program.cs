using System;
using System.Text;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.InputFiles;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramBotChatGpt.Services;


namespace TelegramBotChatGpt
{
    class Program
    {
        static string path = "../../../Files/";
        static ITelegramBotClient bot = new TelegramBotClient("5080390090:AAHpYzn81b0ANVPB8BjJJ5bE_RrD9-vbZ4M");
        static ConverterOggToMp3 converterMp3 = new ConverterOggToMp3();
        static ConverterMp3ToOgg converterOgg = new ConverterMp3ToOgg();
        static ConverterAudioToText converterToTex = new ConverterAudioToText();
        static TextToVoiceConverter converterToVoice = new TextToVoiceConverter();
        static DownloaderMp3Files downloader = new DownloaderMp3Files();
        static UserSettingsHandler userSettings = new UserSettingsHandler();
        static ChatGpt chatGpt = new ChatGpt();

        public static async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            if(update.Message?.Type == Telegram.Bot.Types.Enums.MessageType.Voice)
            {
                var message = update.Message;


                bool isSaved = await userSettings.IsSavedUser(message.From.Id);
                if (!isSaved)
                    await userSettings.SaveNewUser(message.From.Id);

                var filePath = Path.Combine(path, message.Voice.FileId + ".ogg");
                var voiceId = await bot.GetFileAsync(message.Voice.FileId);
                var tgPath = voiceId.FilePath;
                using (var file = System.IO.File.OpenWrite(filePath))
                {
                    await bot.DownloadFileAsync(tgPath, file);
                }

                converterMp3.convertToMp3(message.Voice.FileId);

                StringBuilder textPath = new StringBuilder();
                textPath.Append(path);
                textPath.Append(message.Voice.FileId.ToString());
                textPath.Append(".mp3");
                string text = await converterToTex.ConvertToText(textPath.ToString());

                string responseFromGpt = await chatGpt.GetRequestFrmGpt(text);

                string responseType = await userSettings.GetUserResponseType(message.From.Id);

                if (responseType == "Voice")
                {
                    string urlText = await converterToVoice.GetVoiceUrl(responseFromGpt);

                    await downloader.DownloadMp3(urlText, message.MessageId.ToString());

                    await converterOgg.convertToOgg(message.MessageId.ToString());

                    await using Stream stream = System.IO.File.OpenRead($"../../../Files/{message.MessageId.ToString()}.ogg");
                    InputOnlineFile onlineFile = new InputOnlineFile(stream);
                    Message voiceMessage = await bot.SendVoiceAsync(chatId: message.From.Id,
                        voice: onlineFile,
                        cancellationToken: cancellationToken);

                    Console.WriteLine(responseFromGpt);
                }
                else
                    await bot.SendTextMessageAsync(message.From.Id, responseFromGpt);
            }

            else if(update.Type == Telegram.Bot.Types.Enums.UpdateType.Message)
            {
                var message = update.Message;
                bool isSaved = await userSettings.IsSavedUser(message.From.Id);
                if (!isSaved)
                    await userSettings.SaveNewUser(message.From.Id);
                if (message.Text == "НАСТРОЙКИ")
                {
                    string typeResponse = await userSettings.GetUserResponseType(message.From.Id);
                    if (typeResponse == "Text")
                    {
                        ReplyKeyboardMarkup replyKeyboardMarkup = new(new[]
                    {
                        new KeyboardButton[] { "Текст☑", "Голос"}
                    })
                        {
                            ResizeKeyboard = true,
                        };
                        await bot.SendTextMessageAsync(message.From.Id, "Тип возвращаемого ответа:", replyMarkup: replyKeyboardMarkup);
                    }
                    else if (typeResponse == "Voice")
                    {
                        ReplyKeyboardMarkup replyKeyboardMarkup = new(new[]
                    {
                        new KeyboardButton[] { "Текст", "Голос☑" }
                    })
                        {
                            ResizeKeyboard = true,
                        };
                        await bot.SendTextMessageAsync(message.From.Id, "Тип возвращаемого ответа:", replyMarkup: replyKeyboardMarkup);
                    }
                }
                else if(message.Text == "Текст")
                {
                    ReplyKeyboardMarkup replyKeyboardMarkup = new(new[]
                    {
                        new KeyboardButton[] { "НАСТРОЙКИ" }
                    })
                    {
                        ResizeKeyboard = true,
                    };

                    await userSettings.ChangeUserResponseType(message.From.Id, "Text");
                    await bot.SendTextMessageAsync(message.From.Id, "Тип возвращаемого ответа изменен на текст", replyMarkup: replyKeyboardMarkup);
                }
                else if (message.Text == "Голос")
                {
                    ReplyKeyboardMarkup replyKeyboardMarkup = new(new[]
                    {
                        new KeyboardButton[] { "НАСТРОЙКИ" }
                    })
                    {
                        ResizeKeyboard = true,
                    };

                    await userSettings.ChangeUserResponseType(message.From.Id, "Voice");
                    await bot.SendTextMessageAsync(message.From.Id, "Тип возвращаемого ответа изменен на голос", replyMarkup: replyKeyboardMarkup);
                }
                else
                {
                    string responseFromGpt = await chatGpt.GetRequestFrmGpt(message.Text);

                    ReplyKeyboardMarkup replyKeyboardMarkup = new(new[]
                    {
                        new KeyboardButton[] { "НАСТРОЙКИ" }
                    })
                    {
                        ResizeKeyboard = true,
                    };
                    await bot.SendTextMessageAsync(message.From.Id, responseFromGpt, replyMarkup: replyKeyboardMarkup);
                }
            }
        }
        public static async Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(exception));
        }

        static void Main(string[] args)
        {
            Console.WriteLine("Запущен бот " + bot.GetMeAsync().Result.FirstName);

            var cts = new CancellationTokenSource();
            var cancellationToken = cts.Token;
            var receiverOptions = new ReceiverOptions
            {
                AllowedUpdates = { }, // receive all update types
            };
            bot.StartReceiving(
                HandleUpdateAsync,
                HandleErrorAsync,
                receiverOptions,
                cancellationToken
            );
            Console.ReadLine();
        }
    }

}

