using Org.Mentalis.Network.ProxySocket;
using RetroCore.Helpers;
using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace RetroCore.Network
{
    public class SocketClient
    {
        public ProxySocket Socket { get; set; }
        public byte[] Buffer { get; set; } // buffer

        private const int BufferSize = 16384; // low values -> in cropped packets.

        private Client Client;

        public SocketClient(Client client)
        {
            this.Client = client;
            this.Buffer = new byte[BufferSize];
            this.Socket = new ProxySocket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            var result = this.Socket.BeginConnect(new IPEndPoint(Constants.AuthAddress, Constants.AuthPort), null, this.Socket).AsyncWaitHandle.WaitOne(3000, true);

            if (!result)
            {
                StringHelper.WriteLine("X Error while SocketClient connection..", ConsoleColor.Red);
                return;
            }
            StringHelper.WriteLine("SocketClient connected !", ConsoleColor.Green);
            Receive();
        }

        protected void Receive()
        {
            if (Socket == null || !Socket.Connected)
                return;

            var socketArgs = new SocketAsyncEventArgs();

            socketArgs.SetBuffer(this.Buffer, 0, this.Buffer.Length);
            socketArgs.UserToken = this;
            socketArgs.Completed += ProcessReceive;

            var willRaiseEvent = Socket.ReceiveAsync(socketArgs);
            if (!willRaiseEvent)
                ProcessReceive(this, socketArgs);
        }

        public void ProcessReceive(object sender, SocketAsyncEventArgs args)
        {
            try
            {
                args.Completed -= ProcessReceive;
                var bytesReceived = args.BytesTransferred;
                if (args.LastOperation != SocketAsyncOperation.Receive || bytesReceived == 0)
                {
                    Disconnect();
                    return;
                }

                if (bytesReceived > 0)
                {
                    var data = Encoding.UTF8.GetString(this.Buffer, 0, bytesReceived);
                    foreach (var packet in data.Replace("\x0a", string.Empty).Split('\x00').Where(x => x != ""))
                    {
                        StringHelper.WriteLine($"→ RCV {packet}");
                        if (packet.StartsWith("HC"))
                        {
                            var key = packet.Substring(2, 32);
                            SendPacket(Constants.GameVersion);
                            SendPacket(Client.Username + "\n" + StringHelper.Encrypt(Client.Password, key));
                            SendPacket("Af");
                        }
                    }
                }
                Receive();
            }
            catch (Exception ex)
            {
                StringHelper.WriteLine($"X Error {ex.Message}", ConsoleColor.DarkRed);
            }
        }

        public void SendPacket(string packet)
        {
            this.Socket.Send(Encoding.UTF8.GetBytes(string.Format("{0}\n\x00", packet)));
            StringHelper.WriteLine($"← SND {packet.Replace("\n", "[lineReturn]")}", ConsoleColor.Cyan);
        }

        private void Disconnect()
        {
            if (Socket != null && Socket.Connected)
            {
                StringHelper.WriteLine($"X Disconnecting SocketClient..", ConsoleColor.Yellow);
                try
                {
                    Socket.Shutdown(SocketShutdown.Both);
                    Socket.Close();
                }
                catch { }
            }
        }
    }
}