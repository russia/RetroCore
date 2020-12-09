namespace RetroCore.Manager.MapManager
{
    public class AnimationDuration
    {
        public int Straight { get; private set; }
        public int Horizontal { get; private set; }
        public int Vertical { get; private set; }

        public AnimationDuration(int _straight, int _horizontal, int _vertical)
        {
            Straight = _straight;
            Horizontal = _horizontal;
            Vertical = _vertical;
        }
    }
}