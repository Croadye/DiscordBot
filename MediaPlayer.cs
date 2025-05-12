using Discord.Audio;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace WelcomeBot
{
    class MediaPlayer
    {
        Media sounds;
        UserFavorites assignGreeting;

        public MediaPlayer()
        {
            sounds = new Media();
            assignGreeting = new UserFavorites();
            PopulateSounds();
        }

        public async Task SendAsync(IAudioClient client, string name)
        {

            PopulateSounds();
            try
            {
                string greeting = assignGreeting.GetGreeting(name);
                if (greeting != null)
                {
                    await GetAndPlaySound(client, greeting);
                } else
                {
                    string path = sounds.GetRandSound();
                    await GetAndPlaySound(client, path);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine($"- {ex.StackTrace}");
            }


        }

        public async Task GetAndPlaySound(IAudioClient client, string path)
        {
            Console.WriteLine($"Playing Sound: " + path);
            var output = CreateStream(path).StandardOutput.BaseStream;
            var discord = client.CreatePCMStream(AudioApplication.Voice, 90000, 200);
            await output.CopyToAsync(discord);
            Console.WriteLine($"Sound Played");
        }

        private Process CreateStream(string path)
        {
            return Process.Start(new ProcessStartInfo
            {
                FileName = "ffmpeg",
                Arguments = $"-hide_banner -loglevel panic -i \"{path}\" -ac 2 -f s16le -ar 48000 pipe:1",
                UseShellExecute = false,
                RedirectStandardOutput = true,
            });
        }

        public string[] GetSoundArray()
        {
            PopulateSounds();
            return sounds.GetSoundArray();
        }
        private void PopulateSounds()
        {
            sounds.PopulateSoundArray("sounds");
        }

        public void AssignGreeting(string name, string sound)
        {
            assignGreeting.AddGreeting(name, sound);
        }

        public void SetDefaultGreeting(string name)
        {
            assignGreeting.RemoveGreeting(name);
        }

        public string[] GetFavorites(string name)
        {
           return assignGreeting.GetFavorites(name);
        }

        public void RemoveGreeting(string name, string sound)
        {
            assignGreeting.RemoveGreeting(name, sound);
        }

    }
}
