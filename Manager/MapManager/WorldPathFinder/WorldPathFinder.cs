using RetroCore.Helpers;
using RetroCore.Helpers.MapsReader;
using RetroCore.Helpers.MapsReader.Types;
using RetroCore.Manager.MapManager.WorldPathFinder.Helper;
using RetroCore.Others;
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
                Thread.Sleep(100);
            if (!isEnabled)
            {
                StringHelper.WriteLine("[WorldPathFinder] Sorry WorldPathFinder is only available if Dofus Retro is installed.", ConsoleColor.Yellow);
                return;
            }

            MapDatas currentMapInfo = DataManager.GlobalMapsInfos.First(x => x.xPos == _x && x.yPos == _y && x.SubAreaId == DataManager.GlobalMapsInfos.First(y => y.Id == _client.MapManager.Id).SubAreaId);
            StringHelper.WriteLine($"{_x},{_y} mapId - [{currentMapInfo.Id}]", ConsoleColor.Yellow);

            
            GetNeighboursMaps(DirectionType.BOTTOM);
            GetNeighboursMaps(DirectionType.TOP);
            GetNeighboursMaps(DirectionType.LEFT);
            GetNeighboursMaps(DirectionType.RIGHT);
        }

        private List<MapObj> GetMapDatas(int id)
        {
            List<MapDatas> mapSwfContent = DataManager.ReadListSwfMap(id.ToString());
            List<MapObj> result = new List<MapObj>();
            foreach (MapDatas mapContent in mapSwfContent)
                result.Add(new MapObj(mapContent));
            return result;
        }

        private void GetNeighboursMaps(DirectionType Type)
        {
            Coordinates wantedCoords = Directions.GetCoordsByDirection(currentMap, Type);
            List<MapDatas> mapsList = DataManager.GlobalMapsInfos.Where(x => x.xPos == wantedCoords._x && x.yPos == wantedCoords._y).ToList();
            List<MapObj> finalMapList = new List<MapObj>();
            foreach (var map in mapsList)
                finalMapList.AddRange(GetMapDatas(map.Id));
            List<MapObj> outDoorList = finalMapList.Where(x => x.MapDatas.SwfDatas.OutDoor == true).ToList();

            foreach (var map in outDoorList)
            {
                StringHelper.WriteLine($"{Type.ToString()} map id {map.Id} | cells : {map.Cells.Count()} | teleporter {map.Cells.Count(x => x.is_Teleporter())}",ConsoleColor.Red);
            }
        }
    }
}