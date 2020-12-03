using RetroCore.Network.Dispatcher;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RetroCore.Network.Frames.Game
{
    public class Misc
    {
        [PacketId("Bp")]
        public Task SendPing(Client Client, string packet) => Task.Run(() => Client.Network.SendPacket($"Bp{Client.Network.Ping.GetAveragePing()}|{Client.Network.Ping.GetTotalPings()}|50"));

    }
}
