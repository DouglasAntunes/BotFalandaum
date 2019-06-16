using System;
using System.Diagnostics;
using System.IO;

namespace BotFalandaum
{
    class Sound
    {
        string name;
        int weight;
        byte[] buffer;

        //igual ao createSound()
        public Sound(string name, int weight)
        {
            this.name = name;
            this.weight = weight;
        }

        public string Name { get => name; set => name = value; }
        public int Weight { get => weight; set => weight = value; }
        public byte[] Buffer { get => buffer; set => buffer = value; }


        public void Load(SoundCollection c)
        {
            Console.WriteLine("Carregando " + name);
            string path = $".\\audios\\{c.Prefix}\\{name}.mp3";
            using (var ffmpeg = CreateStream(path))
            {
                Buffer = ReadFully(ffmpeg.StandardOutput.BaseStream);
            }
            Console.WriteLine($"Concluido {name}");
        }

        public void Play()
        {

        }

        public static Process CreateStream(string path)
        {
            return Process.Start(new ProcessStartInfo
            {
                FileName = "ffmpeg",
                Arguments = $"-hide_banner -loglevel panic -i \"{path}\" -ac 2 -f s16le -ar 48000 pipe:1",
                UseShellExecute = false,
                RedirectStandardOutput = true,
            });
        }
        
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
