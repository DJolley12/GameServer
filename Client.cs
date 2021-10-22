using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Numerics;

namespace GameServer
{
    public class Client
    {
        public static int DataBufferSize = 4096;
        public int Id { get; set; }
        public Player CPlayer { get; set; }
        public List<IEnvironmentObject> EnvironmentObjects { get; set; }
        public TCP Tcp { get; set; }
        public UDP Udp { get; set; }

        public Client(int id)
        {
            Id = id;
            Tcp = new TCP(Id);
            Udp = new UDP(Id);
        }

        public class TCP
        {
            public TcpClient Socket;

            private readonly int Id;
            private NetworkStream stream { get; set; }
            private Packet receivedPacket { get; set; }
            private byte[] receiveBuffer { get; set; }

            public TCP(int _id)
            {
                Id = _id;
            }

            public void Connect(TcpClient socket)
            {
                Socket = socket;
                Socket.ReceiveBufferSize = DataBufferSize;
                Socket.SendBufferSize = DataBufferSize;

                stream = Socket.GetStream();

                receivedPacket = new Packet();
                receiveBuffer = new byte[DataBufferSize];

                stream.BeginRead(receiveBuffer, 0, DataBufferSize, ReceiveCallback, null);

                ServerSend.Welcome(Id, "Welcome to the server!");
            }

            public void SendData(Packet packet)
            {
                try
                {
                    if (Socket != null)
                    {
                        stream.BeginWrite(packet.ToArray(), 0, packet.Length(), null, null);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error sending data to player {Id} via TCP: {ex}");
                }
            }

            private void ReceiveCallback(IAsyncResult result)
            {
                try
                {
                    int byteLength = stream.EndRead(result);
                    if (byteLength <= 0)
                    {
                        Server.Clients[Id].Disconnect();
                        return;
                    }

                    byte[] data = new byte[byteLength];
                    Array.Copy(receiveBuffer, data, byteLength);

                    receivedPacket.Reset(HandleData(data));
                    stream.BeginRead(receiveBuffer, 0, DataBufferSize, ReceiveCallback, null);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error receiving TCP data: {ex}");
                    Server.Clients[Id].Disconnect();
                }
            }

            private bool HandleData(byte[] _data)
            {
                int _packetLength = 0;

                receivedPacket.SetBytes(_data);

                if (receivedPacket.UnreadLength() >= 4)
                {
                    _packetLength = receivedPacket.ReadInt();
                    if (_packetLength <= 0)
                    {
                        return true;
                    }
                }

                while (_packetLength > 0 && _packetLength <= receivedPacket.UnreadLength())
                {
                    byte[] _packetBytes = receivedPacket.ReadBytes(_packetLength);
                    ThreadManager.ExecuteOnMainThread(() =>
                    {
                        using (Packet _packet = new Packet(_packetBytes))
                        {
                            int _packetId = _packet.ReadInt();
                            Server.PacketHandlers[_packetId](Id, _packet);
                        }
                    });

                    _packetLength = 0;
                    if (receivedPacket.UnreadLength() >= 4)
                    {
                        _packetLength = receivedPacket.ReadInt();
                        if (_packetLength <= 0)
                        {
                            return true;
                        }
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
                Socket.Close();
                stream = null;
                receivedPacket = null;
                receiveBuffer = null;
                Socket = null;
            }
        }

        public class UDP
        {
            public IPEndPoint UdpEndPoint { get; set; }
            private int id { get; set; }

            public UDP(int udpId)
            {
                id = udpId;
            }

            public void Connect(IPEndPoint endPoint)
            {
                UdpEndPoint = endPoint;
            }

            public void Disconnect()
            {
                 UdpEndPoint = null;
            }

            public void SendData(Packet packet)
            {
                Server.SendUDPData(UdpEndPoint, packet);
            }

            public void HandleData(Packet packetData)
            {
                int packetLength = packetData.ReadInt();
                byte[] packetBytes = packetData.ReadBytes(packetLength);

                ThreadManager.ExecuteOnMainThread(() =>
                {
                    using (Packet packet = new Packet(packetBytes))
                    {
                        int packetId = packet.ReadInt();
                        Server.PacketHandlers[packetId](id, packet);
                    }
                });
            }
        }

        public void SendIntoGame(string playerName)
        {
            CPlayer = new Player(Id, playerName, new Vector3(0, 0, 0));

            foreach (Client client in Server.Clients.Values)
            {
                if (client.CPlayer != null)
                {
                    if (client.Id != Id)
                    {
                        ServerSend.SpawnPlayer(Id, client.CPlayer);
                    }
                }
            }

            foreach (Client client in Server.Clients.Values)
            {
                if (client.CPlayer != null)
                {
                    ServerSend.SpawnPlayer(client.Id, CPlayer);
                }
            }
        }

        public void Disconnect()
        {
            Console.WriteLine($"{Tcp.Socket.Client.RemoteEndPoint} has disconnected.");

            CPlayer = null;

            Tcp.Disconnect();
            Udp.Disconnect();
        }
    }
}
