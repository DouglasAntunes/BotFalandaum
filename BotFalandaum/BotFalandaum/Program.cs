using Discord;
using Discord.WebSocket;
using System;
using System.Threading.Tasks;
using System.Configuration;
using System.IO;
using System.Collections.Generic;

namespace BotFalandaum
{
    class Program
    {
        public static string botVersion = "0.1";
        private static int MaxQueueSize = 20;

        private static Queue queue;
        private static List<SoundCollection> soundCollection;

        private static string BotOwnerDefault;
        private static string BotTokenDefault;

        //
        private DiscordSocketClient _client;
        

        static void Main(string[] args)
        {

            //Configuration File
            ReadAllConfigs();

            //Initialize queue
            queue = new Queue(MaxQueueSize);

            //Load Audios
            soundCollection = new List<SoundCollection>();

            String[] audioFolders  = Directory.GetDirectories($".\\{Sound.audioFolderName}\\");
            foreach (String path in audioFolders)
            {
                DirectoryInfo dirInfo = new DirectoryInfo(path);
                String folderName = dirInfo.Name;

                String[] audioNamesWithPath = Directory.GetFiles(path, $"*.{Sound.extension}");

                List<string> commands = new List<string>();
                commands.Add($"!{folderName.ToLower()}");

                List<Sound> sounds = new List<Sound>();
                foreach(String soundFile in audioNamesWithPath)
                {
                    String audioName = Path.GetFileNameWithoutExtension(soundFile);
                    sounds.Add(new Sound(audioName.ToLower(), 1));
                }

                SoundCollection sC = new SoundCollection(folderName.ToLower(), commands.ToArray(), sounds.ToArray());
                dirInfo = null;
                sC.Load();
                soundCollection.Add(sC);
            }

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
                Environment.Exit(1);
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
            string[] parts = message.Content.Split(" ");

            foreach (SoundCollection c in soundCollection)
            {
                foreach (string command in c.Commands)
                {
                    if (parts[0] == command)
                    {
                        switch (parts.Length)
                        {
                            //1 part (like !command)
                            case 1:
                            {
                                queue.Add(c.Random(), message.Author as IGuildUser);
                                break;
                            }
                            //2 parts (like !command audio or !command times)
                            case 2:
                            {
                                int value = 0;
                                if(int.TryParse(parts[1], out value))
                                {
                                    for(int i = 0; i < value; i++)
                                    {
                                        queue.Add(c.Random(), message.Author as IGuildUser);
                                    }
                                }
                                else
                                {
                                    foreach (Sound s in c.Sounds)
                                    {
                                        if (parts[1] == s.Name)
                                        {
                                            queue.Add(s, message.Author as IGuildUser);
                                            break;
                                        }
                                    }
                                }
                                break;
                            }
                            //3 parts (like !command audio times)
                            case 3:
                            {
                                int value2 = 0;
                                if (int.TryParse(parts[2], out value2))
                                {
                                    foreach (Sound s in c.Sounds)
                                    {
                                        if (parts[1] == s.Name)
                                        {
                                            for (int i = 0; i < value2; i++)
                                            {
                                                queue.Add(s, message.Author as IGuildUser);
                                            }
                                            break;
                                        }
                                    }
                                }
                                break;
                            }
                        }
                        
                        queue.Play().Start();
                    }
                }
            }

            if (message.Content == "!ping")
            {
                await message.Channel.SendMessageAsync("Pong!");
            }

            if (message.Content == "!comandos")
            {
                string commandsAll = "";
                foreach (SoundCollection c in soundCollection)
                    foreach (string command in c.Commands)
                    {
                        commandsAll += $"{command} ";
                    }
                await message.Channel.SendMessageAsync(commandsAll);
            }

            if (message.Content == "!kill")
            {
                queue.killAudio();
            }

            
            
        }

        private Task Log(LogMessage msg)
        {
            Console.WriteLine(msg.ToString());
            return Task.CompletedTask;
        }

    }
}