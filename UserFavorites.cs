using System;
using System.Collections.Generic;
using System.IO;

namespace WelcomeBot
{
    class UserFavorites
    {
        string[] member;
        List<string> greeting;
       
        string path = "favorites";
        public UserFavorites()
        {
            greeting = new List<string>();

            if(!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }


        private void readFile(string path)
        {
            using (StreamReader sr = File.OpenText(path))
            {
                string s = "";
                while ((s = sr.ReadLine()) != null)
                {
                    string sound = s.Substring(0);

                    
                    greeting.Add(sound);
                }
                sr.Close();
            }
        }

        public void AddGreeting(string name, string sound)
        {
            string memberPath = path + "/" + name + ".txt";
            if (!File.Exists(memberPath))
            {
                CreateMemberFile(memberPath);

                greeting.Add(sound);
            }
            else
            {
                readFile(memberPath);
                greeting.Add(sound);
            }
            

            SaveFile(memberPath);
        }

        

        public string GetGreeting(string name)
        {
            string memberPath = path + "/" + name + ".txt";
            string sound = "";
            var rand = new Random();
            if (File.Exists(memberPath))
            {
                readFile(memberPath);
                int x = rand.Next(0, greeting.Count);
                Console.WriteLine(greeting.Count + " " + x);
                sound = greeting[x];
                greeting = new List<string>();
                return sound;
            }
            else
            {
                return null;
            }
        }

        public string[] GetFavorites(String name)
        {
            string memberPath = path + "/" + name + ".txt";
            string[] favorites;
            readFile(memberPath);
            favorites = new string[greeting.Count];
            for (int i = 0; i < favorites.Length; i++)
            {
                favorites[i] = greeting[i];
            }
            SaveFile(memberPath);
            return favorites;

        }

        public void RemoveGreeting(string name)
        {
            string memberPath = path + "/" + name + ".txt";
            if (File.Exists(memberPath))
            {
                SaveFile(memberPath);
            }
            else
            {
                CreateMemberFile(memberPath);
            }
        }

        public void RemoveGreeting(string name, string sound)
        {
            string memberPath = path + "/" + name + ".txt";
            if (File.Exists(memberPath))
            {
                readFile(memberPath);
                if (greeting.Contains(sound))
                {
                    greeting.Remove(sound);
                }
                SaveFile(memberPath);
            }
            else
            {
                CreateMemberFile(memberPath);
            }
        }

        private void SaveFile(String memberPath)
        {
           string[] lines = new string[greeting.Count];
            StreamWriter sw = new StreamWriter(memberPath);
           for (int i = 0; i < greeting.Count; i++)
           {
                sw.WriteLine(greeting[i]);
           }
            sw.Close();
            greeting = new List<string>();
        }

        private static void CreateMemberFile(string memberPath)
        {
            System.IO.FileStream fs = System.IO.File.Create(memberPath);
            fs.Close();
        }
    }
}
