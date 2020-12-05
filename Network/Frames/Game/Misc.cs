using RetroCore.Network.Dispatcher;
using System.Threading.Tasks;

namespace RetroCore.Network.Frames.Game
{
    public class Misc
    {
        [PacketId("Bp")]
        public Task SendPing(Client Client, string packet) => Task.Run(() => Client.Network.SendPacket($"Bp{Client.Network.Ping.GetAveragePing()}|{Client.Network.Ping.GetTotalPings()}|50"));

        [PacketId("As")]
        public Task GetStats(Client Client, string packet) => Task.Run(async () =>
        {
            await Client.Network.SendPacket("BD");
            await Client.Network.SendPacket("Ir742;556;0");//sending screen info
            Client.OnCharacterConnectionFinished();
        });
    }
}