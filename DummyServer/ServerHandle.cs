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
            if(invitee == null)
            {
                Console.WriteLine("No such player to invite");
                //TODO send notification that no such player exist
                return;
            }
            if(!invitee.isLoggedIn)
            {
                Console.WriteLine("Player not online or not given id");
                //TODO send notification that player not online or not given id
                return;
            }
            Console.WriteLine("Invitation sent");
            ServerSend.InviteToLobby(invitee.id, inviter.username);
        }

        internal static void LeaveLobby(int _fromClient, Packet _packet)
        {
            Player client = Server.playerDatabase.GetPlayerById(_fromClient);
            Lobby lobby = Server.lobbyDatabase.FindLobbyWithPlayer(client);
            if(lobby != null)
            {
                if(lobby.leader.username == client.username)
                {
                    foreach(Player p in lobby.GetPlayers())
                    {
                        if(p.username != client.username)
                        {
                            ServerSend.LeaveLobby(p.id, "The leader has left the lobby");
                        }
                    }
                    Server.lobbyDatabase.RemoveLobby(lobby);
                }
                else
                {
                    Server.lobbyDatabase.FindLobbyWithPlayer(client).RemovePlayer(client);
                    lobby.RemovePlayer(client);
                    foreach (Player p in lobby.GetPlayers())
                    {
                          ServerSend.LeaveLobby(p.id, lobby.GetLobbyData());
                    }
                }
            }
        }

        public static void JoinLobby(int _fromClient, Packet _packet)
        {
            Player invitee = Server.playerDatabase.GetPlayerById(_fromClient);
            Player inviter = Server.playerDatabase.GetPlayerByName(_packet.ReadString());
            Lobby lobby = Server.lobbyDatabase.FindLobbyWithPlayer(inviter);
            Lobby initialLobby = Server.lobbyDatabase.FindLobbyWithPlayer(invitee);
            if (initialLobby != null)
            {
                LeaveLobby(_fromClient, _packet);
            }
            if(lobby != null && lobby.GetPlayers().Count < 3)
            {
                Console.WriteLine("Join successful");
                lobby.AddPlayer(invitee);
                string playernames = lobby.GetLobbyData();
                foreach(Player p in lobby.GetPlayers())
                {

                    ServerSend.JoinedLobby(p.id, playernames);
                }
            }
            else
            {
                Console.WriteLine("Join unsuccessful");
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
