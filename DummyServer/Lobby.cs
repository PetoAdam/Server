using System;
using System.Collections.Generic;
using System.Text;

namespace DummyServer
{
    public class Lobby
    {
        private List<Player> players;
        private Player leader;

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
    }
}
