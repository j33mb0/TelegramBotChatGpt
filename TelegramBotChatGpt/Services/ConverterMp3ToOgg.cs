using FFMpegCore.Pipes;
using FFMpegCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelegramBotChatGpt.Services
{
    internal class ConverterMp3ToOgg
    {
        static string path = "../../../Files/";
        public async Task convertToOgg(string fileName)
        {
            await using var audioInputStream = File.Open(string.Concat(path, fileName, ".mp3"), FileMode.Open);
            await using var audioOutputStream = File.Open(string.Concat(path, fileName, ".ogg"), FileMode.OpenOrCreate);


            FFMpegArguments
                .FromPipeInput(new StreamPipeSource(audioInputStream))
                .OutputToPipe(new StreamPipeSink(audioOutputStream), options => options.ForceFormat("ogg"))
                .ProcessSynchronously();

            audioOutputStream.Close();
        }
    }
}
