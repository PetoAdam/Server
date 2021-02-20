using System;
using System.Collections.Generic;
using System.Text;

namespace DummyServer
{
    public class LobbyDatabase
    {
        private List<Lobby> lobbies = new List<Lobby>();

        public void AddLobby(Lobby _lobby)
        {
            lobbies.Add(_lobby);
        }

        public void RemoveLobby(Lobby _lobby)
        {
            if (lobbies.Contains(_lobby))
            {
                lobbies.Remove(_lobby);
            }
        }

        public Lobby FindLobbyWithPlayer(Player _player)
        {
            foreach(Lobby l in lobbies)
            {
                if (l.IsPlayerInLobby(_player))
                {
                    return l;
                }             
            }            return null;
        }
    }
}
