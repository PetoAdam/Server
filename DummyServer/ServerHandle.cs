﻿using System;
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


            //TODO bind player to clientID
            //TODO: wait for player to start matchmaking then send player into game
        }

        public static void InviteToLobby(int _fromClient, Packet _packet)
        {
            Player inviter = Server.playerDatabase.GetPlayerById(_fromClient);
            Player invitee = Server.playerDatabase.GetPlayerByName(_packet.ReadString());
            if (invitee == null)
            {
                Console.WriteLine("No such player to invite");
                //TODO send notification that no such player exist
                return;
            }
            if (!invitee.isLoggedIn)
            {
                Console.WriteLine("Player not online or not given id");
                //TODO send notification that player not online or not given id
                return;
            }
            Console.WriteLine("Invitation sent");
            ServerSend.InviteToLobby(invitee.id, inviter.username);
        }

        public static void SearchingMatch(int _fromClient, Packet _packet)
        {
            Lobby lobby = Server.lobbyDatabase.FindLobbyWithPlayer(Server.playerDatabase.GetPlayerById(_fromClient));
            Server.matchmaking.lobbies.Add(lobby);
            foreach (Player p in lobby.GetPlayers())
            {
                if (_fromClient != p.id)
                {
                    ServerSend.SearchingMatch(p.id, String.Empty);
                }
            }
        }

        internal static void LeaveLobby(int _fromClient, Packet _packet)
        {
            Player client = Server.playerDatabase.GetPlayerById(_fromClient);
            Lobby lobby = Server.lobbyDatabase.FindLobbyWithPlayer(client);
            if (lobby != null)
            {
                if (lobby.leader.username == client.username)
                {
                    foreach (Player p in lobby.GetPlayers())
                    {
                        if (p.username != client.username)
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
            if (lobby != null && lobby.GetPlayers().Count < 3)
            {
                Console.WriteLine("Join successful");
                lobby.AddPlayer(invitee);
                string playernames = lobby.GetLobbyData();
                foreach (Player p in lobby.GetPlayers())
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

        public static void PlaySound(int _fromClient, Packet _packet)
        {
            string name = _packet.ReadString();
            Vector3 position = _packet.ReadVector3();

            Match1v1 m = Server.match1v1Database.GetMatchByPlayer(Server.playerDatabase.GetPlayerById(_fromClient));
            Match m2 = Server.matchDatabase.GetMatchByPlayer(Server.playerDatabase.GetPlayerById(_fromClient));
            if (m != null)
            {
                ServerSend.PlaySound(m.player1.id, name, position);
                ServerSend.PlaySound(m.player2.id, name, position);
            }
            else if (m2 != null)
            {
                foreach (Lobby l in m2.team1)
                {
                    foreach (Player p in l.GetPlayers())
                    {
                        ServerSend.PlaySound(p.id, name, position);
                    }
                }

                foreach (Lobby l in m2.team2)
                {
                    foreach (Player p in l.GetPlayers())
                    {
                        ServerSend.PlaySound(p.id, name, position);
                    }
                }
            }
        }

        public static void OnPlayerNotReady(int _fromClient, Packet _packet)
        {
            Match1v1 m = Server.match1v1Database.GetMatchByPlayer(Server.playerDatabase.GetPlayerById(_fromClient));
            Match m2 = Server.matchDatabase.GetMatchByPlayer(Server.playerDatabase.GetPlayerById(_fromClient));
            if (m != null)
            {
                if (m.player1.isLoggedIn)
                {
                    ServerSend.OnPlayerNotReady(m.player1.id);
                }

                if (m.player2.isLoggedIn)
                {
                    ServerSend.OnPlayerNotReady(m.player2.id);
                }
               
            }
            else if (m2 != null)
            {
                foreach (Lobby l in m2.team1)
                {
                    foreach (Player p in l.GetPlayers())
                    {
                        if (p.isLoggedIn)
                        {
                            ServerSend.OnPlayerNotReady(p.id);
                        }
                    }
                }

                foreach (Lobby l in m2.team2)
                {
                    foreach (Player p in l.GetPlayers())
                    {
                        if (p.isLoggedIn)
                        {
                            ServerSend.OnPlayerNotReady(p.id);
                        }
                    }
                }
            }
        }

        public static void EndOfGame(int _fromClient, Packet _packet)
        {
            int team0wins = _packet.ReadInt(); 
            int team1wins = _packet.ReadInt();
            bool result = team0wins > team1wins;
            Match1v1 m = Server.match1v1Database.GetMatchByPlayer(Server.playerDatabase.GetPlayerById(_fromClient));
            Match m2 = Server.matchDatabase.GetMatchByPlayer(Server.playerDatabase.GetPlayerById(_fromClient));
            if (m!= null)
            {
                List<int> newElos = EloSystem.CalculateEloRating(m.player1.elo, m.player2.elo, result);
                m.player1.elo = newElos[0];
                m.player2.elo = newElos[1];
                ServerSend.SendEndOfGame(m.player1.id, team0wins, team1wins, m.player1.elo);
                ServerSend.SendEndOfGame(m.player2.id, team0wins, team1wins, m.player2.elo);
                Server.match1v1Database.matches.Remove(m);
            }
            else if(m2 != null)
            {
                float team0average = m2.GetAverageEloOfTeam(0);
                float team1average = m2.GetAverageEloOfTeam(1);
                List<int> newElos = EloSystem.CalculateEloRating(team0average, team1average, result);
                foreach(Player p in m2.GetTeam(0))
                {
                    p.elo += newElos[0] - (int)team0average;
                    ServerSend.SendEndOfGame(p.id, team0wins, team1wins, p.elo);
                }
                foreach (Player p in m2.GetTeam(1))
                {
                    p.elo += newElos[1] - (int)team1average;
                    ServerSend.SendEndOfGame(p.id, team0wins, team1wins, p.elo);
                }
                Server.matchDatabase.matches.Remove(m2);
            }

        }

        public static void OnNextRound(int _fromClient, Packet _packet)
        {
            Vector3 vaccinePos = _packet.ReadVector3();
            Vector3 petePos = _packet.ReadVector3();
            float peteRot = _packet.ReadFloat();

            Match1v1 m = Server.match1v1Database.GetMatchByPlayer(Server.playerDatabase.GetPlayerById(_fromClient));
            Match m2 = Server.matchDatabase.GetMatchByPlayer(Server.playerDatabase.GetPlayerById(_fromClient));
            if (m != null)
            {
                ServerSend.OnNextRound(m.player1.id, vaccinePos, petePos, peteRot);
                ServerSend.OnNextRound(m.player2.id, vaccinePos, petePos, peteRot);
            }
            else if (m2 != null)
            {
                foreach (Lobby l in m2.team1)
                {
                    foreach (Player p in l.GetPlayers())
                    {
                        ServerSend.OnNextRound(p.id, vaccinePos, petePos, peteRot);
                    }
                }

                foreach (Lobby l in m2.team2)
                {
                    foreach (Player p in l.GetPlayers())
                    {
                        ServerSend.OnNextRound(p.id, vaccinePos, petePos, peteRot);
                    }
                }
            }
        }

        public static void OnRoundEnd(int _fromClient, Packet _packet)
        {
            int team0wins = _packet.ReadInt();
            int team1wins = _packet.ReadInt();

            Match1v1 m = Server.match1v1Database.GetMatchByPlayer(Server.playerDatabase.GetPlayerById(_fromClient));
            Match m2 = Server.matchDatabase.GetMatchByPlayer(Server.playerDatabase.GetPlayerById(_fromClient));
            if (m != null)
            {
                ServerSend.OnRoundEnd(m.player1.id, team0wins, team1wins);
                ServerSend.OnRoundEnd(m.player2.id, team0wins, team1wins);
            }
            else if (m2 != null)
            {
                foreach (Lobby l in m2.team1)
                {
                    foreach (Player p in l.GetPlayers())
                    {
                        ServerSend.OnRoundEnd(p.id, team0wins, team1wins);
                    }
                }

                foreach (Lobby l in m2.team2)
                {
                    foreach (Player p in l.GetPlayers())
                    {
                        ServerSend.OnRoundEnd(p.id, team0wins, team1wins);
                    }
                }
            }
        }

        public static void OnDying(int _fromClient, Packet _packet)
        {
            string username = _packet.ReadString();
            bool hasVaccine = _packet.ReadBool();
            Vector3 position = Vector3.Zero;
            if (hasVaccine)
            {
                position = _packet.ReadVector3();
            }

            Match1v1 m = Server.match1v1Database.GetMatchByUsername(username);
            Match m2 = Server.matchDatabase.GetMatchByPlayer(Server.playerDatabase.GetPlayerById(_fromClient));
            if(m != null)
            {
                ServerSend.OnDying(m.player1.id, username, hasVaccine, position);
                ServerSend.OnDying(m.player2.id, username, hasVaccine, position);
            }
            else if (m2 != null)
            {
                foreach (Lobby l in m2.team1)
                {
                    foreach (Player p in l.GetPlayers())
                    {
                        ServerSend.OnDying(p.id, username, hasVaccine, position);
                    }
                }

                foreach (Lobby l in m2.team2)
                {
                    foreach (Player p in l.GetPlayers())
                    {
                        ServerSend.OnDying(p.id, username, hasVaccine, position);
                    }
                }
            }



        }

        public static void OnShooting(int _fromClient, Packet _packet)
        {
            string hitUsername = String.Empty;
            float health = 0;
            Vector3 point = _packet.ReadVector3();
            Vector3 normal = _packet.ReadVector3();
            string username = _packet.ReadString();
            bool isTargetHit = _packet.ReadBool();
            if (isTargetHit)
            {
                hitUsername = _packet.ReadString();
                health = _packet.ReadFloat();
            }

            Match1v1 m = Server.match1v1Database.GetMatchByPlayer(Server.playerDatabase.GetPlayerById(_fromClient));
            if (m != null)
            {
                ServerSend.OnShooting(m.player1.id, point, normal, username, isTargetHit, hitUsername, health);
                ServerSend.OnShooting(m.player2.id, point, normal, username, isTargetHit, hitUsername, health);
            }

            Match m2 = Server.matchDatabase.GetMatchByPlayer(Server.playerDatabase.GetPlayerById(_fromClient));
            if (m2 != null)
            {
                foreach (Lobby l in m2.team1)
                {
                    foreach (Player p in l.GetPlayers())
                    {
                        ServerSend.OnShooting(p.id, point, normal, username, isTargetHit, hitUsername, health);
                    }
                }

                foreach (Lobby l in m2.team2)
                {
                    foreach (Player p in l.GetPlayers())
                    {
                        ServerSend.OnShooting(p.id, point, normal, username, isTargetHit, hitUsername, health);
                    }
                }
            }
        }

        public static void OnPlayerMovement(int _fromClient, Packet _packet)
        {
            bool[] _inputs = new bool[_packet.ReadInt()];
            for (int i = 0; i < _inputs.Length; i++)
            {
                _inputs[i] = _packet.ReadBool();
            }
            Quaternion _rotation = _packet.ReadQuaternion();
            string _username = _packet.ReadString();
            float _xRot = _packet.ReadFloat();
            Vector3 _camPos = _packet.ReadVector3();
            Vector3 _camForward = _packet.ReadVector3();
            Match1v1 m = Server.match1v1Database.GetMatchByPlayer(Server.playerDatabase.GetPlayerById(_fromClient));
            if(m != null)
            {
                ServerSend.OnPlayerMovement(m.host.id, _inputs, _rotation, _username, _xRot, _camPos, _camForward);
            }

            Match m2 = Server.matchDatabase.GetMatchByPlayer(Server.playerDatabase.GetPlayerById(_fromClient));
            if (m2 != null)
            {
                ServerSend.OnPlayerMovement(m2.host.id, _inputs, _rotation, _username, _xRot, _camPos, _camForward);
            }

        }

        public static void OnPlayerMovementResponse(int _fromClient, Packet _packet)
        {
            string username = _packet.ReadString();
            Vector3 position = _packet.ReadVector3();
            Quaternion rotation = _packet.ReadQuaternion();
            float xRot = _packet.ReadFloat();
            int state = _packet.ReadInt();
            bool isSprinting = _packet.ReadBool();
            int ammoCount = _packet.ReadInt();
            int pressEText = _packet.ReadInt();
            bool startTimer = _packet.ReadBool();
            int invokeFunctionId = _packet.ReadInt();
            float timeRemaining = _packet.ReadFloat();
            Match1v1 m = Server.match1v1Database.GetMatchByPlayer(Server.playerDatabase.GetPlayerById(_fromClient));
            if (m != null)
            {
                ServerSend.OnPlayerMovementResponse(m.player1.id, username, position, rotation, xRot, state, isSprinting, ammoCount, pressEText, startTimer, invokeFunctionId, timeRemaining);
                ServerSend.OnPlayerMovementResponse(m.player2.id, username, position, rotation, xRot, state, isSprinting, ammoCount, pressEText, startTimer, invokeFunctionId, timeRemaining);
            }

            Match m2 = Server.matchDatabase.GetMatchByPlayer(Server.playerDatabase.GetPlayerById(_fromClient));
            if (m2 != null)
            {
                foreach (Lobby l in m2.team1)
                {
                    foreach(Player p in l.GetPlayers())
                    {
                        ServerSend.OnPlayerMovementResponse(p.id, username, position, rotation, xRot, state, isSprinting, ammoCount, pressEText, startTimer, invokeFunctionId, timeRemaining);
                    }
                }

                foreach (Lobby l in m2.team2)
                {
                    foreach (Player p in l.GetPlayers())
                    {
                        ServerSend.OnPlayerMovementResponse(p.id, username, position, rotation, xRot, state, isSprinting, ammoCount, pressEText, startTimer, invokeFunctionId, timeRemaining);
                    }
                }
            }

        }

        public static void CreateLobby(int _fromClient, Packet _packet)
        {
            Player leader = Server.playerDatabase.GetPlayerById(_fromClient);
            Server.lobbyDatabase.AddLobby(new Lobby(leader));
            Console.WriteLine("Lobby created");
        }

        public static void UDPTestReceive(int _fromClient, Packet _packet)
        {
            string _msg = _packet.ReadString();

            Console.WriteLine(_msg);
        }

        public static void OnSendIntoGame(int _fromClient, Packet _packet)
        {
            VaccineSpawn v = new VaccineSpawn();
            SickSpawn s = new SickSpawn();
            Match m = Server.matchDatabase.GetMatchByPlayer(Server.playerDatabase.GetPlayerById(_fromClient));
            List<Vector3> spawns = new List<Vector3>(Constants.team1Spawns);
            List<SpawnInfo> spawnInfo = new List<SpawnInfo>();

            foreach (Lobby l in m.team1)
            {
                foreach (Player p in l.GetPlayers())
                {
                    int team = 0;
                    int n = new Random().Next(0, spawns.Count - 1);
                    Vector3 spawn = spawns[n];
                    spawns.RemoveAt(n);
                    spawnInfo.Add(new SpawnInfo(spawn, team, p.username));
                }
            }

            spawns = new List<Vector3>(Constants.team2Spawns);
            foreach (Lobby l in m.team2)
            {
                foreach (Player p in l.GetPlayers())
                {
                    int team = 1;
                    int n = new Random().Next(0, spawns.Count - 1);
                    Vector3 spawn = spawns[n];
                    spawns.RemoveAt(n);
                    spawnInfo.Add(new SpawnInfo(spawn, team, p.username));
                }
            }

            foreach (Lobby l in m.team1)
            {
                foreach (Player p in l.GetPlayers())
                {
                    ServerSend.SpawnPlayer(p.id, spawnInfo, s, v);
                }
            }


            foreach (Lobby l in m.team2)
            {
                foreach (Player p in l.GetPlayers())
                {
                    ServerSend.SpawnPlayer(p.id, spawnInfo, s, v);
                }
            }
        }

        public static void OnReadyButtonClicked(int _fromClient, Packet _packet)
        {
            string hostname = _packet.ReadString();
            if (Server.playerDatabase.GetPlayerByName(hostname).isLoggedIn)
            {
                ServerSend.OnReadyButtonClicked(Server.playerDatabase.GetPlayerByName(hostname).id);
            }
            else
            {
                Match m2 = Server.matchDatabase.GetMatchByPlayer(Server.playerDatabase.GetPlayerById(_fromClient));
                if (m2 != null)
                {
                    foreach (Lobby l in m2.team1)
                    {
                        foreach (Player p in l.GetPlayers())
                        {
                            ServerSend.OnPlayerNotReady(p.id);
                        }
                    }

                    foreach (Lobby l in m2.team2)
                    {
                        foreach (Player p in l.GetPlayers())
                        {
                            ServerSend.OnPlayerNotReady(p.id);
                        }
                    }
                }
            }
        }

        public static void Searching1v1Match(int _fromClient, Packet _packet)
        {
            Server.matchmaking1v1.players.Add(Server.playerDatabase.GetPlayerById(_fromClient));
        }

        public static void OnReadyButtonClicked1v1(int _fromClient, Packet _packet)
        {
            string hostname = _packet.ReadString();
            if (Server.playerDatabase.GetPlayerByName(hostname).isLoggedIn)
            {
                ServerSend.OnReadyButtonClicked1v1(Server.playerDatabase.GetPlayerByName(hostname).id);
            }
            else
            {
                Match1v1 m = Server.match1v1Database.GetMatchByPlayer(Server.playerDatabase.GetPlayerById(_fromClient));
                if (m != null)
                {
                    ServerSend.OnPlayerNotReady(m.player1.id);
                    ServerSend.OnPlayerNotReady(m.player2.id);
                }
            }
        }

        public static void OnSendIntoGame1v1(int _fromClient, Packet _packet)
        {
            Match1v1 m = Server.match1v1Database.GetMatchByPlayer(Server.playerDatabase.GetPlayerById(_fromClient));
            List<Vector3> spawns = new List<Vector3>(Constants.team1Spawns);
            List<SpawnInfo> spawnInfo = new List<SpawnInfo>();

            int team = 0;
            int n = new Random().Next(0, spawns.Count - 1);
            Vector3 spawn = spawns[n];
            spawns.RemoveAt(n);
            spawnInfo.Add(new SpawnInfo(spawn, team, m.player1.username));

            spawns = new List<Vector3>(Constants.team2Spawns);
            team = 1;
            n = new Random().Next(0, spawns.Count - 1);
            spawn = spawns[n];
            spawns.RemoveAt(n);
            spawnInfo.Add(new SpawnInfo(spawn, team, m.player2.username));

            VaccineSpawn v = new VaccineSpawn();
            SickSpawn s = new SickSpawn();

            ServerSend.SpawnPlayer1v1(m.player1.id, spawnInfo, s, v);

            ServerSend.SpawnPlayer1v1(m.player2.id, spawnInfo, s ,v);

        }
    }
}
