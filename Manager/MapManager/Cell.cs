using RetroCore.Manager.MapManager.Interactives;
using RetroCore.Others;
using System;
using System.Linq;

namespace RetroCore.Manager.MapManager
{
    public class Cell
    {
        public short Id { get; set; }
        public bool Active { get; set; }
        public CellsType Type { get; set; }
        public bool IsLinear { get; set; } = false;
        public byte Layer_ground_level { get; set; }
        public byte Layer_ground_slope { get; set; }
        public short Layer_object_1_num { get; set; }
        public short Layer_object_2_num { get; set; }
        public InteractiveObject InteractiveObject { get; set; }
        public int X { get; set; } = 0;
        public int Y { get; set; } = 0;

        #region pathfinder

        public int Cost { get; set; }
        public int Distance { get; set; }
        public int CostDistance => Cost + Distance;
        public Cell Parent { get; set; } = null;

        public static readonly int[] teleportTextures = { 1030, 1029, 4088 };

        public void SetParent(Cell parent)
        {
            Parent = parent;
            Cost++;
            SetDistance(1);
        }

        public void SetDistance(int distance)
        {
            this.Distance = distance;
        }

        #endregion pathfinder

        public Cell(short _id, bool active, CellsType _type, bool _isLinear, byte _level, byte _slope, short _interactiveObjId, short _layer_object_1_num, short _layer_object_2_num, Map _map)
        {
            Id = _id;
            Active = active;
            Type = _type;

            Layer_object_1_num = _layer_object_1_num;
            Layer_object_2_num = _layer_object_2_num;

            IsLinear = _isLinear;
            Layer_ground_level = _level;
            Layer_ground_slope = _slope;

            if (_interactiveObjId != -1)
            {
                InteractiveObject = new InteractiveObject(_interactiveObjId, this);
                _map.Interactives.TryAdd(Id, InteractiveObject);
            }

            byte Width = _map.Width;
            int _calcul1 = Id / ((Width * 2) - 1);
            int _calcul2 = Id - (_calcul1 * ((Width * 2) - 1));
            int _calcul3 = _calcul2 % Width;
            Y = _calcul1 - _calcul3;
            X = (Id - ((Width - 1) * Y)) / Width;
        }

        public int GetDistance(Cell destinationCell) => Math.Abs(X - destinationCell.X) + Math.Abs(Y - destinationCell.Y);

        public bool GetIsLinear(Cell destinationCell) => X == destinationCell.X || Y == destinationCell.Y;

        public char GetDirection(Cell cell)
        {
            /** Diagonales **/
            if (X == cell.X)
                return cell.Y < Y ? (char)(3 + 'a') : (char)(7 + 'a');
            else if (Y == cell.Y)
                return cell.X < X ? (char)(1 + 'a') : (char)(5 + 'a');

            /** Lines **/
            else if (X > cell.X)
                return Y > cell.Y ? (char)(2 + 'a') : (char)(0 + 'a');
            else if (X < cell.X)
                return Y < cell.Y ? (char)(6 + 'a') : (char)(4 + 'a');

            throw new Exception("Can't find direction.");
        }

        public bool IsTeleporter() => teleportTextures.Contains(Layer_object_1_num) || teleportTextures.Contains(Layer_object_2_num);

        public bool IsInteractive() => Type == CellsType.INTERACTIVE_OBJECT || InteractiveObject != null;

        public bool IsWalkable() => Active && Type != CellsType.NOT_WALKABLE && !IsInteractive_Walkable();

        public bool IsInteractive_Walkable() => Type == CellsType.INTERACTIVE_OBJECT || InteractiveObject != null && InteractiveObject.Model != null && !InteractiveObject.Model.Walkable;
    }
}