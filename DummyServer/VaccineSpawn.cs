using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace DummyServer
{
    public class VaccineSpawn
    {
        private List<Vector3> spawnPos = new List<Vector3>();
        public Vector3 finalPos = new Vector3();
        public VaccineSpawn()
        {
            spawnPos.Add(new Vector3(546.9634f, 100.8886f, 566.5417f));
            spawnPos.Add(new Vector3(512.203f, 100.8886f, 554.606f));
            spawnPos.Add(new Vector3(547.94f, 102.41f, 553.61f));
            spawnPos.Add(new Vector3(483.03f, 102.41f, 578.69f));
            spawnPos.Add(new Vector3(486.59f, 101.33f, 561.84f));
            spawnPos.Add(new Vector3(532.48f, 101.33f, 544.09f));
            spawnPos.Add(new Vector3(551.6f, 101.33f, 542.92f));
            spawnPos.Add(new Vector3(514.133f, 106.71f, 536.83f));
            spawnPos.Add(new Vector3(520.376f, 101.42f, 509.316f));
            spawnPos.Add(new Vector3(497f, 101.42f, 509.316f));

            Random rnd = new Random();
            int index = rnd.Next(0, 9);
            finalPos = spawnPos[index];
        }
    }
}
