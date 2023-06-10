using Discord;
using Discord.Audio;
using Discord.Audio.Streams;
using Discord.Interactions;
using Discord.WebSocket;
using ffrelaytoolv1.Properties;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ffrelaytoolv1
{
    /// <summary>
    /// https://discord.com/api/oauth2/authorize?client_id=1109449718146289725&permissions=34603008&scope=bot
    /// </summary>
    internal class DiscordVoiceIntegration
    {
        private ulong channelId = 251434041012781057;
        private ulong guildId = 152531471477899265;

        private Dictionary<string, (string, bool)> speakingMap = new Dictionary<string, (string, bool)>();

        public IEnumerable<string> GetSpeaking()
        {
            return speakingMap.Where(kvp => kvp.Value.Item2).Select(kvp => kvp.Value.Item1 ?? kvp.Key);
        }

        public BackgroundWorker GenerateThread()
        {
            var thread = new BackgroundWorker();
            thread.DoWork += MainBotAsync;
            return thread;
        }

        private DiscordSocketClient _client;
        private SocketGuild _guild;

        private async void MainBotAsync(object sender, EventArgs e)
        {
            _client = new DiscordSocketClient();

            _client.Log += Log;
            var token = Resources.discordToken;

            await _client.LoginAsync(Discord.TokenType.Bot, token);
            await _client.StartAsync();
            _client.Ready += OnReady;
            _client.UserVoiceStateUpdated += OnUserVoiceStateUpdated;
            _client.VoiceServerUpdated += OnVoiceServerUpdated;

            
            await Task.Delay(-1);
        }

        private async Task OnReady()
        {
            _guild = _client.GetGuild(guildId);
            if(_guild == null)
            {
                Console.WriteLine("Unable to resolve guild information");
            }
            var channelDetailsRest = await _client.GetChannelAsync(channelId);
            Console.WriteLine("channel rest details: " + channelDetailsRest.Name + "," + channelDetailsRest.Id + ", " + channelDetailsRest.GetChannelType() + ", " + channelDetailsRest.GetType());
            var resolvedChannel = _client.GetChannel(channelId);
            Console.WriteLine("SocketChannel: " + resolvedChannel);
            if (resolvedChannel == null)
            {
                Console.WriteLine("Unable to resolve channel");
            }
            else
            {
                var audioClient = await (resolvedChannel as SocketVoiceChannel).ConnectAsync();
                audioClient.Connected += OnVoiceChannelConnection;
                audioClient.ClientDisconnected += OnVoiceClientDisconnected;
                audioClient.SpeakingUpdated += OnVoiceSpeakingUpdated;
                audioClient.LatencyUpdated += async(i1, i2) => await audioClient_LatencyUpdated(i1, i2, audioClient);
                _ = Task.Run(async () => { await PeriodicAsync(async () => await clearupTimer_Tick(audioClient), new TimeSpan(0, 0, 0, 0, 100)); });
                _ = Task.Run(async () =>
                {
                    await Task.Delay(new TimeSpan(0, 0, 10));
                    Console.WriteLine("sending voice data as test");
                    speakingMap.Clear();
                    Console.WriteLine("setting speaking");
                    await audioClient.SetSpeakingAsync(true);
                    Console.WriteLine("Reading audio stream");
                    //Send an initial bit of audio to bootstrap listening to voice data.
                    using (var stream = Resources.bigBridge)
                    using (var outStream = audioClient.CreatePCMStream(AudioApplication.Mixed))
                    {
                        try
                        {
                            Console.WriteLine("Writing audio stream");
                            await stream.CopyToAsync(outStream);
                        }
                        finally
                        {
                            Console.WriteLine("Flushing audio stream");
                            await outStream.FlushAsync();
                        }
                    }
                    Console.WriteLine("Unsetting speaking");
                    await audioClient.SetSpeakingAsync(false);
                    Console.WriteLine("Exit");
                });

                //Need to wrap my own speaking code here.
                //Have to consume all streams and then put a timeout on the loop to remove the user from speaking if they haven't transmitted data in a sufficient period of time.

            }
        }

        private Task OnVoiceClientDisconnected(ulong id)
        {
            //User for id disconnected from channel
            var user = _guild.GetUser(id);
            if (speakingMap.ContainsKey(user.Username))
            {
                speakingMap.Remove(user.Username);
            }
            return Task.CompletedTask;
        }

        public static async Task PeriodicAsync(Func<Task> action, TimeSpan interval, CancellationToken cancellationToken = default)
        {
            while (true)
            {
                var delayTask = Task.Delay(interval, cancellationToken);
                await action();
                await delayTask;
            }
        }

        private Task clearupTimer_Tick(IAudioClient audioClient)
        {
            var streams = audioClient.GetStreams();
            var knownMissing = speakingMap.Keys.ToHashSet();
            foreach(var stream in streams)
            {
                var user = _client.GetUser(stream.Key);
                var guildUser = _guild.GetUser(user.Id);
                if (user == null)
                {
                    Console.WriteLine("Null user resolved for stream");
                    continue;
                } else
                {
                    knownMissing.Remove(user.Username);
                    Console.WriteLine($"Stream is for user {guildUser.DisplayName} ({user.Username})");
                }
                RTPFrame frame;
                var availableFrames = stream.Value.AvailableFrames;
                //Console.WriteLine($"Available stream frames: {availableFrames}");
                if(availableFrames == 0 && speakingMap.ContainsKey(user.Username))
                {
                    speakingMap.Remove(user.Username);
                    continue;
                }
                var frameToCheck = availableFrames - 1;
                for (var i = 0; i < availableFrames; i++)
                {
                    //Need to read ALL of the frames first and get to the end and then decide if we hit silence when the stream finally stops.
                    stream.Value.TryReadFrame(new CancellationToken(), out frame);
                    if(i != frameToCheck)
                    {
                        continue;
                    }
                    // Only use the contents from the last frame
                    var hasKey = speakingMap.ContainsKey(user.Username);
                    var allZero = ByteArrayIsAllZero(frame.Payload);
                    if (hasKey && allZero)
                    {
                        speakingMap.Remove(user.Username);
                    }
                    else if (!hasKey && !allZero)
                    {
                        speakingMap.Add(user.Username, (guildUser.DisplayName, true));
                    }
                }
            }
            foreach(var missing in knownMissing)
            {
                speakingMap.Remove(missing);
            }
            return Task.CompletedTask;
        }

        private bool ByteArrayIsAllZero(byte[] bytes)
        {
            if (bytes == null) { return true; }
            return !Array.Exists(bytes, b => b != 0);
        }

        private Task audioClient_LatencyUpdated(int arg1, int arg2, IAudioClient client)
        {
            Console.WriteLine($"Latency udpated from {arg1} to {arg2}");
            return Task.CompletedTask;
        }

        private Task OnVoiceSpeakingUpdated(ulong id, bool speaking)
        {
            Console.WriteLine("voice speaking event update");
            var user = _guild.GetUser(id);
            speakingMap.Remove(user.Username);
            if (speaking)
            {
                speakingMap.Add(user.Username, (user.DisplayName, speaking));
            }
            return Task.CompletedTask;
        }

        private Task OnVoiceChannelConnection()
        {
            Console.WriteLine("Bot Connected to voice!");
            return Task.CompletedTask;
        }

        private Task OnVoiceChannelConnectionMade(IAudioClient client)
        {
            return Task.Run(async () => {
                Console.WriteLine("Connected to voice!");
                speakingMap.Clear();
                //Send an initial bit of audio to bootstrap listening to voice data.
                using (var stream = Resources.join)
                using (var outStream = client.CreatePCMStream(AudioApplication.Mixed))
                {
                    try { await stream.CopyToAsync(outStream); }
                    finally { await outStream.FlushAsync(); }
                }
            });
        }

        private Task OnVoiceServerUpdated(SocketVoiceServer server)
        {
            Console.WriteLine("voice token "+server.Token);
            return Task.CompletedTask;
        }

        private Task OnUserVoiceStateUpdated(SocketUser user, SocketVoiceState state, SocketVoiceState state2)
        {
            var guildUser = _guild.GetUser(user.Id);
            Console.WriteLine($"Voice status updated for user {guildUser?.Nickname} ({user.Username}) - {state} -> {state2}");
            return Task.CompletedTask;
        }
        private Task Log(LogMessage msg)
        {
            Console.WriteLine(msg.ToString());
            return Task.CompletedTask;
        }
    }
}
