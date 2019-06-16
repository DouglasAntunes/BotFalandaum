using System;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace BotFalandaum
{
    class Sound
    {
        string name;
        int weight;
        int partDelay;
        Stream buffer;

        //igual ao createSound()
        public Sound(string name, int weight, int partDelay)
        {
            this.name = name;
            this.weight = weight;
            this.partDelay = partDelay;
        }

        public string Name { get => name; set => name = value; }
        public int Weight { get => weight; set => weight = value; }
        public int PartDelay { get => partDelay; set => partDelay = value; }
        public Stream Buffer { get => buffer; set => buffer = value; }

        //public byte[] Buffer { get => buffer; set => buffer = value; }


        public void Load(SoundCollection c)
        {
            Console.WriteLine("Carregando " + name);
            //string path = "audios/" + c.Prefix + "/" + name + ".dca";
            string path = "audios/" + c.Prefix + "/" + name + ".mp3";
            //byte[] bytes = File.ReadAllBytes(path);
            //buffer = File.ReadAllBytes(path);
            /*buffer = new byte[0];
            using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read))
            using(BinaryReader b = new BinaryReader(fs))
            {
                int a = 0;
                while (b.BaseStream.Position != b.BaseStream.Length ) {
                    ushort opusLenght = b.ReadUInt16();
                    
                    byte[] buff;

                    buff = b.ReadBytes(opusLenght);
                    //buff = BitConverter.GetBytes(b.ReadUInt16());

                    Console.WriteLine($"leitura {a} , tamanho {opusLenght}, pos {b.BaseStream.Position} , dado {BitConverter.ToString(buff)}");

                    buffer = Combine(buffer, buff);
                    a++;
                }
            }*/
            using (var ffmpeg = CreateStream(path))
            using(var output = ffmpeg.StandardOutput.BaseStream)
            {
                Buffer = output;
            }
            Console.WriteLine($"Concluido {name}");
        }

        public void Play()
        {

        }
        /*
        //From: https://stackoverflow.com/questions/415291/best-way-to-combine-two-or-more-byte-arrays-in-c-sharp
        private byte[] Combine(params byte[][] arrays)
        {
            byte[] rv = new byte[arrays.Sum(a => a.Length)];
            int offset = 0;
            foreach (byte[] array in arrays)
            {
                System.Buffer.BlockCopy(array, 0, rv, offset, array.Length);
                offset += array.Length;
            }
            return rv;
        }*/

        private Process CreateStream(string path)
        {
            return Process.Start(new ProcessStartInfo
            {
                FileName = "ffmpeg",
                Arguments = $"-hide_banner -loglevel panic -i \"{path}\" -ac 2 -f s16le -ar 48000 pipe:1",
                UseShellExecute = false,
                RedirectStandardOutput = true,
            });
        }
        /*
        //From https://stackoverflow.com/questions/221925/creating-a-byte-array-from-a-stream
        public static byte[] ReadFully(Stream input)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                input.CopyTo(ms);
                return ms.ToArray();
            }
        }*/

    }
}
