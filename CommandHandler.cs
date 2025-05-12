using Discord.Audio;
using Discord.WebSocket;
using System;
using System.Threading.Tasks;

namespace WelcomeBot
{
    class CommandHandler
    {
        private string prefix;
        private readonly DiscordSocketClient client;
        private readonly MediaPlayer mediaPlayer;
        private SocketGuild guild;
        private ulong guildId = 383980359714340864;
        private SocketVoiceChannel voiceChannel;
        private ulong channelId = 383980361522348033;

        private string author;
        private string command;
        private string input;

        public CommandHandler (String _prefix, DiscordSocketClient _client, MediaPlayer _player)
        {
            prefix = _prefix;
            client = _client;
            mediaPlayer = _player;

        }

        public async Task CommandHandle(SocketMessage msg)
        {
            string[] sounds = mediaPlayer.GetSoundArray();
            string list = "";

            if (!msg.Content.StartsWith(prefix) || msg.Author.IsBot) return;

            SetAuthor(msg);
            SetCommand(msg);

            if (command.Equals("help"))
            {
                await msg.Channel.SendMessageAsync(
                    ">>> ``` __Commands__ \n" +
                   prefix + "Ping : Ping The Bot \n" +
                   prefix + "Intros : Displays List Of Bot Welcome Sounds \n" +
                   prefix + "Favorites: Displays List Of Your Personal Favorite Sounds \n" +
                   prefix + "Play (Index): Plays The Selected Greeting Of Index Input From SoundList. Ex: /Play 1 \n" +
                   prefix + "Set (Index): Adds Index From SoundList To Your Favorites. Enter 0 For Random. \n" +
                   prefix + "Remove (Index): Removes Index In Favorites From Your Favorites List. Verify Favorites List Contains Said Index Prior To Using This Command. \n" +
                   "```");
            }

            else if (command.Equals("ping"))
            {
                await msg.Channel.SendMessageAsync(">>> `I'm Here Because I Have To Be`");
            }

            else if (command.Equals("Intros"))
            {
                for (int i = 0; i < sounds.Length; i++)
                {
                    list += "\n" + (i + 1) + ". " + sounds[i].Substring(7, sounds[i].Length - 7);
                }

                await msg.Channel.SendMessageAsync(">>> ```" + " __My Availible Sounds__ " + list + " ```");
                list = "";
            }

            else if (command.Contains("play"))
            {

                string sound = input;

                try
                {
                    int soundIndex = Int32.Parse(sound) - 1;
                    if (soundIndex >= sounds.Length)
                    {
                        await msg.Channel.SendMessageAsync(">>> ```" + author + ", " + (soundIndex + 1) + " Is Out Of Range. " +
                            "Try A Number From 1 To " + sounds.Length + ".```");
                        return;
                    }
                    await Task.Delay(1 / 2);
                    string soundName = sounds[soundIndex].Substring(7, (sounds[soundIndex].Length - 7));
                    await msg.Channel.SendMessageAsync(">>> ```" + "Playing: " + soundName + "\n" + "This Guy Wanted Me To Play This: " + author + " ```");
                    ConnectToChannelAndPlaySound(sounds[soundIndex]);
                }
                catch (FormatException)
                {
                    await msg.Channel.SendMessageAsync(">>> ```" + author + " That Is Not The Proper Way To Use The Play Command. \"" + sound +
                        "\" Is An Invalid Index.```");
                }
            }

            else if (command.Contains("set"))
            {

                string sound = input;
                int soundindex = Int32.Parse(sound);
                if (soundindex == 0)
                {
                    mediaPlayer.SetDefaultGreeting(author);
                    await msg.Channel.SendMessageAsync(">>> `Hello " + author + ", I Set Your Greeting To The Default: Random.`");
                }
                else
                {
                    sound = sounds[soundindex - 1];

                    Console.WriteLine(author + " Adding Sound: " + sound + " To Favorites");
                    mediaPlayer.AssignGreeting(author, sound);
                    await msg.Channel.SendMessageAsync(">>> `Hello " + author + ", I Added " + sound.Substring(7, sound.Length - 7) + " To Your Favorites`");
                }

            }

            else if (command.Contains("remove"))
            {
                string sound = input;
                string[] favorites = mediaPlayer.GetFavorites(author);
                int soundindex = Int32.Parse(sound);
                if (soundindex <= favorites.Length && soundindex > 0)
                {
                    sound = favorites[soundindex - 1];
                    Console.WriteLine(author + " Removing Sound: " + sound + " From Favorites");
                    mediaPlayer.RemoveGreeting(author, sound);
                    await msg.Channel.SendMessageAsync(">>> `Hello " + author + ", I removed " + sound.Substring(7, sound.Length - 7) + " From Your Favorites`");
                }
            }

            else if (command.Contains("favorites"))
            {
                string[] favorites = mediaPlayer.GetFavorites(author);
                for (int i = 0; i < favorites.Length; i++)
                {
                    list += "\n" + (i + 1) + ". " + favorites[i].Substring(7, favorites[i].Length - 7);
                }

                await msg.Channel.SendMessageAsync(">>> ```" + "__" + author + "'s Favorites" + "__" + list + " ```");
                list = "";
            }
        }

        private void SetAuthor(SocketMessage msg)
        {
           author = msg.Author.ToString().Substring(0, msg.Author.ToString().Length - 5);
        }

        private void SetCommand(SocketMessage msg)
        {
            int lengthOfCommand;
            if (msg.Content.Contains(' ')) lengthOfCommand = msg.Content.IndexOf(' ');
            else lengthOfCommand = msg.Content.Length;
            command = msg.Content.Substring(1, lengthOfCommand - 1).ToLower();
            input = msg.Content.Substring(lengthOfCommand - 1, msg.Content.Length).ToLower();
        }

        private async Task ConnectToChannelAndPlaySound(string soundPath)
        {
            IAudioClient connection = await ConnectToChannel();
            await PlaySound(soundPath, connection);
            await DisconnectFromChannel();
        }

        private async Task PlaySound(string soundPath, IAudioClient connection)
        {
            await mediaPlayer.GetAndPlaySound(connection, soundPath);
            await Task.Delay(1);
        }

        private async Task DisconnectFromChannel()
        {
            await voiceChannel.DisconnectAsync();
        }

        private async Task<IAudioClient> ConnectToChannel()
        {
            guild = client.GetGuild(guildId);
            voiceChannel = guild.GetVoiceChannel(channelId);
            var connection = await voiceChannel.ConnectAsync();
            return connection;
        }
    }
}
