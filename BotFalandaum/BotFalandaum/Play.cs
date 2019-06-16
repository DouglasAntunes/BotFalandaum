using Discord;

namespace BotFalandaum
{
    class Play
    {
        public IGuildUser User { get; set; }
        public Sound Sound { get; set; }

        public Play(Sound s, IGuildUser user)
        {
            this.Sound = s;
            this.User = user;
        }
    }
}
