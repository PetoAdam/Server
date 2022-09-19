using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using System.Linq;

namespace DummyServer
{
    class ServerHandle
    {
        public static void WelcomeReceived(int _fromClient, Packet _packet)
        {
            int _clientIdCheck = _packet.ReadInt();
            string _username = _packet.ReadString();
            int _password = _packet.ReadInt();
            int _loginMode = _packet.ReadInt();

            Console.WriteLine($"{Server.clients[_fromClient].tcp.socket.Client.RemoteEndPoint} connected successfully and is now logging in as player {_fromClient} with username {_username} and password {_password}.");
            if (_fromClient != _clientIdCheck)
            {
                Console.WriteLine($"Player \"{_username}\" (ID: {_fromClient}) has assumed the wrong client ID ({_clientIdCheck})!");
                return;
            }

            //TODO check username and password -- create and send packet - if login is unsuccessful dc after
            if (_loginMode == 1)
            {
                if (Server.playerDatabase.Login(new Player(_username, _password, _fromClient), _fromClient))
                {

                    Console.WriteLine($"Player {_username} logged in.");
                    ServerSend.Login(_fromClient, "Success");

                    //TODO send addition data of player (rank, MMR etc.)
                }
                else
                {
                    Console.WriteLine("Incorrect username or password.");
                    ServerSend.Login(_fromClient, "Incorrect username or password.");
                }
            }
            if (_loginMode == 2)
            {
                if (Server.playerDatabase.Register(new Player(_username, _password, _fromClient), _fromClient))
                {
                    Console.WriteLine($"Player {_username} signed up and logged in.");
                    ServerSend.Login(_fromClient, "Success");
                }
                else
                {
                    Console.WriteLine("Signup unsuccessful. Username already taken.");
                    ServerSend.Login(_fromClient, "Signup unsuccessful. Username already taken.");
                }
            }
        }

        public static void UDPTestReceive(int _fromClient, Packet _packet)
        {
            string _msg = _packet.ReadString();

            Console.WriteLine(_msg);
        }

        public static void SearchingMatch1v1(int _fromClient, Packet _packet)
        {
            Server.matchmaking1v1.players.Add(Server.playerDatabase.GetPlayerById(_fromClient));
        }
    }
}
