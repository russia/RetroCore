namespace RetroCore.Manager.MapManager.Interactives
{
    public class InteractiveObject
    {
        public short Gfx { get; private set; }
        public Cell Cell { get; private set; }
        public InteractiveModel Model { get; private set; }
        public bool IsUsable { get; set; } = false;

        public InteractiveObject(short _gfx, Cell _cell)
        {
            Gfx = _gfx;
            Cell = _cell;

            InteractiveModel _model = InteractiveModel.GetGfxModel(Gfx);

            if (_model != null)
            {
                Model = _model;
                IsUsable = true;
            }
        }
    }
}