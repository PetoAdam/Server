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

        public static void OnPlayerMovement(int id, Packet packet)
        {
            SendUDPData(id, packet);
        }

        public static void OnPlayerMovementResponse(int id, Packet packet)
        {
            SendUDPData(id, packet);
        }
    }
}
