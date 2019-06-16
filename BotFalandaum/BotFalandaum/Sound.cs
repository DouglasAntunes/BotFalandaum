using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

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
            this.Name = name;
            this.Weight = weight;
            this.PartDelay = partDelay;
        }

        public string Name { get => name; set => name = value; }
        public int Weight { get => weight; set => weight = value; }
        public int PartDelay { get => partDelay; set => partDelay = value; }
        public byte[] Buffer { get => buffer; set => buffer = value; }

        public void Load()
        {
            byte[] bytes = File.ReadAllBytes(null);
            using (FileStream fs = new FileStream(null, FileMode.Open, FileAccess.Read))
            {

                MemoryStream ms = new MemoryStream(bytes.Length);
                BinaryReader b = new BinaryReader(fs);

                while (fs.Read(null, 0, sizeof(Int16)) > 0)
                {
                    ms.Write(Buffer, 0, bytes.Length);
                }
            }
        }
    }
}
