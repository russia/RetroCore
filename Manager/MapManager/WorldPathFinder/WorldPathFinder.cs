using RetroCore.Helpers;
using RetroCore.Helpers.MapsReader;
using RetroCore.Helpers.MapsReader.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace RetroCore.Manager.MapManager.WorldPathFinder
{
    public class WorldPathFinder
    {
        private Client _client;
        private bool isEnabled { get; } = DataManager.GamePathFound;
        private Map currentMap => _client.MapManager;
        public WorldPathFinder(Client client)
        {
            if (!isEnabled)
            {
                StringHelper.WriteLine("[WorldPathFinder] Sorry WorldPathFinder is only available if Dofus Retro is installed.", ConsoleColor.Yellow);
                return;
            }
            _client = client;
        }

        public void GetPath(int _x, int _y)
        {
            while (!_client.MapManager.Map_updated)
                Thread.Sleep(500);
            if (!isEnabled)
            {
                StringHelper.WriteLine("[WorldPathFinder] Sorry WorldPathFinder is only available if Dofus Retro is installed.", ConsoleColor.Yellow);
                return;
            }

            var list = DataManager.GlobalMapsInfos.Where(x => x.xPos == _x && x.yPos == _y && x.SubAreaId == DataManager.GlobalMapsInfos.First(y => y.Id == _client.MapManager.Id).SubAreaId);
            foreach (var map in list)
                StringHelper.WriteLine($"{_x},{_y} mapId - [{map.Id}]", ConsoleColor.Yellow);

            GetNeighboursMaps();
        }

        private List<MapObj> GetMapDatas(int id)
        {
            List<MapDatas> mapSwfContent = DataManager.ReadListSwfMap(id.ToString());
            List<MapObj> result = new List<MapObj>();
            foreach (var mapContent in mapSwfContent) {
                // if(mapContent.SubAreaId == DataManager.GlobalMapsInfos.First(x => x.Id == currentMap.Id).SubAreaId)
                //var data = DataManager.GlobalMapsInfos.First(x => x == mapContent);
                    result.Add(new MapObj(mapContent));
            }
            return result;
        }

        private void GetNeighboursMaps()
        {
            

            //TOP -> x ,y-1

            int wantedx = currentMap.xCoord;
            int wantedy = currentMap.yCoord - 1;
            List<MapDatas> Top = DataManager.GlobalMapsInfos.Where(x => x.xPos == wantedx && x.yPos == wantedy).ToList();
            var FinalList = new List<MapObj>();
            foreach (var map in Top) //getting maps data by reading swf files
                FinalList.AddRange(GetMapDatas(map.Id).ToList());
            
            var list = FinalList.Where(x => x.MapDatas.OutDoor == true);

            foreach (var map in list)
            {
                Console.WriteLine($"Top map id {map.Id} | cells : {map.Cells.Count()} | teleporter {map.Cells.Count(x => x.is_Teleporter())}");
            }

            //BOTTOM -> x, y+1

            wantedx = currentMap.xCoord;
            wantedy = currentMap.yCoord + 1;
            List<MapDatas> Bottom = DataManager.GlobalMapsInfos.Where(x => x.xPos == wantedx && x.yPos == wantedy).ToList();
            foreach (var map in Bottom) //getting maps data by reading swf files
                GetSwfContent(map.Id.ToString());

            var SameLayerBottom = Bottom.Where(x => x.OutDoor).ToList();

            //LEFT -> x-1 ,y

            wantedx = currentMap.xCoord - 1;
            wantedy = currentMap.yCoord;
            List<MapDatas> Left = DataManager.GlobalMapsInfos.Where(x => x.xPos == wantedx && x.yPos == wantedy).ToList();
            foreach (var map in Left) //getting maps data by reading swf files
                GetSwfContent(map.Id.ToString());

            var SameLayerLeft = Left.Where(x => x.OutDoor).ToList();

            //RIGHT -> x+1, y

            wantedx = currentMap.xCoord + 1;
            wantedy = currentMap.yCoord;
            List<MapDatas> Right = DataManager.GlobalMapsInfos.Where(x => x.xPos == wantedx && x.yPos == wantedy).ToList();
            foreach (var map in Right) //getting maps data by reading swf files
                GetSwfContent(map.Id.ToString());

            var SameLayerRight = Right.Where(x => x.OutDoor).ToList();

            Console.WriteLine("find !");
        }

        public static bool GetSwfContent(string id) // il peut y avoir plusieurs id de map identiques ..
        {
            List<MapDatas> MapData = DataManager.ReadListSwfMap(id);
            foreach (var map in MapData)
            {
                var list = DataManager.GlobalMapsInfos.Where(x => x.Id == map.Id);
                if (list.Count() != 1) //faudra trier ici par Subarea ..
                    return false;

                var obj = list.First();
                obj.OutDoor = map.OutDoor;
            }
            return true;
        }
    }
}