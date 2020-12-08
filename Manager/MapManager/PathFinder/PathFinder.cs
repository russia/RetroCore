using RetroCore.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace RetroCore.Manager.MapManager.PathFinder
{
    public class PathFinder
    {
        private Client Client;
        private List<Cell> mapCellsWalkable = new List<Cell>();     
        private List<Cell> visitedCells = new List<Cell>();

        public PathFinder(Client client)
        {
            Client = client;
        }

        public void Refresh()
        {
            mapCellsWalkable.Clear();
            visitedCells.Clear();

            while (!Client.MapManager.Map_updated)
                Thread.Sleep(500);
            mapCellsWalkable = Client.MapManager.Cells.Where(x => x.is_Walkable() == true).ToList();
        }

        public List<Cell> GetPath(short cellId)
        {
            Refresh();
            Cell StartCell = Client.MapManager.CurrentCell;
            Cell DestinationCell = Client.MapManager.GetCellById(cellId);
            StringHelper.WriteLine("Start cellid: " + StartCell.Id, ConsoleColor.Blue);
            StringHelper.WriteLine("Destination cellid: " + DestinationCell.Id, ConsoleColor.Blue);

            List<Cell> Path = new List<Cell>();
            StartCell.SetDistance(StartCell.get_Distance(DestinationCell));

            List<Cell> activeCells = new List<Cell>();
            activeCells.Add(StartCell);

            while (activeCells.Any())
            {
                Cell checkCell = activeCells.OrderBy(x => x.CostDistance).First();

                if (checkCell.x == DestinationCell.x && checkCell.y == DestinationCell.y)
                {
                    StringHelper.WriteLine("We found a way !", ConsoleColor.Blue);
                    Cell tempCell = checkCell;

                    while (true)
                    {
                        if (tempCell.Parent == null)
                            break;

                        Path.Add(tempCell.Parent);
                        tempCell = tempCell.Parent;
                    }
                    Path.Reverse();
                    Path.Add(DestinationCell);
                    Client.MapManager.ActualPath = Path;
                    return Path;
                }

                visitedCells.Add(checkCell);
                activeCells.Remove(checkCell);

                var walkableCells = GetWalkableCells(checkCell, DestinationCell);

                foreach (var walkablecell in walkableCells)
                {
                    if (visitedCells.Any(x => x.x == walkablecell.x && x.y == walkablecell.y))
                        continue;

                    if (activeCells.Any(x => x.x == walkablecell.x && x.y == walkablecell.y))
                    {
                        Cell existingCell = activeCells.First(x => x.x == walkablecell.x && x.y == walkablecell.y);
                        if (existingCell.CostDistance > checkCell.CostDistance)
                        {
                            activeCells.Remove(existingCell);
                            activeCells.Add(walkablecell);
                        }
                    }
                    else
                        activeCells.Add(walkablecell);
                }
            }

            Console.WriteLine("No Path Found!");
            return null;
        }

        public List<Cell> GetUnvisitedDiagonales(Cell currentCell)
        {
            List<Cell> OneCellsRadius = new List<Cell>();
            OneCellsRadius = mapCellsWalkable.Where(x => x.Id != currentCell.Id && x.get_Distance(currentCell) == 1 && !visitedCells.Contains(x) && x.Parent == null).ToList();
            List<Cell> Result = new List<Cell>();
            foreach (var cell in OneCellsRadius)
            {
                IEnumerable<Cell> list = mapCellsWalkable.Where(x => x.get_Distance(cell) == 1 && x.Id != currentCell.Id);
                foreach (Cell neighbours in list)
                    Result.Add(neighbours);
            }
            Result = Result.Where(x => Result.Where(y => y == x).Count() == 2).Distinct().ToList();
            return Result;
        }

        public List<Cell> GetUnvisitedNeighbours(Cell currentCell) => mapCellsWalkable.Where(x => x.Id != currentCell.Id && x.get_Distance(currentCell) == 1 && !visitedCells.Contains(x) && x.Parent == null).ToList();

        private List<Cell> GetWalkableCells(Cell currentCell, Cell targetCell)
        {
            List<Cell> possibleTiles = GetUnvisitedDiagonales(currentCell);
            possibleTiles.AddRange(GetUnvisitedNeighbours(currentCell));
            foreach (var possibility in possibleTiles)
            {
                possibility.Parent = currentCell;
                possibility.Cost++;
                possibility.SetDistance(1);
            }

            return possibleTiles.ToList();
        }
    }
}