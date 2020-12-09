namespace RetroCore.Helpers.MapsReader.Types
{
    public class MapDatas
    {
        public int Id { get; set; }
        public int XPos { get; set; }
        public int YPos { get; set; }
        public int AreaId { get; set; } = -1; //main area -> Incarnam MA.a
        public int SubAreaId { get; set; } = -1; // sub -> pitons rocheux MA.sa
        public SwfDecompiledMap SwfDatas { get; set; }

        public MapDatas(int _id, int _x, int _y, int _subAreaId)
        {
            Id = _id;
            XPos = _x;
            YPos = _y;
            SubAreaId = _subAreaId;
        }
    }
}