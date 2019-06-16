using System;
using System.Diagnostics;
using System.IO;

namespace BotFalandaum
{
    class Sound
    {
        public string Name { get ; set; }
        public int Weight { get; set; }
        public byte[] Buffer { get; set; }

        public static string audioFolderName = "audios_mp3";
        public static string extension = "mp3";

        //same as createSound()
        public Sound(string name, int weight)
        {
            this.Name = name;
            this.Weight = weight;
        }
        
        public void Load(SoundCollection c)
        {
            Console.WriteLine("Loading " + Name);
            string path = $".\\{audioFolderName}\\{c.Prefix}\\{Name}.{extension}";
            using (var ffmpeg = CreateStream(path))
            {
                Buffer = ReadFully(ffmpeg.StandardOutput.BaseStream);
            }
            if(Buffer.Length == 0)
            {
                Console.WriteLine($"Error on load {Name}");
            }
        }

        public void Play()
        {

        }

        //Use of ffmpeg to convert to pcm/opus
        public static Process CreateStream(string path)
        {
            return Process.Start(new ProcessStartInfo
            {
                FileName = "ffmpeg",
                Arguments = $"-hide_banner -loglevel panic -i \"{path}\" -ac 2 -f s16le -ar 48000 pipe:1",    //pcm
                //Arguments = $"-hide_banner -loglevel panic -i \"{path}\" -acodec libopus -compression_level 10 -ac 2 -ar 48000 -map 0:a -f data pipe:1",    //opus
                UseShellExecute = false,
                RedirectStandardOutput = true,
            });
        }

        //Use of opusenc to convert to opus (wav => opus)
        /*public static Process CreateStream(string path)
        {
            return Process.Start(new ProcessStartInfo
            {
                FileName = ".\\opusenc",
                //Arguments = $"-hide_banner -loglevel panic -i \"{path}\" -ac 2 -f s16le -ar 48000 pipe:1",    //pcm
                Arguments = $"--quiet {path} -",    //opus
                UseShellExecute = false,
                RedirectStandardOutput = true,
            });
        }*/

        //From https://stackoverflow.com/questions/221925/creating-a-byte-array-from-a-stream
        public static byte[] ReadFully(Stream input)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                input.CopyTo(ms);
                return ms.ToArray();
            }
        }

    }
}
