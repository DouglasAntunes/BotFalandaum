using System;
using System.Collections.Generic;
using System.Text;

namespace BotFalandaum
{
    class SoundCollection
    {
        string prefix;
        string[] commands;
        Sound[] sounds;
        int soundRange;

        public string Prefix { get => prefix; set => prefix = value; }
        public string[] Commands { get => commands; set => commands = value; }
        public Sound[] Sounds { get => sounds; set => sounds = value; }
        public int SoundRange { get => soundRange; set => soundRange = value; }

        public void Load()
        {
            foreach (Sound s in Sounds)
            {
                this.SoundRange += s.Weight;
                s.Load();
            }

        }

        public Sound Random()
        {
            int j = 0;
            int number = RandomRange(0, soundRange);

            foreach (Sound s in sounds)
            {
                j += s.Weight;

                if (number < j)
                {
                    return s;
                }
            }
            return null;
        }

        private int RandomRange(int min, int max)
        {
            long time = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            Random rand = new Random();
            return rand.Next(max - min) + min;
        }
    }
}
