using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Threading;

namespace DummyServer
{
    public class Matchmaking1v1
    {
        public List<Match1v1> matches = new List<Match1v1>();
        public List<Player> players = new List<Player>();

        public Matchmaking1v1()
        {

        }

        public void CallAfterDelay()
        {
            while (true)
            {
                Search();
                Thread.Sleep(10000);
            }
        }

        public void Search()
        {
            List<Player> orderedPlayers = players.OrderByDescending(o => o.elo).ToList();
            for (int i = 0; i < orderedPlayers.Count; i++)
            {
                if (i % 2 == 0 && i !=orderedPlayers.Count-1)
                {
                    var player1 = orderedPlayers[i];
                    var player2 = orderedPlayers[i+1];
                    var ping1 = PingPlayer(player1);
                    var ping2 = PingPlayer(player2);

                    var host = ping1 < ping2 ? player1 : player2;

                    matches.Add(new Match1v1() {player1 = player1, player2=player2, host = host });
                }
            }
            foreach(Match1v1 m in matches)
            {
                players.Remove(m.player1);
                players.Remove(m.player2);
            }
            StartMatches();
        }

        private void StartMatches()
        {
            foreach(Match1v1 m in matches)
            {
                Server.match1v1Database.matches.Add(m);
                ServerSend.StartMatch1v1(m.player1.id, m.host.username + ":1");
                ServerSend.StartMatch1v1(m.player2.id, m.host.username + ":2");
            }
            matches.Clear();
        }

        private long PingPlayer(Player p)
        {
            string remoteEndPoint = Server.clients[p.id].tcp.socket.Client.RemoteEndPoint.ToString();
            string ipAddress = remoteEndPoint.Split(':')[0];
            Ping sendPing = new Ping();
            PingReply reply = sendPing.Send(ipAddress, 1000);
            if (reply != null)
            {
                return reply.RoundtripTime;
            }
            else
            {
                return 100000;
            }
        }
    }
}