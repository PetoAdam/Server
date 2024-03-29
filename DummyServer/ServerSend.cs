﻿using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace DummyServer
{
    class ServerSend
    {

        private static void SendTCPData(int _toClient, Packet _packet)
        {
            _packet.WriteLength();
            Server.clients[_toClient].tcp.SendData(_packet);
        }

        private static void SendTCPDataToAll(Packet _packet)
        {
            _packet.WriteLength();
            for (int i = 1; i < Server.MaxPlayers; i++)
            {
                Server.clients[i].tcp.SendData(_packet);
            }
        }

        private static void SendTCPDataToAll(int _exceptClient, Packet _packet)
        {
            _packet.WriteLength();
            for (int i = 1; i < Server.MaxPlayers; i++)
            {
                if(i != _exceptClient)
                {
                    Server.clients[i].tcp.SendData(_packet);
                }  
            }
        }

        private static void SendUDPData(int _toClient, Packet _packet)
        {
            _packet.WriteLength();
            Server.clients[_toClient].udp.SendData(_packet);
        }

        private static void SendUDPDataToAll(Packet _packet)
        {
            _packet.WriteLength();
            for (int i = 1; i < Server.MaxPlayers; i++)
            {
                Server.clients[i].udp.SendData(_packet);
            }
        }

        private static void SendUDPDataToAll(int _exceptClient, Packet _packet)
        {
            _packet.WriteLength();
            for (int i = 1; i < Server.MaxPlayers; i++)
            {
                if (i != _exceptClient)
                {
                    Server.clients[i].udp.SendData(_packet);
                }
            }
        }

        public static void StartMatch1v1(int _toClient, string _msg)
        {
            using (Packet _packet = new Packet((int)ServerPackets.startMatch1v1))
            {
                _packet.Write(_msg);
                _packet.Write(_toClient);

                SendTCPData(_toClient, _packet);
            }
        }

        public static void Welcome(int _toClient, string _msg)
        {
            using (Packet _packet = new Packet((int)ServerPackets.welcome))
            {
                _packet.Write(_msg);
                _packet.Write(_toClient);

                SendTCPData(_toClient, _packet);
            }
        }

        public static void Login(int _toClient, string _msg)
        {
            using (Packet _packet = new Packet((int)ServerPackets.login))
            {
                _packet.Write(_msg);
                _packet.Write(_toClient);

                SendTCPData(_toClient, _packet);
            }
        }

        public static void InviteToLobby(int _toClient, string _msg)
        {
            using (Packet _packet = new Packet((int)ServerPackets.inviteToLobby))
            {
                _packet.Write(_msg);
                _packet.Write(_toClient);

                SendTCPData(_toClient, _packet);
            }
        }

        public static void JoinedLobby(int _toClient, string _msg)
        {
            using (Packet _packet = new Packet((int)ServerPackets.joinedLobby))
            {
                _packet.Write(_msg);
                _packet.Write(_toClient);

                SendTCPData(_toClient, _packet);
            }
        }


        public static void CantJoinLobby(int _toClient, string _msg)
        {
            using (Packet _packet = new Packet((int)ServerPackets.cantJoinLobby))
            {
                _packet.Write(_msg);
                _packet.Write(_toClient);

                SendTCPData(_toClient, _packet);
            }
        }

        public static void LeaveLobby(int _toClient, string _msg)
        {
            using (Packet _packet = new Packet((int)ServerPackets.leaveLobby))
            {
                _packet.Write(_msg);
                _packet.Write(_toClient);

                SendTCPData(_toClient, _packet);
            }
        }

        public static void SearchingMatch(int _toClient, string _msg)
        {
            using (Packet _packet = new Packet((int)ServerPackets.searchingMatch))
            {
                _packet.Write(_msg);
                _packet.Write(_toClient);

                SendTCPData(_toClient, _packet);
            }
        }

        public static void StartMatch(int _toClient, string _msg)
        {
            using (Packet _packet = new Packet((int)ServerPackets.startMatch))
            {
                _packet.Write(_msg);
                _packet.Write(_toClient);

                SendTCPData(_toClient, _packet);
            }
        }

        public static void UDPTest(int _toClient)
        {
            using (Packet _packet = new Packet((int)ServerPackets.udpTest))
            {
                _packet.Write("A test packet for udp");

                SendUDPData(_toClient, _packet);
            }
        }

        public static void SpawnPlayer(int _toClient, List<SpawnInfo> info, SickSpawn s, VaccineSpawn v)
        {
            using (Packet _packet = new Packet((int)ServerPackets.spawnPlayer))
            {
                _packet.Write(v.finalPos);
                _packet.Write(s.finalPos);
                _packet.Write(s.finalRotation);
                foreach (SpawnInfo si in info)
                {
                    _packet.Write(si.username);
                    _packet.Write(si.team);
                    _packet.Write(si.spawn);
                }
                _packet.Write(_toClient);

                SendTCPData(_toClient, _packet);
            }
        }

        public static void OnReadyButtonClicked(int _toClient)
        {
            using (Packet _packet = new Packet((int)ServerPackets.sceneLoaded))
            {
                _packet.Write(_toClient);

                SendTCPData(_toClient, _packet);
            }
        }

        public static void OnReadyButtonClicked1v1(int _toClient)
        {
            using (Packet _packet = new Packet((int)ServerPackets.sceneLoaded1v1))
            {
                _packet.Write(_toClient);

                SendTCPData(_toClient, _packet);
            }
        }

        public static void DisconnectInMatchmaking(int _toClient)
        {
            using (Packet _packet = new Packet((int)ServerPackets.disconnectInMatchmaking))
            {
                _packet.Write(_toClient);

                SendTCPData(_toClient, _packet);
            }
        }

        public static void OnPlayerDisconnect(int _toClient, int elo, string username)
        {
            using (Packet _packet = new Packet((int)ServerPackets.onPlayerDisconnect))
            {
                _packet.Write(elo);
                _packet.Write(username);
                _packet.Write(_toClient);

                SendTCPData(_toClient, _packet);
            }
        }

        public static void PlaySound(int _toClient, string name, Vector3 position)
        {
            using (Packet _packet = new Packet((int)ServerPackets.playSound))
            {
                _packet.Write(name);
                _packet.Write(position);
                SendUDPData(_toClient, _packet);
            }
        }

        public static void OnPlayerNotReady(int _toClient)
        {
            using (Packet _packet = new Packet((int)ServerPackets.onPlayerNotReady))
            {
                SendTCPData(_toClient, _packet);
            }
        }

        public static void OnNextRound(int _toClient, Vector3 vaccinePos, Vector3 petePos, float peteRot)
        {
            using (Packet _packet = new Packet((int)ServerPackets.onNextRound))
            {
                _packet.Write(vaccinePos);
                _packet.Write(petePos);
                _packet.Write(peteRot);
                SendTCPData(_toClient, _packet);
            }
        }

        public static void OnRoundEnd(int _toClient, int team0wins, int team1wins)
        {
            using (Packet _packet = new Packet((int)ServerPackets.onRoundEnd))
            {
                _packet.Write(team0wins);
                _packet.Write(team1wins);
                SendTCPData(_toClient, _packet);
            }
        }

        internal static void SendEndOfGame(int id, int team0wins, int team1wins, int elo)
        {
            using (Packet _packet = new Packet((int)ServerPackets.endOfGame))
            {
                _packet.Write(team0wins);
                _packet.Write(team1wins);
                _packet.Write(elo);
                SendTCPData(id, _packet);
            }
        }

        public static void SpawnPlayer1v1(int _toClient, List<SpawnInfo> info, SickSpawn s, VaccineSpawn v)
        {
            using (Packet _packet = new Packet((int)ServerPackets.spawnPlayer1v1))
            {
                _packet.Write(v.finalPos);
                _packet.Write(s.finalPos);
                _packet.Write(s.finalRotation);
                foreach (SpawnInfo si in info)
                {
                    _packet.Write(si.username);
                    _packet.Write(si.team);
                    _packet.Write(si.spawn);
                }
                _packet.Write(_toClient);

                SendTCPData(_toClient, _packet);
            }
        }

        public static void OnDying(int id, string username, bool hasVaccine, Vector3 position)
        {
            using (Packet _packet = new Packet((int)ServerPackets.onDying))
            {
                _packet.Write(username);
                _packet.Write(hasVaccine);
                if (hasVaccine)
                {
                    _packet.Write(position);
                }
                SendUDPData(id, _packet);
            }
        }

        public static void OnPlayerMovement(int id, bool[] _inputs, Quaternion rotation, string username, float _xRot, Vector3 _camPos, Vector3 _camForward)
        {
            using (Packet _packet = new Packet((int)ServerPackets.playerMovement))
            {
                _packet.Write(_inputs.Length);
                foreach (bool _input in _inputs)
                {
                    _packet.Write(_input);
                }
                _packet.Write(rotation);
                _packet.Write(username);
                _packet.Write(_xRot);
                _packet.Write(_camPos);
                _packet.Write(_camForward);
                SendUDPData(id, _packet);
            }
           
        }

        public static void OnShooting(int id, Vector3 point, Vector3 normal, string username, bool isTargetHit, string hitUsername, float health)
        {
            using (Packet _packet = new Packet((int)ServerPackets.shooting))
            {
                _packet.Write(point);
                _packet.Write(normal);
                _packet.Write(username);
                _packet.Write(isTargetHit);
                if (isTargetHit)
                {
                    _packet.Write(hitUsername);
                    _packet.Write(health);
                }
                SendUDPData(id, _packet);
            }
        }

        public static void OnPlayerMovementResponse(int id, string username, Vector3 position, Quaternion rotation, float xRot, int state, bool isSprinting, int ammoCount, int pressEText, bool startTimer, int invokeFunctionId, float timeRemaining)
        {
            using (Packet _packet = new Packet((int)ServerPackets.onPlayerMovementResponse))
            {
                _packet.Write(username);
                _packet.Write(position);
                _packet.Write(rotation);
                _packet.Write(xRot);
                _packet.Write(state);
                _packet.Write(isSprinting);
                _packet.Write(ammoCount);
                _packet.Write(pressEText);
                _packet.Write(startTimer);
                _packet.Write(invokeFunctionId);
                _packet.Write(timeRemaining);
                SendUDPData(id, _packet);
            }
        }
    }
}
