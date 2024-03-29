﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;

namespace DummyServer
{
    class Client
    {
        public static int dataBufferSize = 4096;

        public int id;
        public TCP tcp;
        public UDP udp;

        public string username;
        public string password;
       
        public Client(int _clientId)
        {
            id = _clientId;
            tcp = new TCP(id);
            udp = new UDP(id);
        }

        public class TCP
        {
            public TcpClient socket;
            private readonly int id;
            private NetworkStream stream;
            private Packet receivedData;
            private byte[] receiveBuffer;

            public TCP(int _id)
            {
                id = _id;
            }

            public void Connect(TcpClient _socket)
            {
                socket = _socket;
                socket.ReceiveBufferSize = dataBufferSize;
                socket.SendBufferSize = dataBufferSize;

                stream = socket.GetStream();

                receivedData = new Packet();
                receiveBuffer = new byte[dataBufferSize];

                stream.BeginRead(receiveBuffer, 0, dataBufferSize, ReceiveCallback, null);

                ServerSend.Welcome(id, "Welcome to the server!");
            }

            public void SendData(Packet _packet)
            {
                try
                {
                    if(socket != null)
                    {
                        stream.BeginWrite(_packet.ToArray(), 0, _packet.Length(), null, null);
                    }
                }
                catch(Exception _ex)
                {
                    Console.WriteLine($"Error sending data to player {id} via TCP: {_ex}");
                }
            }

            private void ReceiveCallback(IAsyncResult _result)
            {
                try
                {
                    int _byteLength = stream.EndRead(_result);
                    if(_byteLength <= 0)
                    {
                        Server.clients[id].Disconnect();
                        return;
                    }

                    byte[] _data = new byte[_byteLength];
                    Array.Copy(receiveBuffer, _data, _byteLength);

                    receivedData.Reset(HandleData(_data));

                    stream.BeginRead(receiveBuffer, 0, dataBufferSize, ReceiveCallback, null);
                }
                catch(Exception _ex)
                {
                    Console.WriteLine($"Error receiving TCP data: {_ex}");
                    Server.clients[id].Disconnect();
                }
            }

            private bool HandleData(byte[] _data)
            {
                int _packetLength = 0;

                receivedData.SetBytes(_data);

                if (receivedData.UnreadLength() >= 4)
                {
                    _packetLength = receivedData.ReadInt();
                    if (_packetLength <= 0)
                    {
                        return true;
                    }
                }

                while (_packetLength > 0 && _packetLength <= receivedData.UnreadLength())
                {
                    byte[] _packetBytes = receivedData.ReadBytes(_packetLength);
                    ThreadManager.ExecuteOnMainThread(() =>
                    {
                        using (Packet _packet = new Packet(_packetBytes))
                        {
                            int _packetId = _packet.ReadInt();
                            Server.packetHandlers[_packetId](id, _packet);
                        }
                    });

                    _packetLength = 0;
                    if (_packetLength <= 0)
                    {
                        return true;
                    }
                }

                if (_packetLength <= 1)
                {
                    return true;
                }
                return false;
            }


            public void Disconnect()
            {
                socket.Close();
                stream = null;
                receivedData = null;
                receiveBuffer = null;
                socket = null;
            }
        }

        public class UDP
        {
            public IPEndPoint endPoint;
            private int id;

            public UDP(int _id)
            {
                id = _id;
            }

            public void Connect(IPEndPoint _endPoint)
            {
                endPoint = _endPoint;
                ServerSend.UDPTest(id);
            }

            public void SendData(Packet _packet)
            {
                Server.SendUDPData(endPoint, _packet);
            }

            public void HandleData(Packet _packetData)
            {
                int _packetLength = _packetData.ReadInt();
                byte[] _packetBytes = _packetData.ReadBytes(_packetLength);

                ThreadManager.ExecuteOnMainThread(() =>
                {
                    using (Packet _packet = new Packet(_packetBytes))
                    {
                        int _packetId = _packet.ReadInt();
                        Server.packetHandlers[_packetId](id, _packet);
                    }
                });
            }

            public void Disconnect()
            {
                endPoint = null;
            }
        }

        private void Disconnect()
        {
            Console.WriteLine($"{tcp.socket.Client.RemoteEndPoint} has disconnected.");

            Player player = Server.playerDatabase.GetPlayerById(id);
            player.isLoggedIn = false;


            Lobby l = Server.matchmaking.FindLobbyWithPlayer(player);
            if (l != null)
            {
                lock (Server.matchmaking)
                {
                    Server.matchmaking.lobbies.Remove(l);
                }
                foreach(Player p in l.GetPlayers())
                {
                    if(player != p)
                    {
                        ServerSend.DisconnectInMatchmaking(p.id);
                    }
                }
            }

            lock (Server.matchmaking1v1)
            {
                Server.matchmaking1v1.RemovePlayer(player);
            }
            Server.lobbyDatabase.RemoveLobby(l);


            Match m = Server.matchDatabase.GetMatchByPlayer(player);
            Match1v1 m1 = Server.match1v1Database.GetMatchByPlayer(player);
            if(m != null)
            {
                player.elo -= 20;
                foreach (Player p in m.GetTeam(0))
                {
                    ServerSend.OnPlayerDisconnect(p.id, p.elo, player.username);
                }
            }
            if(m1 != null)
            {
                player.elo -= 20;
                ServerSend.OnPlayerDisconnect(m1.player1.id, m1.player1.elo, player.username);
                ServerSend.OnPlayerDisconnect(m1.player2.id, m1.player2.elo, player.username);
            }

            Lobby lobby = Server.lobbyDatabase.FindLobbyWithPlayer(player);
            if (lobby != null)
            {
                if (lobby.leader.username == player.username)
                {
                    foreach (Player p in lobby.GetPlayers())
                    {
                        if (p.username != player.username)
                        {
                            ServerSend.LeaveLobby(p.id, "The leader has left the lobby");
                        }
                    }
                    Server.lobbyDatabase.RemoveLobby(lobby);
                }
                else
                {
                    Server.lobbyDatabase.FindLobbyWithPlayer(player).RemovePlayer(player);
                    lobby.RemovePlayer(player);
                    foreach (Player p in lobby.GetPlayers())
                    {
                        ServerSend.LeaveLobby(p.id, lobby.GetLobbyData());
                    }
                }
            }



            tcp.Disconnect();
            udp.Disconnect();
        }
    }
}
