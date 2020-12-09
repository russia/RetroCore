using RetroCore.Others;
using System.Collections.Generic;

namespace RetroCore.Manager.MapManager.PathFinder
{
    public class PathTiming
    {
        private static readonly Dictionary<AnimationType, AnimationDuration> AnimationTime = new Dictionary<AnimationType, AnimationDuration>()
        {
            { AnimationType.MOUNT, new AnimationDuration(135, 200, 120) },
            { AnimationType.RUNNING, new AnimationDuration(170, 255, 150) },
            { AnimationType.WALKING, new AnimationDuration(480, 510, 425) },
            { AnimationType.PHANTOM, new AnimationDuration(57, 85, 50) }
        };

        public static int GetMovementTiming(Cell currentCell, List<Cell> path, bool con_montura = false)
        {
            int movementTime = 20;
            AnimationDuration animationType;

            if (con_montura)
                animationType = AnimationTime[AnimationType.MOUNT];
            else
                animationType = path.Count > 6 ? AnimationTime[AnimationType.RUNNING] : AnimationTime[AnimationType.WALKING];

            Cell nextCell;

            for (int i = 1; i < path.Count; i++)
            {
                nextCell = path[i];

                if (currentCell.Y == nextCell.Y)
                    movementTime += animationType.Horizontal;
                else if (currentCell.X == nextCell.Y)
                    movementTime += animationType.Vertical;
                else
                    movementTime += animationType.Straight;

                if (currentCell.Layer_ground_level < nextCell.Layer_ground_level)
                    movementTime += 100;
                else if (nextCell.Layer_ground_level > currentCell.Layer_ground_level)
                    movementTime -= 100;
                else if (currentCell.Layer_ground_slope != nextCell.Layer_ground_slope)
                {
                    if (currentCell.Layer_ground_slope == 1)
                        movementTime += 100;
                    else if (nextCell.Layer_ground_slope == 1)
                        movementTime -= 100;
                }
                currentCell = nextCell;
            }

            return movementTime;
        }
    }
}