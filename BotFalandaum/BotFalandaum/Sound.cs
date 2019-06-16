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
                buffer = ReadFully(ffmpeg.StandardOutput.BaseStream);
            }
            Console.WriteLine($"Concluido {name}. Tamanho {buffer.Length}");
        }

        public void Play()
        {

        }

        //Arguments found on https://stackoverflow.com/questions/35754212/not-outputting-opus-raw-audio and http://ffmpeg.org/ffmpeg-codecs.html#libopus and executing ffmpeg -h
        public static Process CreateStream(string path)
        {
            return Process.Start(new ProcessStartInfo
            {
                FileName = "ffmpeg",
                //Arguments = $"-hide_banner -loglevel panic -i \"{path}\" -ac 2 -f s16le -ar 48000 pipe:1",
                Arguments = $"-hide_banner -loglevel panic -i \"{path}\" -sample_fmt s16 -ar 48000 -ac 2 -acodec libopus -b:a 192k -vbr on -compression_level 10 -map 0:a -f data pipe:1",
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
