using System;

namespace BotFalandaum
{
    class SoundCollection
    {
        public string Prefix { get; set; }
        public string[] Commands { get; set; }
        public Sound[] Sounds { get; set; }
        public int SoundRange { get; set; }

        public SoundCollection(string prefix, string[] commands)
        {
            this.Prefix = prefix;
            this.Commands = commands;
        }

        public SoundCollection(string prefix, string[] commands, Sound[] sounds)
            : this(prefix, commands)
        {
            this.Sounds = sounds;
        }
        
        public void Load()
        {
            foreach (Sound s in Sounds)
            {
                SoundRange += s.Weight;
                s.Load(this);
            }

        }

        public Sound Random()
        {
            int j = 0;
            int number = RandomRange(0, SoundRange);

            foreach (Sound s in Sounds)
            {
                j += s.Weight;

                if (number < j)
                {
                    return s;
                }
            }
            return null;
        }

        public static int RandomRange(int min, int max)
        {
            Random rand = new Random(DateTime.UtcNow.Millisecond);
            return rand.Next(max - min) + min;
        }


    }
}
