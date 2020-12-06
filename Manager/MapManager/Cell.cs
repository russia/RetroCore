using RetroCore.Manager.MapManager.Interactives;
using RetroCore.Others;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RetroCore.Manager.MapManager
{
    public class Cell
    {
        public short Id { get; private set; }
        public bool Active { get; private set; }
        public CellsType Type { get; private set; }
        public bool isLinear { get; private set; } = false;
        public byte layer_ground_level { get; private set; }
        public byte layer_ground_slope { get; private set; }
        public short layer_object_1_num { get; private set; }
        public short layer_object_2_num { get; private set; }
        public InteractiveObject objectInteractive { get; private set; }
        public int x { get; set; } = 0;
        public int y { get; set; } = 0;

        #region pathfinder
        public int Cost { get; set; }
        public int Distance { get; set; }
        public int CostDistance => Cost + Distance;
        public Cell Parent { get; set; } = null;

        public static readonly int[] teleportTextures = { 1030, 1029, 4088 };

        public Cell()//(int _x, int _y, Cell parent, int _cost)
        {
            //x = _x;
            //y = _y;
            //Parent = parent;
            //Cost = _cost;
        }

        public void SetDistance(int distance)
        {
            this.Distance = 1;
        }

        #endregion

        public Cell(short _id, bool active, CellsType _type, bool _isLinear, byte _level, byte _slope, short _interactiveObjId, short _layer_object_1_num, short _layer_object_2_num, Map _map)
        {
            Id = _id;
            Active = active;
            Type = _type;

            layer_object_1_num = _layer_object_1_num;
            layer_object_2_num = _layer_object_2_num;

            isLinear = _isLinear;
            layer_ground_level = _level;
            layer_ground_slope = _slope;

            if (_interactiveObjId != -1)
            {
                objectInteractive = new InteractiveObject(_interactiveObjId, this);
                _map.Interactives.TryAdd(Id, objectInteractive);
            }

            byte Width = _map.Width;
            int _calcul1 = Id / ((Width * 2) - 1);
            int _calcul2 = Id - (_calcul1 * ((Width * 2) - 1));
            int _calcul3 = _calcul2 % Width;
            y = _calcul1 - _calcul3;
            x = (Id - ((Width - 1) * y)) / Width;
        }

        public int get_Distance(Cell destinationCell) => Math.Abs(x - destinationCell.x) + Math.Abs(y - destinationCell.y);


        public bool get_IsLinear(Cell destinationCell) => x == destinationCell.x || y == destinationCell.y;

        public char get_Direction(Cell cell)
        {
            /** Diagonales **/
            if (x == cell.x)
                return cell.y < y ? (char)(3 + 'a') : (char)(7 + 'a');
            else if (y == cell.y)
                return cell.x < x ? (char)(1 + 'a') : (char)(5 + 'a');

            /** Lines **/
            else if (x > cell.x)
                return y > cell.y ? (char)(2 + 'a') : (char)(0 + 'a');
            else if (x < cell.x)
                return y < cell.y ? (char)(6 + 'a') : (char)(4 + 'a');

            throw new Exception("Can't find direction.");
        }

        public bool is_Teleporter() => teleportTextures.Contains(layer_object_1_num) || teleportTextures.Contains(layer_object_2_num);

        public bool is_Interactive() => Type == CellsType.INTERACTIVE_OBJECT || objectInteractive != null;

        public bool is_Walkable() => Active && Type != CellsType.NOT_WALKABLE && !is_Interactive_Walkable();

        public bool is_Interactive_Walkable() => Type == CellsType.INTERACTIVE_OBJECT || objectInteractive != null && !objectInteractive.model.Walkable;
    }
}