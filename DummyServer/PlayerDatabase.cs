using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace DummyServer
{
    class PlayerDatabase
    {
        public List<Player> players = new List<Player>();
        //private string path = @"C:\Users\Papp Pál\Documents\database.txt";
        private string path = @"E:\database.txt";

        public PlayerDatabase()
        {
            string[] lines = File.ReadAllLines(path);
            foreach(string line in lines)
            {
                var listStrLineElements = line.Split(',');
                players.Add(new Player(listStrLineElements[0], Int32.Parse(listStrLineElements[1])));
            }
        }

        public bool Login(Player player, int _id)
        {
            foreach(Player p in players)
            {
                if(p.username == player.username && p.password == player.password)
                {
                    if (p.isLoggedIn)
                    {
                        return false;
                    }
                    else
                    {
                        p.isLoggedIn = true;
                        p.id = _id;
                        return true;
                    }
                }
            }
            return false;
        }

        public bool Register(Player player, int _id)
        {
            foreach (Player p in players)
            {
                if (p.username == player.username)
                {
                    return false;
                }
            }

            players.Add(player);
            using (StreamWriter sw = File.AppendText(path))
            {
                sw.WriteLine(player.username + "," + player.password);
            }
            return true;
        }

        public Player GetPlayerById(int _id)
        {
            foreach(Player p in players)
            {
                if(p.id == _id)
                {
                    return p;
                }
            }

            return null;
        }

        public Player GetPlayerByName(string _name)
        {
            foreach (Player p in players)
            {
                if (p.username == _name)
                {
                    return p;
                }
            }

            return null;
        }

        public void Logout() { } // TODO
    }
}
