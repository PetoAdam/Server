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
        public static Matchmaking1v1 matchmaking1v1;
        public static int Port { get; private set; }
        public static Dictionary<int, Client> clients = new Dictionary<int, Client>();
        public delegate void PacketHandler(int _fromClient, Packet _packet);
        public static Dictionary<int, PacketHandler> packetHandlers;
        public static MatchDatabase matchDatabase;
        public static Match1v1Database match1v1Database;
        private static TcpListener tcpListener;
        private static UdpClient udpListener;

        public static void Start(int _maxPlayers, int _port)
        {
            MaxPlayers = _maxPlayers;
            Port = _port;

            Console.WriteLine("Starting server...");
            InitializeServerData();

            tcpListener = new TcpListener(IPAddress.Any, Port);
            tcpListener.Start();
            tcpListener.BeginAcceptTcpClient(new AsyncCallback(TCPConnectCallback), null);

            udpListener = new UdpClient(Port);
            udpListener.BeginReceive(UDPReceiveCallback, null);

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

        private static void UDPReceiveCallback(IAsyncResult _result)
        {
            try
            {
                IPEndPoint _clientEndPoint = new IPEndPoint(IPAddress.Any, 0);
                byte[] _data = udpListener.EndReceive(_result, ref _clientEndPoint);
                udpListener.BeginReceive(UDPReceiveCallback, null);

                if(_data.Length < 4)
                {
                    return;
                }

                using (Packet _packet = new Packet(_data))
                {
                    int _clientId = _packet.ReadInt();

                    if(_clientId == 0)
                    {
                        return;
                    }

                    if(clients[_clientId].udp.endPoint == null)
                    {
                        clients[_clientId].udp.Connect(_clientEndPoint);
                        return;
                    }

                    if(clients[_clientId].udp.endPoint.ToString() == _clientEndPoint.ToString())
                    {
                        clients[_clientId].udp.HandleData(_packet);
                    }
                }
            }
            catch(Exception _ex)
            {
                Console.WriteLine($"Error receiving UDP data: {_ex}");
            }
        }

        public static void SendUDPData(IPEndPoint _clientEndPoint, Packet _packet)
        {
            try
            {
                if(_clientEndPoint != null)
                {
                    udpListener.BeginSend(_packet.ToArray(), _packet.Length(), _clientEndPoint, null, null);
                }
            }
            catch(Exception _ex)
            {
                Console.WriteLine($"Error sending data to {_clientEndPoint} via UDP: {_ex}");
            }
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
                { (int)ClientPackets.searchingMatch, ServerHandle.SearchingMatch },
                { (int)ClientPackets.udpTestReceive, ServerHandle.UDPTestReceive },
                { (int)ClientPackets.sceneLoaded, ServerHandle.OnReadyButtonClicked},
                { (int)ClientPackets.sendIntoGame, ServerHandle.OnSendIntoGame},
                { (int)ClientPackets.searching1v1Match, ServerHandle.Searching1v1Match},
                { (int)ClientPackets.sceneLoaded1v1, ServerHandle.OnReadyButtonClicked1v1},
                { (int)ClientPackets.sendIntoGame1v1, ServerHandle.OnSendIntoGame1v1},
                { (int)ClientPackets.playerMovement, ServerHandle.OnPlayerMovement},
                { (int)ClientPackets.onPlayerMovementResponse, ServerHandle.OnPlayerMovementResponse},
                { (int)ClientPackets.shooting, ServerHandle.OnShooting},
                { (int)ClientPackets.onDying, ServerHandle.OnDying},
                { (int)ClientPackets.onRoundEnd, ServerHandle.OnRoundEnd},


            };
            Console.WriteLine("Initialized packets.");

            playerDatabase = new PlayerDatabase();
            lobbyDatabase = new LobbyDatabase();
            matchmaking = new Matchmaking();
            matchmaking1v1 = new Matchmaking1v1();
            matchDatabase = new MatchDatabase();
            match1v1Database = new Match1v1Database();
            Thread matchmakingThread = new Thread(matchmaking.CallAfterDelay);
            matchmakingThread.Start();
            Thread matchmaking1v1Thread = new Thread(matchmaking1v1.CallAfterDelay);
            matchmaking1v1Thread.Start();


        }
    }
}
