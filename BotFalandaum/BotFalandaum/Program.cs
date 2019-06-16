using Discord;
using Discord.WebSocket;
using Discord.Audio;
using System;
using System.Threading.Tasks;
using System.Configuration;
using System.IO;

namespace BotFalandaum
{
    class Program
    {
        string botVersion = "0.1";
        int MaxQueueSize = 20;
        //List<Play> = new List<Play>();
        string Owner;

        static string BotOwnerDefault;
        static string BotTokenDefault;

        //
        private DiscordShardedClient _client;
        private static SoundCollection c;


        static void Main(string[] args)
        {
            //Configuration File
            ReadAllConfigs();
            
            //Sound Load Test Sequence
            Sound[] sounds = {
                new Sound("default", 1000, 250),
                new Sound("clownshort", 800, 250)
            };

            string[] commands = {
                "!airhorn"
            };

            c = new SoundCollection("airhorn", commands, sounds);
            c.Load();

            Console.ReadKey();

            new Program().MainAsync().GetAwaiter().GetResult();

            Console.ReadKey();
        }

        private static void ReadAllConfigs()
        {
            try
            {
                var appSettings = ConfigurationManager.AppSettings;
                if (appSettings.Count == 0)
                {
                    Console.WriteLine("AppSettings is empty.");
                }
                else
                {
                    BotOwnerDefault = appSettings["Owner"];
                    BotTokenDefault = appSettings["Token"];
                }
            }
            catch (ConfigurationErrorsException)
            {
                Console.WriteLine("Error reading app settings");
                System.Environment.Exit(1);
            }
        }

        public async Task MainAsync()
        {
            _client = new DiscordShardedClient();
            
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
            if(message.Content == "!test")
            {
                await JoinChannel(message);
            }
        }

        private Task Log(LogMessage msg)
        {
            Console.WriteLine(msg.ToString());
            return Task.CompletedTask;
        }
        
        public async Task JoinChannel(SocketMessage msg, IVoiceChannel channel = null)
        {
            channel = channel ?? (msg.Author as IGuildUser)?.VoiceChannel;
            if(channel == null)
            {

            }
            Console.WriteLine("caraioo");
            var audioClient = await channel.ConnectAsync();
            Console.WriteLine("caraioo 2");


            using (Stream stream = new MemoryStream(c.Sounds[0].Buffer))
            {
                await SendAsync(audioClient, stream).ConfigureAwait(false); 
            }
                
        }

        private async Task SendAsync(IAudioClient client, Stream output)
        {
            Console.WriteLine(output.Length);
            using (var discord = client.CreateOpusStream())
            {
                try {
                    await output.CopyToAsync(discord);
                }
                finally {
                    await discord.FlushAsync();
                }
            }
        }
    }
}