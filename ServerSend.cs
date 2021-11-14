using System;

namespace GameServer
{
    public class ServerSend
    {
        private static void SendTCPData(int toClientId, Packet packet)
        {
            packet.WriteLength();
            Server.Clients[toClientId].Tcp.SendData(packet);
        }

        private static void SendTCPDataToAll(Packet packet)
        {
            packet.WriteLength();
            for (int i = 1; i <= Server.MaxPlayers; i++)
            {
                Server.Clients[i].Tcp.SendData(packet);
            }
        }
        
        private static void SendTCPDataToAll(int exceptClient, Packet packet)
        {
            packet.WriteLength();

            for (int i = 1; i <=Server.MaxPlayers; i++)
            {
                if (i != exceptClient)
                {
                    Server.Clients[i].Tcp.SendData(packet);
                }
            }
        }

        private static void SendUDPData(int toClientId, Packet packet)
        {
            packet.WriteLength();
            Server.Clients[toClientId].Udp.SendData(packet);
        }
        
        private static void SendUDPDataToAll(Packet packet)
        {
            packet.WriteLength();
            for (int i = 1; i <= Server.MaxPlayers; i++)
            {
                Server.Clients[i].Udp.SendData(packet);
            }
        }
        
        private static void SendUDPDataToAll(int exceptClient, Packet packet)
        {
            packet.WriteLength();

            for (int i = 1; i <=Server.MaxPlayers; i++)
            {
                if (i != exceptClient)
                {
                    Server.Clients[i].Udp.SendData(packet);
                }
            }
        }

        public static void Welcome(int toClient, string msg)
        {
            using (Packet packet = new Packet((int)ServerPackets.Welcome))
            {
                packet.Write(msg);
                packet.Write(toClient);

                SendTCPData(toClient, packet);
            }
        }

        internal static void SpawnPlayer(int toClientId, Player player)
        {
            using (Packet packet = new Packet((int)ServerPackets.SpawnPlayer))
            {
                packet.Write(player.Id);
                packet.Write(player.Username);
                packet.Write(player.Position);
                packet.Write(player.Rotation);

                SendTCPData(toClientId, packet);
            }
        }

        public static void PlayerPosition(Player player)
        {
            using (Packet packet = new Packet((int)ServerPackets.PlayerRotation))
            {
                packet.Write(player.Id);
                packet.Write(player.Position);

                SendUDPDataToAll(player.Id, packet);
            }
        }

        public static void PlayerRotation(Player player)
        {
            using (Packet packet = new Packet((int)ServerPackets.PlayerRotation))
            {
                packet.Write(player.Id);
                packet.Write(player.Rotation);

                SendUDPDataToAll(player.Id, packet);
            }
        }

        public static void EnvironmentObjectPosition(IEnvironmentObject envObject)
        {
            using (Packet packet = new Packet((int)ServerPackets.EnvironmentObjectPosition))
            {
                packet.Write(envObject.Id);
                packet.Write(envObject.Position);

                SendUDPDataToAll(packet);
            }
        }

        public static void EnvironmentObjectRotation(IEnvironmentObject envObject)
        {
            using (Packet packet = new Packet((int)ServerPackets.EnvironmentObjectRotation))
            {
                packet.Write(envObject.Id);
                packet.Write(envObject.Position);

                SendUDPDataToAll(packet);
            }
        }

        public static void EnvironmentObject(int toClientId, IEnvironmentObject envObject)
        {
            using (Packet packet = new Packet((int)ServerPackets.EnvironmentObject))
            {
                packet.Write(envObject.Id);
                packet.Write(envObject.OwnerId);
                packet.Write(envObject.Position);
                packet.Write(envObject.Rotation);
                packet.Write(envObject.Velocity);
                packet.Write(envObject.Torque);
                packet.Write(envObject.GetType().ToString());

                for (int i = 0; i < envObject.SubscribedClientIds.Count; i++)
                {
                    SendUDPData(envObject.SubscribedClientIds[i], packet);
                }
            }
        }
    }
}
