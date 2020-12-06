namespace RetroCore.Helpers.MapsReader
{
    public class SwfDecompiledMap
    {
        public int Id { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public int XValue { get; set; }
        public int YValue { get; set; }
        public string MapData { get; set; }
        public string DecypheredMapData { get; set; }

        public SwfDecompiledMap()
        {
        }

        public SwfDecompiledMap(int id, int width, int height, string mapdata)
        {
            this.Id = id;
            this.Width = width;
            this.Height = height;
            this.MapData = mapdata;
        }
    }
}