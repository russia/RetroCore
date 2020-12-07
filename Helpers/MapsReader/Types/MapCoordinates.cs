namespace RetroCore.Helpers.MapsReader.Types
{
    public class MapCoordinates
    {
        public int MapId { get; set; }
        public int xPos { get; set; }
        public int yPos { get; set; }
        public int AreaId { get; set; } = -1; //main area -> Incarnam MA.a
        public int SubAreaId { get; set; } // sub -> pitons rocheux MA.sa


        public MapCoordinates(int _mapid, int _x, int _y, int _subAreaId)
        {
            MapId = _mapid;
            xPos = _x;
            yPos = _y;
            SubAreaId = _subAreaId;
        }
    }
}