using FFMpegCore;
using FFMpegCore.Pipes;

namespace TelegramBotChatGpt.Services
{
    /// <summary>
    /// unfinished!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
    /// </summary>
    internal class ConverterOggToMp3
    {
        static string path = "../../../Files/";
        public async void convertToMp3(string fileName)
        {
            await using var audioInputStream = File.Open(string.Concat(path, fileName, ".ogg"), FileMode.Open);
            await using var audioOutputStream = File.Open(string.Concat(path, fileName, ".mp3"), FileMode.OpenOrCreate);


            FFMpegArguments
                .FromPipeInput(new StreamPipeSource(audioInputStream))
                .OutputToPipe(new StreamPipeSink(audioOutputStream), options => options.ForceFormat("mp3"))
                .ProcessSynchronously();

            audioOutputStream.Close();
        }
    }
}
