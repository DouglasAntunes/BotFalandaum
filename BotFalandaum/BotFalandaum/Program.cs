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

        public static string ProgramPath;

        //
        private DiscordSocketClient _client;
        private static SoundCollection c;
        private IAudioClient _audioClient;
        public IVoiceChannel VoiceChannel { get; private set; }

        static void Main(string[] args)
        {

            //Configuration File
            ReadAllConfigs();
            ProgramPath = Environment.CurrentDirectory;
            //Sound Load Test Sequence
            Sound[] sounds = {
                new Sound("default", 1000, 250),
                //new Sound("clownshort", 800, 250)
            };

            string[] commands = {
                "!airhorn"
            };

            c = new SoundCollection("airhorn", commands, sounds);
            c.Load();

            //Console.ReadKey();

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
            if(message.Content == "!test")
            {
                JoinChannel(message).Start();
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
                //User isnt on a VoiceChannel
                await msg.Channel.SendMessageAsync("Você não está em um Voice Chat!");
            }
            else
            {
                VoiceChannel = channel;
                SendAudio().Start();
            }

        }

        private async Task<IAudioClient> GetAudioClient()
        {
            if(_audioClient == null)
            {
                _audioClient = await VoiceChannel.ConnectAsync();
            }
            return _audioClient;
        }

        private async Task SendAsync(IAudioClient client, byte[] output)
        {
            using (var discord = client.CreatePCMStream(AudioApplication.Mixed))
            using (MemoryStream ms = new MemoryStream(output))
            {
                try
                {
                    await ms.CopyToAsync(discord);
                }
                finally
                {
                    await discord.FlushAsync();
                }
            }
        }

        private async Task SendAudio()
        {
            var ac = await GetAudioClient();
            await SendAsync(ac, c.Sounds[0].Buffer);
            /*using (var ffmpeg = Sound.CreateStream("C:\\Users\\Douglas\\Source\\Repos\\BotFalandaum\\BotFalandaum\\BotFalandaum\\audios\\airhorn\\default.mp3"))
            using (var output = ffmpeg.StandardOutput.BaseStream)
            using (var discord = ac.CreatePCMStream(AudioApplication.Mixed))
            {
                try { await output.CopyToAsync(discord); }
                finally { await discord.FlushAsync(); }
            }*/
            ac.Dispose();
        }
    }
}