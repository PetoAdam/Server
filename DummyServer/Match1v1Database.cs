using System.Collections.Generic;

namespace DummyServer
{
    public class Match1v1Database
    {
        public List<Match1v1> matches = new List<Match1v1>();

        public Match1v1 GetMatchByPlayer(Player p)
        {
            foreach(Match1v1 m in matches)
            {
                if (m.player1 == p || m.player2 == p)
                    return m;
            }
            return null;
        }

        public Match1v1 GetMatchByUsername(string p)
        {
            foreach (Match1v1 m in matches)
            {
                if (m.player1.username == p || m.player2.username == p)
                    return m;
            }
            return null;
        }
    }
}