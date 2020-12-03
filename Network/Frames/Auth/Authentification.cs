using RetroCore.Helpers;
using RetroCore.Network.Dispatcher;
using RetroCore.Others;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace RetroCore.Network.Frames.Auth
{
    public class Authentification : FrameTemplate
    {
        [PacketId("HC")]
        public Task Connection(Client client, string packet) => Task.Run(async () =>
        {
            var key = PacketsReceiver.GetPacketContent(packet);
            StringHelper.WriteLine($"[{client.Username}:{client.Password}] got key : {key}");
            await client.Network.SendPacket(Constants.GameVersion);
            await client.Network.SendPacket(client.Username + "\n" + Hash.Encrypt(client.Password, key));
            await client.Network.SendPacket("Af");
        });

        [PacketId("AQ")]
        public Task AuthentificationSecretQuestion(Client client, string packet) => Task.Run(async () =>
        {
            await client.Network.SendPacket($"Ap{Constants.AuthPort}");

            await client.Network.SendPacket($"Ai{StringHelper.GetRandomNetworkKey()}");
           
            await client.Network.SendPacket("Ax");

        });

        [PacketId("Af")]
        public void AuthentificationQueue(Client client, string packet)
        {
            packet = PacketsReceiver.GetPacketContent(packet);
            StringHelper.WriteLine($"Queue position {packet.Split('|')[0]}/{packet.Split('|')[1]}");
        }

        [PacketId("M01")]
        public void Inactivity(Client client, string packet)
        {
            packet = PacketsReceiver.GetPacketContent(packet);
            StringHelper.WriteLine($"Disconnected for inactivity..",ConsoleColor.Red);
        }

        [PacketId("AH")]
        public Task GetServersStates(Client client, string packet) => Task.Run(async () =>
        {
            string[] availableServersDatas = PacketsReceiver.GetPacketContent(packet).Substring(2).Split('|');


            foreach(string servDatas in availableServersDatas)
            {
                string[] splitter = servDatas.Split(';');
                int id = int.Parse(splitter[0]);
                ServerStatus state = (ServerStatus)byte.Parse(splitter[1]);
            }
        });

        [PacketId("M013")]
        public void WrongSocketAdress(Client client, string packet)
        {
            StringHelper.WriteLine($"[{client.Username}:{client.Password}] servers are in maintenance..", ConsoleColor.Yellow);
        }
        [PacketId("Ad")]
        public void AuthentificationNickname(Client client, string packet)
        {
            StringHelper.WriteLine($"[Auth] Account {client.Username} is connected !", ConsoleColor.Blue);
        }
        [PacketId("AlE")]
        public void AuthentificationFailed(Client client, string packet)
        {
            var error = PacketsReceiver.GetPacketContent(packet);
            string reason = "unknown";
            string errorbis = error.Substring(0, 1);
            switch (errorbis)
            {
                case "n":
                    reason = "CONNECT_NOT_FINISHED";
                    break;

                case "a":
                case "c":
                case "d":
                    reason = "ALREADY_LOGGED";
                    client.Network.Dispose();
                    break;

                case "k":
                    reason = "TEMP_BANNED";
                    break;

                case "v":
                    reason = "BAD_VERSION";
                    Constants.GameVersion = error.Replace("v", "");
                    break;

                case "p":
                    reason = "NOT_PLAYER";
                    break;

                case "b":
                    reason = "BANNED";
                    break;

                case "w":
                    reason = "SERVER_FULL";
                    client.Network.Dispose();
                    break;

                case "o":
                    reason = "OLD_ACCOUNT";
                    break;

                case "e":
                    reason = "OLD_ACCOUNT_USE_NEW";
                    break;

                case "m":
                    reason = "MAINTAIN_ACCOUNT";
                    break;

                case "f":
                    reason = "WRONG_PASSWORD";
                    break;
            }
            StringHelper.WriteLine($"[{client.Username}:{client.Password}] can't login, reason : {reason}", ConsoleColor.Red);
            client.Network.Dispose();
        }

        [PacketId("AxK")]
        public Task ServerList(Client client, string packet) => Task.Run(async () =>
        {
            var rawPacket = PacketsReceiver.GetPacketContent(packet).Split('|').Distinct().ToList();
            DateTime subEndDate = StringHelper.UnixTimeStampToDateTime(long.Parse(rawPacket[0]));
            //StringHelper.WriteLine($"[{client.Username}:{client.Password}] is subscribed, end date : {subEndDate.Day}/{subEndDate.Month}/{subEndDate.Year}", ConsoleColor.Green);
             await client.Network.SendPacket($"AX624");
         
        });

        [PacketId("AXK")]
        public Task GetServerAdress(Client client, string packet) => Task.Run(async () =>
        {
            client.GameTicket = packet.Substring(14);
            string ip = Hash.GetIp(packet.Substring(3, 8));
            int port = Hash.GetPort(packet.Substring(11, 3).ToCharArray());
            StringHelper.WriteLine($"[{client.Username}:{client.Password}] is switching to game : {ip}:{port}", ConsoleColor.Green);
            client.Network.Connection(ip, port);
        });
    }
}//172.65.206.193", 443)

