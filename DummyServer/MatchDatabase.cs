using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DummyServer
{
    public class MatchDatabase
    {
        public List<Match> matches = new List<Match>();

        public Match GetMatchByPlayer(Player _player)
        {
            foreach(Match m in matches)
            {
                if (m.IsPlayerInMatch(_player))
                {
                    return m;
                }
            }
            return null;
        }
    }
}
