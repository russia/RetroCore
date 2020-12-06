namespace RetroCore.Manager.MapManager.Interactives
{
    public class InteractiveObject
    {
        public short gfx { get; private set; }
        public Cell cell { get; private set; }
        public InteractiveModel model { get; private set; }
        public bool isUsable { get; set; } = false;

        public InteractiveObject(short _gfx, Cell _cell)
        {
            gfx = _gfx;
            cell = _cell;

            InteractiveModel _model = InteractiveModel.GetGfxModel(gfx);

            if (_model != null)
            {
                model = _model;
                isUsable = true;
            }
        }
    }
}