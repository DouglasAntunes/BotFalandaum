using Discord;
using Discord.WebSocket;
using System;
using System.Threading.Tasks;

namespace BotFalandaum
{
    class Program
    {
        string botVersion = "0.1";
        int MaxQueueSize = 20;
        //List<Play> = new List<Play>();
        string Owner;

        string BotOwnerDefault = "";
        string BotTokenDefault = "";

        //
        private DiscordSocketClient _client;


        static void Main(string[] args)
        {
            //Sound Load Test Sequence
            Sound[] sounds = {
                new Sound("default", 1000, 250),
                new Sound("clownshort", 800, 250)
            };

            string[] commands = {
                "!airhorn"
            };

            SoundCollection c = new SoundCollection("airhorn", commands, sounds);
            c.Load();

            Console.ReadKey();

            new Program().MainAsync().GetAwaiter().GetResult();

            Console.ReadKey();
        }

        public async Task MainAsync()
        {
            _client = new DiscordSocketClient();

            _client.Log += Log;
            _client.MessageReceived += MessageReceived;

            await _client.LoginAsync(TokenType.Bot, BotTokenDefault);
            await _client.StartAsync();
            
            // Block this task until the program is closed.
            await Task.Delay(-1);
        }

        private async Task MessageReceived(SocketMessage message)
        {
            if(message.Content == "!ping")
            {
                await message.Channel.SendMessageAsync("Pong!");
            }
        }

        private Task Log(LogMessage msg)
        {
            Console.WriteLine(msg.ToString());
            return Task.CompletedTask;
        }

        public async Task JoinChannel(IVoiceChannel channel = null)
        {
            channel = channel ?? (msg.Author as IGuildUser)?.VoiceChannel;
            if(channel == null)
            {
                await msg.Channel.S
            }
            var audioClient = await channel.ConnectAsync();
        }

        /*private async Task SendAsync(IAudioClient client, Sound[] s)
        {
            var discord = 
            //await discord.
        }*/
    }
}