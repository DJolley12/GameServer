using System;
using System.Net;
using System.Net.Sockets;

namespace GameServer
{
    public class Client
    {
        public static int DataBufferSize = 4096;
        public int Id;
        public TCP Tcp;

        public Client(int id)
        {
            Id = id;
            Tcp = new TCP(Id);
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
                        // TODO: disconnect
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
                    // TODO: disconnect
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
                            Server.packetHandlers[_packetId](Id, _packet);
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
        }
    }
}
