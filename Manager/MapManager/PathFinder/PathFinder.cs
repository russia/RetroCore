using RetroCore.Helpers;
using RetroCore.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace RetroCore.Manager.MapManager.PathFinder
{
    public class PathFinder : IClearable
    {
        private readonly Client Client;
        private List<Cell> mapCellsWalkable = new List<Cell>();
        private readonly List<Cell> visitedCells = new List<Cell>();

        public PathFinder(Client client)
        {
            Client = client;
        }

        public void Refresh()
        {
            Clear();
            while (!Client.MapManager.Map_updated)
                Thread.Sleep(50);
            mapCellsWalkable = Client.MapManager.Cells.Where(x => x.IsWalkable() == true).ToList();
        }

        public List<Cell> GetPath(short cellId)
        {
            Refresh();
            Cell StartCell = Client.MapManager.CurrentCell;
            Cell DestinationCell = Client.MapManager.GetCellById(cellId);
            StringHelper.WriteLine("Start cellid: " + StartCell.Id, ConsoleColor.Blue);
            StringHelper.WriteLine("Destination cellid: " + DestinationCell.Id, ConsoleColor.Blue);

            List<Cell> Path = new List<Cell>();
            StartCell.SetDistance(StartCell.GetDistance(DestinationCell));

            List<Cell> activeCells = new List<Cell>
            {
                StartCell
            };

            while (activeCells.Any())
            {
                Cell checkCell = activeCells.OrderBy(x => x.CostDistance).First();

                if (checkCell.X == DestinationCell.X && checkCell.Y == DestinationCell.Y)
                {
                    StringHelper.WriteLine("We found a way !", ConsoleColor.Blue);
                    Cell tempCell = checkCell;

                    while (true) // while(tempCell.Parent == null) ?
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

                var walkableCells = GetWalkableCells(checkCell);//, DestinationCell);

                foreach (var walkablecell in walkableCells)
                {
                    if (visitedCells.Any(x => x.X == walkablecell.X && x.Y == walkablecell.Y))
                        continue;

                    if (activeCells.Any(x => x.X == walkablecell.X && x.Y == walkablecell.Y))
                    {
                        Cell existingCell = activeCells.First(x => x.X == walkablecell.X && x.Y == walkablecell.Y);
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
            OneCellsRadius = mapCellsWalkable.Where(x => x.Id != currentCell.Id && x.GetDistance(currentCell) == 1 && !visitedCells.Contains(x) && x.Parent == null).ToList();
            List<Cell> Result = new List<Cell>();
            foreach (var cell in OneCellsRadius)
            {
                IEnumerable<Cell> cells = mapCellsWalkable.Where(x => x.GetDistance(cell) == 1 && x.Id != currentCell.Id);
                cells.ToList().ForEach(x => Result.Add(x));
            }
            Result = Result.Where(x => Result.Where(y => y == x).Count() == 2).Distinct().ToList();
            return Result;
        }

        public List<Cell> GetUnvisitedNeighbours(Cell currentCell) => mapCellsWalkable.Where(x => x.Id != currentCell.Id && x.GetDistance(currentCell) == 1 && !visitedCells.Contains(x) && x.Parent == null).ToList();

        private List<Cell> GetWalkableCells(Cell currentCell)//, Cell targetCell)
        {
            List<Cell> possibleCells = GetUnvisitedDiagonales(currentCell);
            possibleCells.AddRange(GetUnvisitedNeighbours(currentCell));
            possibleCells.ForEach(x => x.SetParent(currentCell));
            return possibleCells.ToList();
        }
        public void Clear()
        {
            mapCellsWalkable.Clear();
            visitedCells.Clear();
        }
    }
}