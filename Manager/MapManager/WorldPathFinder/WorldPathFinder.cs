using RetroCore.Helpers;
using RetroCore.Helpers.MapsReader;
using RetroCore.Helpers.MapsReader.Types;
using RetroCore.Interfaces;
using RetroCore.Manager.MapManager.WorldPathFinder.Helper;
using RetroCore.Others;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace RetroCore.Manager.MapManager.WorldPathFinder
{
    public class WorldPathFinder : IClearable
    {
        private readonly Client Client;
        private bool IsEnabled => DataManager.GamePathFound;
        private Map CurrentMap => Client.MapManager;

        public WorldPathFinder(Client client)
        {
            if (!IsEnabled)
                throw new Exception("[WorldPathFinder] Sorry WorldPathFinder is only available if Dofus Retro is installed.");
            Client = client;
        }

        public void GetPath(int _x, int _y)
        {
            while (!Client.MapManager.Map_updated)
                Thread.Sleep(100);
            if (!IsEnabled)
                throw new Exception("[WorldPathFinder] Sorry WorldPathFinder is only available if Dofus Retro is installed.");
            //TODO: Utiliser AreaId au lieu de SubArea
            List<MapDatas> currentMapInfo = DataManager.GlobalMapsInfos.Where(x => x.XPos == _x && x.YPos == _y && x.SubAreaId == DataManager.GlobalMapsInfos.First(y => y.Id == Client.MapManager.Id).SubAreaId).ToList();
            currentMapInfo.ForEach(x => StringHelper.WriteLine($"{_x},{_y} mapId - [{x.Id}]", ConsoleColor.Yellow));

            List<Cell> availablesTeleporters = Teleporters.GetAccessiblesTeleporters(Client, CurrentMap);

            //GetNeighboursMaps(DirectionType.BOTTOM);
            //GetNeighboursMaps(DirectionType.TOP);
            //GetNeighboursMaps(DirectionType.LEFT);
            //GetNeighboursMaps(DirectionType.RIGHT);
        }

        private List<Map> GetMapDatas(int id)
        {
            List<MapDatas> mapSwfContent = DataManager.ReadListSwfMap(id.ToString());
            List<Map> result = new List<Map>();
            mapSwfContent.ForEach(x => result.Add(new Map(x)));
            return result;
        }

        private void GetNeighboursMaps(DirectionType Type)
        {
            Coordinates wantedCoords = Directions.GetCoordsByDirection(CurrentMap, Type);
            List<MapDatas> mapsList = DataManager.GlobalMapsInfos.Where(x => x.XPos == wantedCoords._x && x.YPos == wantedCoords._y).ToList();
            List<Map> finalMapList = new List<Map>();
            mapsList.ForEach(x => finalMapList.AddRange(GetMapDatas(x.Id)));
            List<Map> outDoorList = finalMapList.Where(x => x.MapDatas.SwfDatas.OutDoor == true).ToList();
            outDoorList.ForEach(x => StringHelper.WriteLine($"{Type} map id {x.Id} | cells : {x.Cells.Count()} | teleporter {x.Cells.Count(x => x.IsTeleporter())}", ConsoleColor.Red));
        }

        public void Clear()
        {
        }
    }
}