using RetroCore.Others;

namespace RetroCore.Manager.MapManager.WorldPathFinder.Helper
{
    public class Directions
    {
        public static Coordinates GetCoordsByDirection(Map currentMap, DirectionType type)
        {
            return type switch
            {
                DirectionType.TOP => new Coordinates(currentMap.XCoord, currentMap.YCoord - 1),
                DirectionType.BOTTOM => new Coordinates(currentMap.XCoord, currentMap.YCoord + 1),
                DirectionType.LEFT => new Coordinates(currentMap.XCoord - 1, currentMap.YCoord),
                DirectionType.RIGHT => new Coordinates(currentMap.XCoord + 1, currentMap.YCoord),
                _ => null,
            };
        }
    }

    public class Coordinates
    {
        public int _x;
        public int _y;

        public Coordinates(int x, int y)
        {
            _x = x;
            _y = y;
        }
    }
}