using System;

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

        public static void EnvironmentObject(int fromClient, Packet packet)
        {
            var objId = packet.ReadInt();
            var position = packet.ReadVector3();
            var rotation = packet.ReadQuaternion();
            var velocity = packet.ReadVector3();
            var torque = packet.ReadFloat();
            var typeString = packet.ReadString();

            if (objId < 0)
            {
                // TODO: add to EnvironmentManager list and send to clients
                EnvironmentManager.AddNewEnvObject(objId, fromClient, position, rotation, velocity, torque, typeString);
            }
            else
            {
                EnvironmentManager.EnvironmentObjects[objId].SetValues(position, rotation, velocity, torque, typeString);
            }
        }

        public static void Animation(int fromClient, Packet packet)
        {
            ServerSend.Animation(fromClient, packet);
        }
    }
}
