using System;
using System.Collections.Generic;
using System.Text;

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
            if(_fromClient != _clientIdCheck)
            {
                Console.WriteLine($"Player \"{_username}\" (ID: {_fromClient}) has assumed the wrong client ID ({_clientIdCheck})!");
                return;
            }

            //TODO check username and password -- create and send packet - if login is unsuccessful dc after
            if(_loginMode == 1)
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
            if(_loginMode == 2)
            {
                if(Server.playerDatabase.Register(new Player(_username, _password, _fromClient), _fromClient))
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
            

            //TODO bind player to clientID
            //TODO: wait for player to start matchmaking then send player into game
        }

        public static void InviteToLobby(int _fromClient, Packet _packet)
        {
            Player inviter = Server.playerDatabase.GetPlayerById(_fromClient);
            Player invitee = Server.playerDatabase.GetPlayerByName(_packet.ReadString());
            Console.WriteLine("Invitation sent");
            ServerSend.InviteToLobby(invitee.id, inviter.username);
        }

        public static void JoinLobby(int _fromClient, Packet _packet)
        {
            Player invitee = Server.playerDatabase.GetPlayerById(_fromClient);
            Player inviter = Server.playerDatabase.GetPlayerByName(_packet.ReadString());
            Lobby lobby = Server.lobbyDatabase.FindLobbyWithPlayer(inviter);
            if(lobby != null && lobby.GetPlayers().Count < 3)
            {
                lobby.AddPlayer(invitee);
                string playernames = String.Empty;
                foreach(Player p in lobby.GetPlayers())
                {
                    playernames += p.username;
                    playernames += ",";
                }
                foreach(Player p in lobby.GetPlayers())
                {

                    ServerSend.JoinedLobby(p.id, playernames);
                }
            }
            else
            {
                ServerSend.CantJoinLobby(invitee.id, "Error, can't join the lobby");
            }

        }

        public static void CreateLobby(int _fromClient, Packet _packet)
        {
            Player leader = Server.playerDatabase.GetPlayerById(_fromClient);
            Server.lobbyDatabase.AddLobby(new Lobby(leader));
            Console.WriteLine("Lobby created");
        }
    }
}
