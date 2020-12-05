using RetroCore.Helpers.MapsReader;

namespace RetroCore.Manager
{
    public class MapManager
    {
        public short Id { get; set; }
        public byte Width { get; set; }
        public byte Height { get; set; }
        public Client _client { get; set; }
        public sbyte x { get; set; }
        public sbyte y { get; set; }
        public bool map_updated { get; private set; }
        public MapManager(Client client)
        {
            this._client = client;
        }

        #region updates

        public void UpdateMapData(string packet)
        {
            //entities.Clear();
            //interactives.Clear();
            string[] splitted = packet.Split('|');
            SwfDecompiledMap map = DataManager.GetSwfContent(splitted[1], splitted[2], splitted[3]);
            Id = (short)map.Id;
            x = sbyte.Parse(map.Width.ToString());
            y = sbyte.Parse(map.Height.ToString());
          //  descomprimir_mapa(map.DecypheredMapData);


            map_updated = true;
        }

        #endregion updates
    }
}