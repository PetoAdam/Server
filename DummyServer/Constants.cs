using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace DummyServer
{
    class Constants
    {
        public const int TICKS_PER_SEC = 30;
        public const int MS_PER_TICK = 1000 / TICKS_PER_SEC;
        public const int TEAM_SIZE = 3;
        public static List<Vector3> team1Spawns = new List<Vector3>() { new Vector3(507.56f, 100.06f, 508.86f), new Vector3(542.41f, 100.06f, 571.46f), new Vector3(477.45f, 100.06f, 563.06f) };
        public static List<Vector3> team2Spawns = new List<Vector3>() { new Vector3(505.02f, 105.279f, 526.6f), new Vector3(497f, 105.279f, 552.13f), new Vector3(484.64f, 100.959f, 546.73f) };
    }
}
