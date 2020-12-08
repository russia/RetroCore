using RetroCore.Helpers;
using RetroCore.Helpers.MapsReader.Types;
using RetroCore.Manager.MapManager.Interactives;
using RetroCore.Others;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace RetroCore.Manager.MapManager.WorldPathFinder
{
    public class MapObj : IDisposable
    {
        //WorldPathFinder
        public MapObj Parent;

        public short Id { get; set; }
        public byte Width { get; set; }
        public byte Height { get; set; }
        public int xCoord { get; set; }
        public int yCoord { get; set; }
        public MapDatas MapDatas { get; set; }
        public Cell CurrentCell { get; set; }
        public List<Cell> ActualPath;
        public ConcurrentDictionary<int, Entity> Entities;
        public ConcurrentDictionary<int, InteractiveObject> Interactives;
        public Cell[] Cells;

        public MapObj(MapDatas map)
        {
            ActualPath = new List<Cell>();
            Entities = new ConcurrentDictionary<int, Entity>();
            Interactives = new ConcurrentDictionary<int, InteractiveObject>();
            MapDatas = map;
            //Getting map infos
            //Id = (short)map.Id;
            //Width = byte.Parse(map.SwfDatas.Width.ToString());
            //Height = byte.Parse(map.SwfDatas.Height.ToString());
            //xCoord = map.xPos;
            //yCoord = map.yPos;
            DecompressMap(map.SwfDatas.DecypheredMapData);
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
            Cells = null;
            Entities.Clear();
            Interactives.Clear();
        }
    }
}