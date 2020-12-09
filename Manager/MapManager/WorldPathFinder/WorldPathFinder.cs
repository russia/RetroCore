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
        private readonly List<Map> visitedMaps = new List<Map>();

        public WorldPathFinder(Client client)
        {
            if (!IsEnabled)
                throw new Exception("[WorldPathFinder] Sorry WorldPathFinder is only available if Dofus Retro is installed.");
            Client = client;
        }

        public void GetPath(int _x, int _y)
        {
            while (!Client.MapManager.Map_updated)
                Thread.Sleep(500);
            if (!IsEnabled)
                throw new Exception("[WorldPathFinder] Sorry WorldPathFinder is only available if Dofus Retro is installed.");
            //TODO: Utiliser AreaId au lieu de SubArea
            //List<MapDatas> currentMapInfo = DataManager.GlobalMapsInfos.Where(x => x.XPos == _x && x.YPos == _y && x.SubAreaId == DataManager.GlobalMapsInfos.First(y => y.Id == Client.MapManager.Id).SubAreaId).ToList();
            //currentMapInfo.ForEach(x => StringHelper.WriteLine($"{_x},{_y} mapId - [{x.Id}]", ConsoleColor.Yellow));

            //List<Cell> availablesTeleporters = Teleporters.GetAccessiblesTeleporters(Client, CurrentMap);
            //List<DirectionType> AvailableDirections = new List<DirectionType>();
            //availablesTeleporters.ForEach(x => AvailableDirections.Add(Teleporters.GetTeleporterSide(x)));

            //AvailableDirections.ForEach(x => Console.WriteLine("Direction possible : " + x.ToString()));

            var test = GetWorldPath(2,-19);
            Console.WriteLine(test.Count());
            //GetNeighboursMaps(DirectionType.BOTTOM);
            //GetNeighboursMaps(DirectionType.TOP);
            //GetNeighboursMaps(DirectionType.LEFT);
            //GetNeighboursMaps(DirectionType.RIGHT);
        }

        public List<Map> GetWorldPath(int xCoordsDestination, int yCoordsDestination)
        {
            Map StartMap = CurrentMap;
            //Todo replace First by where ?
            var DestinationMap = DataManager.GlobalMapsInfos.First(x => x.XPos == xCoordsDestination && x.YPos == yCoordsDestination && x.AreaId == DataManager.GlobalMapsInfos.First(y => y.Id == Client.MapManager.Id).AreaId);
            StringHelper.WriteLine("Start map: " + StartMap.Id, ConsoleColor.Blue);
            StringHelper.WriteLine("Destination map: " + DestinationMap.Id, ConsoleColor.Blue);

            Dictionary<Map, DirectionType> Path = new Dictionary<Map, DirectionType>();
            StartMap.SetDistance(StartMap.GetDistance(DestinationMap.XPos, DestinationMap.YPos));
            StringHelper.WriteLine("Distance (in maps) : " + StartMap.Distance, ConsoleColor.Blue);
            List<Map> activeMaps = new List<Map>
            {
                StartMap
            };

            while (activeMaps.Any())
            {
                Map checkMap = activeMaps.OrderBy(x => x.CostDistance).First();

                if (checkMap.XCoord == DestinationMap.XPos && checkMap.YCoord == DestinationMap.YPos)
                {
                    StringHelper.WriteLine("We found a way !", ConsoleColor.Blue);
                }

                visitedMaps.Add(checkMap);
                activeMaps.Remove(checkMap);

                var walkableMaps = GetWalkableMaps(checkMap);//, DestinationCell);

                foreach (var walkablemap in walkableMaps)
                {
                    if (visitedMaps.Any(x => x.XCoord == walkablemap.XCoord && x.YCoord == walkablemap.YCoord))
                        continue;

                    if (activeMaps.Any(x => x.XCoord == walkablemap.XCoord && x.YCoord == walkablemap.YCoord))
                    {
                        Map existingMap = activeMaps.First(x => x.XCoord == walkablemap.XCoord && x.YCoord == walkablemap.YCoord);
                        if (existingMap.CostDistance > checkMap.CostDistance)
                        {
                            activeMaps.Remove(existingMap);
                            activeMaps.Add(walkablemap);
                        }
                    }
                    else
                        activeMaps.Add(walkablemap);
                }
            }

            Console.WriteLine("No Path Found!");
            return null;
        }

        private List<Map> GetWalkableMaps(Map currentMap)//, Cell targetCell)
        {
            var FinalList = new List<Map>();
            List<Cell> availablesTeleporters = Teleporters.GetAccessiblesTeleporters(Client, currentMap);
            List<DirectionType> AvailableDirections = new List<DirectionType>();
            availablesTeleporters.ForEach(x => AvailableDirections.Add(Teleporters.GetTeleporterSide(x)));
            AvailableDirections.ForEach(x => FinalList.AddRange(GetNeighboursMaps(x)));
            FinalList.RemoveAll(x => visitedMaps.Contains(x));
            FinalList.ForEach(x => x.SetParent(currentMap));
            FinalList.ForEach(x => Console.WriteLine("GetWalkablemaps returned : " + x.Id));
            return FinalList;
        }

        private List<Map> GetMapDatas(int id)
        {
            List<MapDatas> mapSwfContent = DataManager.ReadListSwfMap(id.ToString());
            List<Map> result = new List<Map>();
            mapSwfContent.ForEach(x => result.Add(new Map(x)));
            return result;
        }

        private List<Map> GetNeighboursMaps(DirectionType Type)
        {
            Coordinates wantedCoords = Directions.GetCoordsByDirection(CurrentMap, Type);
            List<MapDatas> mapsList = DataManager.GlobalMapsInfos.Where(x => x.XPos == wantedCoords._x && x.YPos == wantedCoords._y).ToList();
            List<Map> finalMapList = new List<Map>();
            mapsList.ForEach(x => finalMapList.AddRange(GetMapDatas(x.Id)));
            List<Map> outDoorList = finalMapList.Where(x => x.MapDatas.SwfDatas.OutDoor == true).ToList();
            //outDoorList.ForEach(x => StringHelper.WriteLine($"{Type} map id {x.Id} | cells : {x.Cells.Count()} | teleporter {x.Cells.Count(x => x.IsTeleporter())}", ConsoleColor.Red));
            return outDoorList;
        }

        public void Clear()
        {
        }
    }
}