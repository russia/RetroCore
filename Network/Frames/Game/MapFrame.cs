using RetroCore.Manager.MapManager;
using RetroCore.Network.Dispatcher;
using System.Threading.Tasks;

namespace RetroCore.Network.Frames.Game
{
    internal class MapFrame : FrameTemplate
    {
        [PacketId("GDM")]
        public Task GetMapData(Client Client, string packet) => Task.Run(async () =>
        {
            await Client.Network.SendPacket("GI");
            Client.MapManager.UpdateMapData(packet);
        });

        [PacketId("GM")]
        public Task GetGameMovment(Client client, string packet) => Task.Factory.StartNew(async () =>
        {
            Map map = client.MapManager;
            string[] EntitySplitter = packet.Substring(3).Split('|'), informations;
            string templateName, type;
            int id;

            while (!map.Map_updated)
                await Task.Delay(100);

            foreach (string entity in EntitySplitter)
            {
                if (entity.Length < 1)
                    continue;

                if (entity[0].Equals('+'))
                {
                    informations = entity.Substring(1).Split(';');
                    Cell cell = map.GetCellById(short.Parse(informations[0]));
                    id = int.Parse(informations[3]);
                    templateName = informations[4];
                    type = informations[5];

                    if (type.Contains(","))
                        type = type.Split(',')[0];

                    switch (int.Parse(type))
                    {
                        case -1://Creature
                        case -2://Monsters

                            break;

                        case -3://Monsters Group

                            break;

                        case -4://NPC'S

                            break;

                        case -5:// Merchants

                            break;

                        case -6:// Percepteur

                            break;

                        case -7:
                        case -8:
                        case -9:
                        case -10:
                            break;

                        default:// player
                            if (client.isFighting)
                            {
                                int vitality = int.Parse(informations[14]);
                                byte pa = byte.Parse(informations[15]);
                                byte pm = byte.Parse(informations[16]);
                                byte equips = byte.Parse(informations[24]);
                            }
                            else
                            {
                                if (client.CharacterId == id)
                                {
                                    client.MapManager.CurrentCell = cell;
                                }
                            }
                            break;
                    }
                }

                if (entity[0].Equals('-'))
                {
                    if (!client.isFighting)
                    {
                        id = int.Parse(entity.Substring(1));
                        map.Entities.TryRemove(id, out Entity entityout);
                    }
                }
            }
        }, TaskCreationOptions.LongRunning);
    }
}