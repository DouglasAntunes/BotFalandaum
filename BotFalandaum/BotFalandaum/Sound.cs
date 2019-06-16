using System.IO;

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
            string path = "audio/" + c.Prefix + "/" + name + ".dca";
            //byte[] bytes = File.ReadAllBytes(path);
            buffer = File.ReadAllBytes(path);
            /*using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                MemoryStream ms = new MemoryStream(bytes.Length);
                BinaryReader b = new BinaryReader(fs);
                byte[] temp = new byte[bytes.Length];
                while (fs.Read(temp, 0, sizeof(Int16)) > 0)
                {
                    ms.Write(temp, 0, bytes.Length);
                }
                temp = null;
                buffer = ms.ToArray();
            }
            bytes = null;*/
        }

        public void Play()
        {

        }


    }
}
