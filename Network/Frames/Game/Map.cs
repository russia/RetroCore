﻿using RetroCore.Helpers.MapsReader;
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
        public Task GetMapData(Client Client, string packet) => Task.Run(async () =>
        {
            Client.MapManager.UpdateMapData(packet);
            await Client.Network.SendPacket("GI");                   
            //await Client.Network.SendPacket("BYI");                   
        });
    }
}