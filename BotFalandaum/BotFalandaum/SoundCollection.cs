using System;

namespace BotFalandaum
{
    class SoundCollection
    {
        string prefix;
        string[] commands;
        Sound[] sounds;
        int soundRange;

        public SoundCollection(string prefix, string[] commands)
        {
            this.prefix = prefix;
            this.commands = commands;
        }

        public SoundCollection(string prefix, string[] commands, Sound[] sounds)
            : this(prefix, commands)
        {
            this.sounds = sounds;
        }

        public string Prefix { get => prefix; set => prefix = value; }
        public string[] Commands { get => commands; set => commands = value; }
        public Sound[] Sounds { get => sounds; set => sounds = value; }
        public int SoundRange { get => soundRange; set => soundRange = value; }

        public void Load()
        {
            foreach (Sound s in sounds)
            {
                soundRange += s.Weight;
                s.Load(this);
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

        public static int RandomRange(int min, int max)
        {
            Random rand = new Random(DateTime.UtcNow.Millisecond);
            return rand.Next(max - min) + min;
        }


    }
}
