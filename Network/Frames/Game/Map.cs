using RetroCore.Network.Dispatcher;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RetroCore.Network.Frames.Game
{
    class Map : FrameTemplate
    {
        [PacketId("GDM")]
        public Task get_Nuevo_Mapa(Client Client, string packet) => Task.Run(async () =>
        {
            await Client.Network.SendPacket("GI");
        });
    }
}
