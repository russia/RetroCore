namespace RetroCore.Helpers.MapsReader.Types
{
    public class MapCoordinates
    {
        public int MapId { get; set; }
        public int xPos { get; set; }
        public int yPos { get; set; }

        public MapCoordinates(int _mapid, int _x, int _y)
        {
            MapId = _mapid;
            xPos = _x;
            yPos = _y;
        }
    }
}