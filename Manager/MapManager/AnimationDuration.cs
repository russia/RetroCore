using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RetroCore.Manager.MapManager
{
    public class AnimationDuration
    {
        public int straight { get; private set; }
        public int horizontal { get; private set; }
        public int vertical { get; private set; }

        public AnimationDuration(int _straight, int _horizontal, int _vertical)
        {
            straight = _straight;
            horizontal = _horizontal;
            vertical = _vertical;
        }
    }
}
