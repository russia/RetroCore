using System;
using System.Collections.Generic;
using System.Linq;

namespace RetroCore.Manager.MapManager.PathFinder
{
    public class PathFinder
    {
        private List<Cell> mapCellsWalkable = new List<Cell>();
        private Client Client;
        private List<Cell> visitedTiles = new List<Cell>();

        public PathFinder(Client client)
        {
            Client = client;
        }

        public void Update()
        {
            mapCellsWalkable = Client.MapManager.Cells.Where(x => x.is_Walkable() == true).ToList();
        }

        public List<Cell> GetPath(short cellId)
        {
            Update();
            Cell StartCell = Client.MapManager.CurrentCell;
            Console.WriteLine("Start cellid : " + StartCell.Id);
            Cell DestinationCell = Client.MapManager.GetCellById(cellId);
            List<Cell> Path = new List<Cell>();
            StartCell.SetDistance(DestinationCell.x, DestinationCell.y);

            var activeTiles = new List<Cell>();
            activeTiles.Add(StartCell);

            while (activeTiles.Any())
            {
                var checkTile = activeTiles.OrderBy(x => x.CostDistance).First();

                if (checkTile.x == DestinationCell.x && checkTile.y == DestinationCell.y)
                {
                    Console.WriteLine("We are at the destination!");
                    Cell tempCell = checkTile;

                    while (true)
                    {
                        if (tempCell.Parent == null)
                            break;
                        Path.Add(tempCell.Parent);
                        tempCell = tempCell.Parent;
                    }
                    return Path;
                }

                visitedTiles.Add(checkTile);
                activeTiles.Remove(checkTile);

                var walkableTiles = GetWalkableCells(checkTile, DestinationCell);

                foreach (var walkableTile in walkableTiles)
                {
                    if (visitedTiles.Any(x => x.x == walkableTile.x && x.y == walkableTile.y))
                        continue;

                    if (activeTiles.Any(x => x.x == walkableTile.x && x.y == walkableTile.y))
                    {
                        var existingTile = activeTiles.First(x => x.x == walkableTile.x && x.y == walkableTile.y);
                        if (existingTile.CostDistance > checkTile.CostDistance)
                        {
                            activeTiles.Remove(existingTile);
                            activeTiles.Add(walkableTile);
                        }
                    }
                    else
                        activeTiles.Add(walkableTile);
                }
            }

            Console.WriteLine("No Path Found!");
            return null;
        }

        private List<Cell> GetWalkableCells(Cell currentCell, Cell targetCell)
        {
            var possibleTiles = mapCellsWalkable.Where(x => x.Id != currentCell.Id && x.get_Distance(currentCell) == 1 && !visitedTiles.Contains(x)).ToList();

            foreach (var possibility in possibleTiles)
            {
                possibility.Parent = currentCell;
                possibility.Cost++;
            }
            possibleTiles.ForEach(tile => tile.SetDistance(targetCell.x, targetCell.y));

            return possibleTiles
                    .ToList();
        }
    }
}