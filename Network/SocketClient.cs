using Org.Mentalis.Network.ProxySocket;
using RetroCore.Helpers;
using RetroCore.Manager;
using RetroCore.Network.Dispatcher;
using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RetroCore.Network
{
    public class SocketClient : IDisposable
    {
        public ProxySocket Socket { get; set; }
        public byte[] Buffer { get; set; } // buffer
        private readonly SemaphoreSlim Semaphore;
        private const int BufferSize = 16384;
        public bool isDisposed = false;
        private readonly Client Client;
        public PingManager Ping;

        public SocketClient(Client client)
        {
            this.Semaphore = new SemaphoreSlim(1);
            this.Client = client;
            Connection(Constants.AuthAddress, Constants.AuthPort);
            this.Ping = new PingManager();
        }

        public void Connection(string ip, int port)
        {
            Disconnect();
            this.Buffer = new byte[BufferSize];
            this.Socket = new ProxySocket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            var result = this.Socket.BeginConnect(new IPEndPoint(IPAddress.Parse(ip), port), null, this.Socket).AsyncWaitHandle.WaitOne(3000, true);

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
                    //Disconnect();
                    return;
                }

                if (bytesReceived > 0)
                {
                    var data = Encoding.UTF8.GetString(this.Buffer, 0, bytesReceived);

                    //if (Program.Debug)
                    //{
                    //    string finalStr = "";
                    //    foreach (var value in this.Buffer)
                    //    {
                    //        if (value == 0)
                    //        {
                    //            StringHelper.WriteLine(finalStr, ConsoleColor.Gray);
                    //            break;
                    //        }
                    //        finalStr += value + " ";
                    //    }
                    //}
                    Ping.Update();

                    foreach (var packet in data.Replace("\x0a", string.Empty).Split('\x00').Where(x => x != ""))
                    {
                        StringHelper.WriteLine($"→ RCV {packet}");
                        PacketsReceiver.Receive(this.Client, packet);
                    }
                }
                Receive();
            }
            catch (Exception ex)
            {
                StringHelper.WriteLine($"X Error {ex.Message}", ConsoleColor.DarkRed);
            }
        }

        public async Task SendPacket(string packet)
        {
            if (!this.Socket.Connected)
                return;
            await Semaphore.WaitAsync().ConfigureAwait(false); //awaiting current tasks to finish
            this.Socket.Send(Encoding.UTF8.GetBytes(string.Format("{0}\n\x00", packet)));
            StringHelper.WriteLine($"← SND {packet.Replace("\n", "[lineReturn]")}", ConsoleColor.Cyan);
            Semaphore.Release(); // releasing semaphore
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

        public void Dispose()
        {
        }
    }
}