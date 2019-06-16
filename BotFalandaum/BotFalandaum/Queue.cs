using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Discord;
using Discord.Audio;

namespace BotFalandaum
{
    class Queue
    {
        List<Play> QueueList { get; set;}
        static int MaxQueue { get; set; }

        bool playing = false;

        public IAudioClient _audioClient { get; private set; }
        public IVoiceChannel VoiceChannel { get; private set; }

        public Queue(int size)
        {
            QueueList = new List<Play>();
            MaxQueue = size;
        }    

        public async Task Play()
        {
            if(!playing)
            {
                playing = true;
                while (QueueList.Count > 0)
                {
                    Console.WriteLine($"Playing {QueueList[0].Sound.Name} from user {QueueList[0].User.Username}");
                    await SendAudio(QueueList[0].User, QueueList[0].Sound);
                    //await Task.Delay(5000);
                    Next();
                }
                playing = false;
                _audioClient.Dispose();
            }
        }

        public void Next()
        {
            Console.WriteLine($"Removed {QueueList[0].Sound.Name}");
            QueueList.RemoveAt(0);
            
        }

        public void Add(Sound s, IGuildUser user)
        {
            if(QueueList.Count < MaxQueue)
            {
                Play p = new Play(s, user);
                QueueList.Add(p);
                Console.WriteLine($"Queued: User: {user.Username}, Sound {s.Name}, Pos {QueueList.Count}");
            }
        }

        public void clearQueue()
        {
            QueueList.Clear();
        }

        public void killAudio()
        {
            _audioClient.Dispose();
            playing = false;
            clearQueue();
            Console.WriteLine("[ERRO] Maybe a error on play the audio?");
        }

        private async Task<IAudioClient> GetAudioClient()
        {
            if (_audioClient == null || _audioClient.ConnectionState != ConnectionState.Connected)
            {
                _audioClient = await VoiceChannel.ConnectAsync();
            }
            return _audioClient;
        }

        private async Task SendAudio(IGuildUser user, Sound sound)
        {
            VoiceChannel = user.VoiceChannel;

            var ac = await GetAudioClient();
            using (var discord = ac.CreatePCMStream(AudioApplication.Mixed))
            using (MemoryStream ms = new MemoryStream(sound.Buffer))
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
    }
}
