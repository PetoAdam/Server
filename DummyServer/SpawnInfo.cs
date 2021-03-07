using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace DummyServer
{
    public class SpawnInfo
    {
        public Vector3 spawn;
        public int team;
        public string username;

        public SpawnInfo(Vector3 _spawn, int _team, string _username)
        {
            spawn = _spawn;
            team = _team;
            username = _username;
        }
    }
}
