using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace WelcomeBot
{
    class Media
    {
        string[] listOfSounds;
        int lastPlayed = -1;

        public Media() 
        {
            string path = "sounds";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }

        public string GetRandSound()
        {
            return GenerateRandomSound();
        }

        private string GenerateRandomSound()
        {
            var rand = new Random();
            int randNum = rand.Next(listOfSounds.Length);
            if (randNum != lastPlayed)
            {
                lastPlayed = randNum;
                return listOfSounds[randNum];
            } else
            {
                if (randNum == (listOfSounds.Length - 1)) randNum -= 1;
                else randNum += 1;
                lastPlayed = randNum;
                return listOfSounds[randNum];
            }
        }

        public void PopulateSoundArray(String path)
        {
            listOfSounds = Directory.GetFiles(path);
        }

        public String[] GetSoundArray()
        {
            return listOfSounds;
        }
    }
}
