using System;
using System.Collections.Generic;
using System.Text;

namespace GameServer
{
    public class ServerHandle
    {
        public static void WelcomeReceived(int fromClient, Packet packet)
        {
            int clientIdCheck = packet.ReadInt();
            string username = packet.ReadString();
            
            Console.WriteLine($"{Server.Clients[fromClient].Tcp.Socket.Client.RemoteEndPoint} connected successfully and is now player {fromClient}.");
            if (fromClient != clientIdCheck)
            {
                Console.WriteLine($"Player \"{username}\" (ID: {fromClient}) has assumed the wrong client ID({clientIdCheck})!");
            }
            Server.Clients[fromClient].SendIntoGame(username);
        }

        public static void PlayerPosition(int fromClient, Packet packet)
        {
            var position = packet.ReadVector3();
        }

        public static void PlayerMovement(int fromClient, Packet packet)
        {
            bool[] inputs = new bool[packet.ReadInt()];

            for (int i = 0; i < inputs.Length; i++)
            {
                inputs[i] = packet.ReadBool();
            }

            var position = packet.ReadVector3();

            var rotation = packet.ReadQuaternion();

            Server.Clients[fromClient].CPlayer.SetInput(inputs, rotation, position);
        }
    }
}
