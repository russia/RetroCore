using System;

namespace RetroCore.Helpers.MapsReader.Types
{
    public class MapDatas
    {
        public int MapId { get; set; }
        public int xPos { get; set; }
        public int yPos { get; set; }
        public int AreaId { get; set; } = -1; //main area -> Incarnam MA.a
        public int SubAreaId { get; set; } = -1; // sub -> pitons rocheux MA.sa
        public bool OutDoor { get; set; }

        public MapDatas(int _mapid, int _x, int _y, int _subAreaId)
        {
            MapId = _mapid;
            xPos = _x;
            yPos = _y;
            SubAreaId = _subAreaId;
        }

        internal int Count()
        {
            throw new NotImplementedException();
        }
    }
}