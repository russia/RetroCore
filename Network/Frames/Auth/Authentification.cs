using RetroCore.Helpers;
using RetroCore.Network.Dispatcher;
using System;
using System.Linq;

namespace RetroCore.Network.Frames.Auth
{
    public class Authentification : FrameTemplate
    {
        [PacketId("HC")]
        public void Connection(Client client, string packet)
        {
            var key = packet.Substring(2, 32);
            StringHelper.WriteLine($"[{client.Username}:{client.Password}] got key : {key}");
            client.Network.SendPacket(Constants.GameVersion);
            client.Network.SendPacket(client.Username + "\n" + StringHelper.Encrypt(client.Password, key));
            client.Network.SendPacket("Af");
        }

        [PacketId("AQ")]
        public void AuthentificationSuccess(Client client, string packet)
        {
            client.Network.SendPacket("Ax");
        }

        [PacketId("Af")]
        public void AuthentificationQueue(Client client, string packet)
        {
            packet = PacketsReceiver.GetPacketContent(packet);
            StringHelper.WriteLine($"Queue position {packet.Split('|')[0]}/{packet.Split('|')[1]}");
        }

        [PacketId("M013")]
        public void WrongSocketAdress(Client client, string packet)
        {
            StringHelper.WriteLine($"[{client.Username}:{client.Password}] servers are in maintenance..", ConsoleColor.Yellow);
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
                    reason = "WRONG_PASSWORD"; // user/pwd are wrong
                    break;
            }
            StringHelper.WriteLine($"[{client.Username}:{client.Password}] can't login, reason : {reason}", ConsoleColor.Red);
            client.Network.Dispose();
        }

        [PacketId("AxK")]
        public void ServerList(Client client, string packet)
        {
            var rawPacket = packet.Substring(3).Split('|').Distinct().ToList();
            var subscriptionEndDate = long.Parse(rawPacket[0]);
            string str = subscriptionEndDate != 0 ? "and subscribed" : "";
            StringHelper.WriteLine($"Account {client.Username} working {str}", ConsoleColor.Blue);
        }
    }
}