using Discord.Audio;
using Discord.WebSocket;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace WelcomeBot
{
    class ChannelConnector
    {
        private readonly MediaPlayer mediaPlayer;
        private SocketVoiceChannel voiceState;
        private readonly int delay = 500;
        private bool isConnected = false;
        private IAudioClient client;

        public ChannelConnector(MediaPlayer player)
        {
            mediaPlayer = player;
        }

        public async Task OnVoiceStateUpdated(SocketUser user, SocketVoiceState before, SocketVoiceState after)
        {
            
            SocketVoiceChannel channel = after.VoiceChannel;

            // Check if this was a non-bot user joining a voice channel
            if (user.IsBot) return;

            Console.WriteLine(user.Username + " Joined " + after + " Channel.");

            if (before.VoiceChannel == null && channel.Name.Contains("channelName") && isConnected == false)
            {
                
                try
                {
                    await Task.Delay(delay);
                    client = (IAudioClient)ConnectToChannelAsync(channel);

                    
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                }
                await PlayMedia(client, user.Username);
                await DisconnectFromChannel(channel);
            }
        }

        public async Task<IAudioClient> ConnectToChannelAsync(SocketVoiceChannel channel)
        {
            Console.WriteLine($"Connecting to channel {channel.Name}");
            IAudioClient client = await channel.ConnectAsync().ConfigureAwait(false);
            voiceState = channel;
            Console.WriteLine($"Connected to channel!");
            isConnected = true;
            return client;

        }


        public async Task<IAudioClient> ConnectToChannelAsync(SocketVoiceChannel channel, string name)
        {
            Console.WriteLine($"Connecting to channel {channel.Name}");
            IAudioClient client = await channel.ConnectAsync().ConfigureAwait(false);
            voiceState = channel;
            Console.WriteLine($"Connected to channel!");
            isConnected = true;

            await PlayMedia(client, name);
            await DisconnectFromChannel(voiceState);
            return client;
        }

        private async Task PlayMedia(IAudioClient client, string name)
        {
            Console.WriteLine($"Playing Media");
            await mediaPlayer.SendAsync(client, name);
        }

        private async Task DisconnectFromChannel(SocketVoiceChannel channel)
        {
            await Task.Delay(delay);
            try {
                await channel.DisconnectAsync();
            } catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            
            voiceState = null;
            Console.WriteLine($"Disconnected from channel.");
            isConnected = false;
        }
    }
}
