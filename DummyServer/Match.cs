using System;
using System.Collections.Generic;
using System.Text;

namespace DummyServer
{
    public class Match
    {

        public List<Lobby> team1 = new List<Lobby>();
        public List<Lobby> team2 = new List<Lobby>();

        public Match()
        {

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
