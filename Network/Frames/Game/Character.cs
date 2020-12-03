using RetroCore.Network.Dispatcher;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RetroCore.Network.Frames.Game
{
    public class Character : FrameTemplate
    {
        [PacketId("As")]
        public Task get_Stats_Actualizados(Client Client, string packet) => Task.Run(async () =>
        {
            await Client.Network.SendPacket("BD");
            await Client.Network.SendPacket("Ir742;556;0");//sending screen info
            //this.aks.send("Ir" + Stage.width + ";" + Stage.height + ";" + _loc3_);
        });
    }
}
