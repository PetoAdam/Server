using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace DummyServer
{
    public class PlayerDatabase
    {
        public List<Player> players = new List<Player>();
        //private string path = @"C:\Users\Papp Pál\source\repos\Server\database2.txt";
        //private string path = @"E:\database.txt";
        private string path = @"C:\Users\Hallgato\source\repos\Server\database2.txt";

        public PlayerDatabase()
        {
            string[] lines = File.ReadAllLines(path);
            foreach(string line in lines)
            {
                var listStrLineElements = line.Split(',');
                players.Add(new Player(){ username = listStrLineElements[0], password = Int32.Parse(listStrLineElements[1]), elo = Int32.Parse(listStrLineElements[2]) });
            }
        }

        public void SavePlayers()
        {
            File.WriteAllText(path, String.Empty);
            foreach (Player player in players)
            {
                using (StreamWriter sw = File.AppendText(path))
                {
                    sw.WriteLine(player.username + "," + player.password + "," + player.elo);
                }
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
                sw.WriteLine(player.username + "," + player.password + "," + player.elo /* <- 1000 */);
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
