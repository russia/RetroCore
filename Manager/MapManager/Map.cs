using RetroCore.Helpers;
using RetroCore.Helpers.MapsReader;
using RetroCore.Helpers.MapsReader.Types;
using RetroCore.Manager.MapManager.Interactives;
using RetroCore.Others;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RetroCore.Manager.MapManager
{
    public class Map : IDisposable
    {
        public short Id { get; set; }
        public byte Width { get; set; }
        public byte Height { get; set; }
        public int XCoord { get; set; }
        public int YCoord { get; set; }
        public bool Map_updated { get; protected set; }
        public MapDatas MapDatas { get; set; }
        public Cell CurrentCell { get; set; }
        public List<Cell> ActualPath;
        public ConcurrentDictionary<int, IEntity> Entities;
        public ConcurrentDictionary<int, InteractiveObject> Interactives;
        public Cell[] Cells;

        public Client Client { get; set; }

        //WorldPathFinder
        public Map ParentMap { get; set; } = null;

        public int Cost { get; set; }
        public int Distance { get; set; }
        public int CostDistance => Cost + Distance;

        public void SetParent(Map parent)
        {
            ParentMap = parent;
            Cost++;
            SetDistance(1);
        }

        public int GetDistance(int xDestination, int yDestination) => Math.Abs(XCoord - xDestination) + Math.Abs(YCoord - yDestination);

        public int GetDistance(Map destinationMap) => Math.Abs(XCoord - destinationMap.XCoord) + Math.Abs(YCoord - destinationMap.YCoord);

        public void SetDistance(int distance)
        {
            this.Distance = distance;
        }

        /////////

        public Map(Client client)
        {
            this.Client = client;
            ActualPath = new List<Cell>();
            Entities = new ConcurrentDictionary<int, IEntity>();
            Interactives = new ConcurrentDictionary<int, InteractiveObject>();
        }

        public Map(MapDatas map)
        {
            ActualPath = new List<Cell>();
            Entities = new ConcurrentDictionary<int, IEntity>();
            Interactives = new ConcurrentDictionary<int, InteractiveObject>();
            MapDatas = map;
            //Getting map infos
            Id = (short)map.Id;
            Width = byte.Parse(map.SwfDatas.Width.ToString());
            Height = byte.Parse(map.SwfDatas.Height.ToString());
            XCoord = map.XPos;
            YCoord = map.YPos;
            DecompressMap(map.SwfDatas.DecypheredMapData);
        }

        #region updates

        public void UpdateMapData(string packet)
        {
            Task.Factory.StartNew(() =>
            {
                Dispose();
                string[] splitted = packet.Split('|');
                MapDatas map = DataManager.GetMapContent(splitted[1], splitted[2], splitted[3]);
                Id = (short)map.Id;
                Width = byte.Parse(map.SwfDatas.Width.ToString());
                Height = byte.Parse(map.SwfDatas.Height.ToString());
                XCoord = map.XPos;
                YCoord = map.YPos;
                DecompressMap(map.SwfDatas.DecypheredMapData);
                Map_updated = true;
                StringHelper.WriteLine($"Current map Coords : [{Client.MapManager.XCoord},{Client.MapManager.YCoord} Area [{DataManager.GlobalMapsInfos.First(x => x.Id == Id).AreaId}] - SubArea [{DataManager.GlobalMapsInfos.First(x => x.Id == Id).SubAreaId}]]", ConsoleColor.Green);
                StringHelper.WriteLine($"Current size : [{Client.MapManager.Width},{Client.MapManager.Height}]", ConsoleColor.Green);
            });
        }

        #region Getters

        public Cell GetCellById(short CellId) => Cells[CellId];

        #endregion Getters

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

        public void Dispose()
        {
            Map_updated = false;
            Cells = null;
            Entities.Clear();
            Interactives.Clear();
        }

        #endregion updates
    }
}