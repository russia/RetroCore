using RetroCore.Helpers;
using RetroCore.Helpers.MapsReader;
using RetroCore.Manager.MapManager.Interactives;
using RetroCore.Others;
using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace RetroCore.Manager.MapManager
{
    public class Map
    {
        public short Id { get; set; }
        public byte Width { get; set; }
        public byte Height { get; set; }
        public Client _client { get; set; }
        public sbyte x { get; set; }
        public sbyte y { get; set; }
        public bool Map_updated { get; private set; }

        public Cell CurrentCell { get; set; }

        public ConcurrentDictionary<int, Entity> Entities;
        public ConcurrentDictionary<int, InteractiveObject> Interactives;
        public Cell[] Cells;

        public Map(Client client)
        {
            this._client = client;
            Entities = new ConcurrentDictionary<int, Entity>();
            Interactives = new ConcurrentDictionary<int, InteractiveObject>();
        }
        #region Getters
        public Cell GetCellById(short CellId) => Cells[CellId];
        #endregion

        #region updates

        public void UpdateMapData(string packet)
        {
            Task.Factory.StartNew(() =>
            {
                Entities.Clear();
                Interactives.Clear();
                string[] splitted = packet.Split('|');
                SwfDecompiledMap map = DataManager.GetSwfContent(splitted[1], splitted[2], splitted[3]);
                Id = (short)map.Id;
                Width = byte.Parse(map.Width.ToString());
                Height = byte.Parse(map.Height.ToString());
                DecompressMap(map.DecypheredMapData);
                Map_updated = true;
            });
        }

        #endregion updates

        #region mapdecompression

        public void DecompressMap(string mapData)
        {
            Cells = new Cell[mapData.Length / 10];
            string cellDatas;

            for (int i = 0; i < mapData.Length; i += 10)
            {
                cellDatas = mapData.Substring(i, 10);
                Cells[i / 10] = DecompressCell(cellDatas, Convert.ToInt16(i / 10));
            }
        }

        public Cell DecompressCell(string celldatas, short cellid)
        {
            byte[] cellinfo = new byte[celldatas.Length];

            for (int i = 0; i < celldatas.Length; i++)
                cellinfo[i] = Convert.ToByte(Hash.GetHash(celldatas[i]));

            CellsType type = (CellsType)((cellinfo[2] & 56) >> 3);
            bool isActive = (cellinfo[0] & 32) >> 5 != 0;
            bool isLinear = (cellinfo[0] & 1) != 1;
            bool containInteractive = ((cellinfo[7] & 2) >> 1) != 0;
            short layer_object_2_num = Convert.ToInt16(((cellinfo[0] & 2) << 12) + ((cellinfo[7] & 1) << 12) + (cellinfo[8] << 6) + cellinfo[9]);
            short layer_object_1_num = Convert.ToInt16(((cellinfo[0] & 4) << 11) + ((cellinfo[4] & 1) << 12) + (cellinfo[5] << 6) + cellinfo[6]);
            byte level = Convert.ToByte(cellinfo[1] & 15);
            byte slope = Convert.ToByte((cellinfo[4] & 60) >> 2);

            return new Cell(cellid, isActive, type, isLinear, level, slope, containInteractive ? layer_object_2_num : Convert.ToInt16(-1), layer_object_1_num, layer_object_2_num, this);
        }

        #endregion mapdecompression
    }
}