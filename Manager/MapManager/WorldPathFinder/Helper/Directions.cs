using RetroCore.Others;

namespace RetroCore.Manager.MapManager.WorldPathFinder.Helper
{
    public class Directions
    {
        public static Coordinates GetCoordsByDirection(Map currentMap, DirectionType type)
        {
            switch (type)
            {
                case DirectionType.TOP:
                    return new Coordinates(currentMap.xCoord, currentMap.yCoord - 1);

                case DirectionType.BOTTOM:
                    return new Coordinates(currentMap.xCoord, currentMap.yCoord + 1);

                case DirectionType.LEFT:
                    return new Coordinates(currentMap.xCoord - 1, currentMap.yCoord);

                case DirectionType.RIGHT:
                    return new Coordinates(currentMap.xCoord + 1, currentMap.yCoord);

                default:
                    return null;
            }
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