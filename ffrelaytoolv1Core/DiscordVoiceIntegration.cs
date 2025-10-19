using ffrelaytoolv1.Properties;
using NetCord;
using NetCord.Gateway;
using NetCord.Gateway.Voice;
using NetCord.Logging;
using NetCord.Rest;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using static ffrelaytoolv1.MetaFile;

namespace ffrelaytoolv1
{
    /// <summary>
    /// https://discord.com/api/oauth2/authorize?client_id=1109449718146289725&permissions=34603008&scope=bot
    /// </summary>
    internal class DiscordVoiceIntegration
    {
        private ulong channelId;
        private ulong guildId;
        private string token;
        private VoiceClient connected;

        private Dictionary<ulong, (string, bool)> speakingMap = new Dictionary<ulong, (string, bool)>();
        private Dictionary<ulong, string> usernameMap = new Dictionary<ulong, string>();

        public bool shouldCancel { set; get; }
        public bool isActive { private set; get; }

        public DiscordVoiceIntegration(DiscordFeatures config)
        {
            if(config.channelId == 0 || config.guildId == 0 || config.token == "")
            {
                Log("Invalid config!");
                throw new InvalidDataException("Configuration for discord integration is wrong");
            }
            this.channelId = config.channelId;
            this.guildId = config.guildId;
            this.token = config.token;
        }

        public IEnumerable<string> GetSpeaking()
        {
            if(speakingMap == null || !isActive)
            {
                return null;
            }
            return speakingMap.Where(kvp => kvp.Value.Item2).Select(kvp => kvp.Value.Item1);
        }

        public BackgroundWorker GenerateThread()
        {
            var thread = new BackgroundWorker();
            thread.DoWork += MainBotAsync;
            shouldCancel = false;
            return thread;
        }

        private GatewayClient _client;
        private RestClient _rest;
        private VoiceClient audioClient;

        public class CancellationEventArgs : EventArgs
        {
            public CancellationToken cancelToken;

            public CancellationEventArgs(CancellationToken token)
            {
                cancelToken = token;
            }
        }

        private void Log(string message)
        {
            Debug.WriteLine(message);
            Console.WriteLine(message);
        }

        private async void MainBotAsync(object sender, DoWorkEventArgs e)
        {
            isActive = true;
            _client = new(new BotToken(token), new GatewayClientConfiguration
            {
                Logger = new ConsoleLogger(),
                Intents = GatewayIntents.GuildVoiceStates
            });

            _rest = new(new BotToken(token), new RestClientConfiguration
            {
                Logger = new ConsoleLogger(),
            });

            _client.Ready += OnReady;
            _client.VoiceServerUpdate += OnVoiceServerUpdated;
            _client.VoiceStateUpdate += OnUserVoiceStateUpdated;

            await _client.StartAsync();
            

            while (!shouldCancel)
            {
                await Task.Delay(10_000); //every 10 seconds check for cancellation
            }
            if(connected != null)
            {
                await connected.CloseAsync();
            }
            await Task.Delay(5_000); //5s grace period on disconnection
            await _client.CloseAsync();
            shouldCancel = false;
            isActive = false;
        }

        private async ValueTask OnReady(ReadyEventArgs args)
        {
            var _guild = await _rest.GetGuildAsync(guildId);
            if(_guild == null)
            {
                Log("Unable to resolve guild information");
            }
            var channelDetailsRest = await _rest.GetChannelAsync(channelId);
            Log("channel rest details: " + channelDetailsRest.Id + ", " + ", " + channelDetailsRest.GetType());
            //var resolvedChannel = _rest.GetChannel(channelId);
            //Log("SocketChannel: " + resolvedChannel);
            if (channelDetailsRest == null)
            {
                Log("Unable to resolve channel");
            }
            else
            {
                audioClient = await _client.JoinVoiceChannelAsync(
                    guildId,
                    channelId,
                    new VoiceClientConfiguration
                    {
                        ReceiveHandler = new VoiceReceiveHandler(),
                        Logger = new ConsoleLogger()
                    }
                    );

                audioClient.Connect += OnVoiceChannelConnection;
                audioClient.UserDisconnect += OnVoiceClientDisconnected;
                audioClient.Speaking += OnVoiceSpeakingUpdated;
                audioClient.LatencyUpdate += audioClient_LatencyUpdated;
                audioClient.VoiceReceive += AudioClient_VoiceReceive;

                await audioClient.StartAsync();
                //_ = Task.Run(async () => {
                //    await PeriodicAsync(
                //        async () => await clearupTimer_Tick(audioClient),
                //        new TimeSpan(0, 0, 0, 0, 100),
                //        async () => { await audioClient.StopAsync();}
                //    );
                //});
                _ = Task.Run(async () =>
                {
                    await Task.Delay(new TimeSpan(0, 0, 5));
                    Log("sending voice data as test");
                    speakingMap.Clear();
                    Log("setting speaking");
                    await audioClient.EnterSpeakingStateAsync(new SpeakingProperties(SpeakingFlags.Microphone));
                    Log("Reading audio stream");
                    //Send an initial bit of audio to bootstrap listening to voice data.
                    using (var stream = Resources.bigBridge)
                    using (var outStream = audioClient.CreateOutputStream())
                    {
                        try
                        {
                            Log("Writing audio stream");
                            await stream.CopyToAsync(outStream);
                        }
                        finally
                        {
                            Log("Flushing audio stream");
                            await outStream.FlushAsync();
                        }
                    }
                    Log("Exit");
                });
            }
        }

