using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DummyServer
{
    public class Match
    {

        public List<Lobby> team1 = new List<Lobby>();
        public List<Lobby> team2 = new List<Lobby>();
        public Player host;

        public Match()
        {

        }

        public bool IsPlayerInMatch(Player p)
        {
            foreach(Lobby l in team1)
            {
                if (l.IsPlayerInLobby(p))
                    return true;
            }
            foreach (Lobby l in team2)
            {
                if (l.IsPlayerInLobby(p))
                    return true;
            }
            return false;
        }

        public float GetAverageEloOfTeam(int team)
        {
            List<int> playerelos = new List<int>();
            if (team == 0)
            {

                foreach (Lobby l in team1)
                {
                    foreach(Player p in l.GetPlayers())
                    {
                        playerelos.Add(p.elo);
                    }
                }
            }
            else
            {
                foreach (Lobby l in team2)
                {
                    foreach (Player p in l.GetPlayers())
                    {
                        playerelos.Add(p.elo);
                    }
                }
            }

            return Convert.ToSingle(playerelos.Average());

        }

        public List<Player> GetTeam(int team)
        {
            List<Player> players = new List<Player>();
            if (team == 0)
            {

                foreach (Lobby l in team1)
                {
                    foreach (Player p in l.GetPlayers())
                    {
                        players.Add(p);
                    }
                }
            }
            else
            {
                foreach (Lobby l in team2)
                {
                    foreach (Player p in l.GetPlayers())
                    {
                        players.Add(p);
                    }
                }
            }

            return players;
        }


        //Gives the max players to join one team -> max 3
        public int GetMaxFreePlaces()
        {
            int places1 = 0;
            int places2 = 0;
            foreach(Lobby l in team1)
            {
                places1 += l.GetPlayers().Count;
            }
            foreach (Lobby l in team2)
            {
                places2 += l.GetPlayers().Count;
            }

            return places1 < places2 ? Constants.TEAM_SIZE - places1 : Constants.TEAM_SIZE - places2;
        }
    }
}
