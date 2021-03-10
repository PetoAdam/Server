using System;
using System.Collections.Generic;
using System.Numerics;

namespace DummyServer
{
    public class SickSpawn
    {


        private List<Vector3> spawnPos = new List<Vector3>();
        private List<float> rotation = new List<float>();
        public Vector3 finalPos = new Vector3();
        public float finalRotation;

        public SickSpawn()
        {
            spawnPos.Add(new Vector3(491f, 100.397f, 527.07f));
            spawnPos.Add(new Vector3(482.196f, 101.121f, 536.03f));
            spawnPos.Add(new Vector3(485.7f, 100.552f, 544.9f));
            spawnPos.Add(new Vector3(485.7f, 100.552f, 549.412f));
            spawnPos.Add(new Vector3(500.247f, 100.552f, 550.566f));
            spawnPos.Add(new Vector3(500.247f, 100.552f, 555.194f));
            spawnPos.Add(new Vector3(491.71f, 104.9f, 521.697f));
            spawnPos.Add(new Vector3(496.997f, 104.885f, 521.697f));
            spawnPos.Add(new Vector3(491.878f, 104.885f, 539.548f));
            spawnPos.Add(new Vector3(492.229f, 104.885f, 547.365f));
            spawnPos.Add(new Vector3(489.006f, 104.885f, 551.497f));
            spawnPos.Add(new Vector3(488.782f, 104.885f, 540.383f));

            rotation.Add(0);
            rotation.Add(0);
            rotation.Add(-90);
            rotation.Add(-90);
            rotation.Add(-90);
            rotation.Add(-90);
            rotation.Add(0);
            rotation.Add(0);
            rotation.Add(90);
            rotation.Add(90);
            rotation.Add(-90);
            rotation.Add(-90);

            Random rnd = new Random();
            int index = rnd.Next(0, 11);
            finalPos = spawnPos[index];
            finalRotation = rotation[index];
        }
    }
}
