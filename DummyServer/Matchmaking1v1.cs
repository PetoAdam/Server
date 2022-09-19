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

        public void RemovePlayer(Player player)
        {
            if (players.Contains(player))
            {
                players.Remove(player);
            }
        }

        public void CallAfterDelay()
        {
            while (true)
            {
                lock (this)
                {
                    Search();
                }
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

                    long ping1 = 10000;
                    long ping2 = 10000;

                    if(player1.isLoggedIn && player2.isLoggedIn)
                    {
                        ping1 = PingPlayer(player1);
                        ping2 = PingPlayer(player2);
                    }
                    

                    var host = ping1 < ping2 ? player1 : player2;

                    matches.Add(new Match1v1() {player1 = player1, player2=player2, host = "" }); //Add Docker ip + port
                }
            }
            foreach(Match1v1 m in matches)
            {
                players.Remove(m.player1);
                players.Remove(m.player2);
            }
            new Thread(StartMatches).Start();
        }

        private void StartMatches()
        {
            foreach(Match1v1 m in matches)
            {
                Server.match1v1Database.matches.Add(m);
                ServerSend.StartMatch1v1(m.player1.id, m.host);
                ServerSend.StartMatch1v1(m.player2.id, m.host);
                //Server.lobbyDatabase.RemoveLobby(Server.lobbyDatabase.FindLobbyWithPlayer(m.player1));
                //Server.lobbyDatabase.RemoveLobby(Server.lobbyDatabase.FindLobbyWithPlayer(m.player2));
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