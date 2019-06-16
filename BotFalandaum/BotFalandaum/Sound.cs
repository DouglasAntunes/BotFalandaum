using System;
using System.IO;
using System.Linq;

namespace BotFalandaum
{
    class Sound
    {
        string name;
        int weight;
        int partDelay;
        byte[] buffer;

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
        public byte[] Buffer { get => buffer; set => buffer = value; }

        public void Load(SoundCollection c)
        {
            Console.WriteLine("Carregando " + name);
            string path = "audios/" + c.Prefix + "/" + name + ".dca";
            //byte[] bytes = File.ReadAllBytes(path);
            //buffer = File.ReadAllBytes(path);
            buffer = new byte[0];
            using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read))
            using(BinaryReader b = new BinaryReader(fs))
            {
                while (b.BaseStream.Position != b.BaseStream.Length) {
                    ushort opusLenght = b.ReadUInt16();

                    byte[] buff;

                    buff = b.ReadBytes(opusLenght);

                    buffer = Combine(buffer, buff);
                }
            }
            Console.WriteLine($"Concluido {name} {buffer.Length}");
        }

        public void Play()
        {

        }

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
        }

    }
}
