using System;
using System.Collections.Generic;
using System.Text;

namespace DummyServer
{
    public class Lobby
    {

        private List<Player> players = new List<Player>();
        public Player leader { get; set; }

        public Lobby (Player _leader)
        {
            leader = _leader;
            AddPlayer(_leader);
        }

        public void AddPlayer(Player _player)
        {
            players.Add(_player);
        }

        public void RemovePlayer(Player _player)
        {
            if (players.Contains(_player))
            {
                players.Remove(_player);
            }
        }

        public List<Player> GetPlayers()
        {
            return players;
        }

        public bool IsPlayerInLobby(Player _player)
        {
            foreach(Player p in players)
            {
                if(p == _player)
                {
                    return true;
                }
            }

            return false;
        }

        public string GetLobbyData()
        {
            string playernames = String.Empty;
            foreach (Player p in GetPlayers())
            {
                playernames += p.username;
                if (p.username != GetPlayers()[GetPlayers().Count - 1].username)
                {
                    playernames += ",";
                }
            }
            playernames += ";";
            playernames += leader.username;
            return playernames;
        }

        public int GetAverageElo()
        {
            int averageElo = 0;
            foreach(Player p in GetPlayers())
            {
                averageElo += p.elo;
            }
            return averageElo / GetPlayers().Count;
        }
    }
}
