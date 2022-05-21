using Discord;
using Discord.WebSocket;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace WelcomeBot
{
    class Bot
    {
        public DiscordSocketClient client { get; private set; }
        private ChannelConnector channel;
        private MediaPlayer mediaPlayer;
        private CommandHandler commandHandler;

        public async Task MainAsync()
        {
            //Reads Json File 
            var json = string.Empty;
            using (var fs = File.OpenRead("config/config.json"))
            using (var sr = new StreamReader(fs, new UTF8Encoding(false)))
            json = await sr.ReadToEndAsync().ConfigureAwait(false);
            var configJson = JsonConvert.DeserializeObject<ConfigJson>(json);
            //Get Token From Json File
            var token = configJson.Token;
            var prefix = configJson.Prefix;

            //Creats Bot
            client = new DiscordSocketClient();
            client.Log += Log;


            mediaPlayer = new MediaPlayer();
            channel = new ChannelConnector(mediaPlayer);

            commandHandler = new CommandHandler(prefix, client, mediaPlayer);

            //Adds Events
            client.UserVoiceStateUpdated += channel.OnVoiceStateUpdated;
            client.MessageReceived += commandHandler.CommandHandle;

            //Starts Up The Bot
            await client.LoginAsync(TokenType.Bot, token);
            await client.StartAsync();

            // Block this task until the program is closed.
            await Task.Delay(-1);
        }

        private Task Log(LogMessage msg)
        {
            Console.WriteLine(msg.ToString());
            return Task.CompletedTask;
        }
    }
}
