using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace DummyServer
{
    class Server
    {
        public static int MaxPlayers { get; private set; }
        public static PlayerDatabase playerDatabase;
        public static LobbyDatabase lobbyDatabase;
        public static Matchmaking matchmaking;
        public static int Port { get; private set; }
        public static Dictionary<int, Client> clients = new Dictionary<int, Client>();
        public delegate void PacketHandler(int _fromClient, Packet _packet);
        public static Dictionary<int, PacketHandler> packetHandlers;
        public static MatchDatabase matchDatabase;
        private static TcpListener tcpListener;

        public static void Start(int _maxPlayers, int _port)
        {
            MaxPlayers = _maxPlayers;
            Port = _port;

            Console.WriteLine("Starting server...");
            InitializeServerData();

            tcpListener = new TcpListener(IPAddress.Any, Port);
            tcpListener.Start();
            tcpListener.BeginAcceptTcpClient(new AsyncCallback(TCPConnectCallback), null);

            Console.WriteLine($"Server started on {Port}.");
        }

        private static void TCPConnectCallback(IAsyncResult _result)
        {
            TcpClient _client = tcpListener.EndAcceptTcpClient(_result);
            tcpListener.BeginAcceptTcpClient(new AsyncCallback(TCPConnectCallback), null);

            Console.WriteLine($"Incoming connection from {_client.Client.RemoteEndPoint}...");

            for (int i = 1; i <= MaxPlayers; i++)
            {
                if(clients[i].tcp.socket == null)
                {
                    clients[i].tcp.Connect(_client);
                    return;
                }
            }

            Console.WriteLine($"{_client.Client.RemoteEndPoint} failed to connect: Server full!");
        }

        private static void InitializeServerData()
        {
            for (int i = 1; i <= MaxPlayers; i++)
            {
                clients.Add(i, new Client(i));
            }

            packetHandlers = new Dictionary<int, PacketHandler>()
            {
                { (int)ClientPackets.welcomeReceived, ServerHandle.WelcomeReceived },
                { (int) ClientPackets.createLobby, ServerHandle.CreateLobby},
                { (int)ClientPackets.inviteToLobby, ServerHandle.InviteToLobby },
                { (int)ClientPackets.joinLobby, ServerHandle.JoinLobby },
                 { (int)ClientPackets.leaveLobby, ServerHandle.LeaveLobby },
                 { (int)ClientPackets.searchingMatch, ServerHandle.SearchingMatch }


            };
            Console.WriteLine("Initialized packets.");

            playerDatabase = new PlayerDatabase();
            lobbyDatabase = new LobbyDatabase();
            matchmaking = new Matchmaking();
            matchDatabase = new MatchDatabase();
            Thread newthread = new Thread(matchmaking.CallAfterDelay);
            newthread.Start();


        }
    }
}