        private ValueTask AudioClient_VoiceReceive(VoiceReceiveEventArgs arg)
        {
            var frame = arg.Frame;
            audioClient.Cache.Users.TryGetValue(arg.Ssrc, out var voiceUserId);
            if(voiceUserId == 0)
            {
                // Ignore unattached frames??
                return default;
            }
            if (!speakingMap.ContainsKey(voiceUserId))
            {
                var user = _rest.GetUserAsync(voiceUserId).Result;
                speakingMap.Remove(voiceUserId);
                speakingMap.Add(voiceUserId, (user.Username, false));
            }
            if (frame.Length <= 3 || ByteArrayIsAllZero(frame))
            {
                speakingMap[voiceUserId] = (speakingMap[voiceUserId].Item1, false);
            } else
            {
                speakingMap[voiceUserId] = (speakingMap[voiceUserId].Item1, true);
            }
            return default;
        }

        private ValueTask OnVoiceClientDisconnected(UserDisconnectEventArgs args)
        {
            //User for id disconnected from channel
            if (speakingMap.ContainsKey(args.UserId))
            {
                speakingMap.Remove(args.UserId);
            }
            return default;
        }

        public async Task PeriodicAsync(Func<Task> action, TimeSpan interval, Func<Task> cleanup, CancellationToken cancellationToken = default)
        {
            while (isActive)
            {
                var delayTask = Task.Delay(interval, cancellationToken);
                await action();
                await delayTask;
            }
            await cleanup();
        }

        //private Task clearupTimer_Tick(VoiceClient audioClient)
        //{
        //    try
        //    {
        //        var streams = audioClient.GetStreams();
        //        var knownMissing = speakingMap.Keys.ToHashSet();
        //        foreach (var stream in streams)
        //        {
        //            var user = _client.GetUser(stream.Key);
        //            var guildUser = _guild.GetUser(user.Id);
        //            if (user == null)
        //            {
        //                Log("Null user resolved for stream");
        //                continue;
        //            }
        //            else
        //            {
        //                knownMissing.Remove(user.Username);
        //                // Log($"Stream is for user {guildUser.DisplayName} ({user.Username})");
        //            }
        //            RTPFrame frame;
        //            var availableFrames = stream.Value.AvailableFrames;
        //            //Log($"Available stream frames: {availableFrames}");
        //            if (availableFrames == 0 && speakingMap.ContainsKey(user.Username))
        //            {
        //                speakingMap.Remove(user.Username);
        //                continue;
        //            }
        //            var frameToCheck = availableFrames - 1;
        //            for (var i = 0; i < availableFrames; i++)
        //            {
        //                //Need to read ALL of the frames first and get to the end and then decide if we hit silence when the stream finally stops.
        //                stream.Value.TryReadFrame(new CancellationToken(), out frame);
        //                if (i != frameToCheck)
        //                {
        //                    continue;
        //                }
        //                // Only use the contents from the last frame
        //                var hasKey = speakingMap.ContainsKey(user.Username);
        //                var allZero = ByteArrayIsAllZero(frame.Payload);
        //                if (hasKey && allZero)
        //                {
        //                    speakingMap.Remove(user.Username);
        //                }
        //                else if (!hasKey && !allZero)
        //                {
        //                    speakingMap.Add(user.Username, (guildUser.Username, true));
        //                }
        //            }
        //        }
        //        foreach (var missing in knownMissing)
        //        {
        //            speakingMap.Remove(missing);
        //        }
        //    } catch (Exception e)
        //    {
        //        Log("Error in speaking loop, terminating");
        //        Log(e.ToString());
        //        speakingMap = null;
        //    }
        //    return Task.CompletedTask;
        //}

        private bool ByteArrayIsAllZero(ReadOnlySpan<byte> bytes)
        {
            if (bytes == null) { return true; }
            return !Array.Exists(bytes.ToArray(), b => b != 0);
        }

        private ValueTask audioClient_LatencyUpdated(TimeSpan span)
        {
            Log($"Discord client latency udpated to {span}");
            return default;
        }

        private ValueTask OnVoiceSpeakingUpdated(SpeakingEventArgs args)
        {
            Log("voice speaking event update");
            var user = _rest.GetUserAsync(args.UserId).Result;
            speakingMap.Remove(args.UserId);
            speakingMap.Add(args.UserId, (user.Username, false));
            return default;
        }

        private ValueTask OnVoiceChannelConnection()
        {
            Log("Bot Connected to voice!");
            return default;
        }

        private ValueTask OnVoiceServerUpdated(VoiceServerUpdateEventArgs server)
        {
            Log("voice token "+server.Token);
            return default;
        }

        private ValueTask OnUserVoiceStateUpdated(VoiceState state)
        {
            Log($"User is now speaking {state.User.Username}");
            return default;
        }
    }
}
