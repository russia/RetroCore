namespace RetroCore.Helpers.MapsReader
{
    public class SwfDecompiledMap
    {
        public int Id { get; set; }
        public string MapId { get; set; }
        public bool OutDoor { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public string CypheredMapData { get; set; }
        public string DecypheredMapData { get; set; }

        public SwfDecompiledMap()
        {
        }
    }
}