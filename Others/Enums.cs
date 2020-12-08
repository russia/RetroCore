namespace RetroCore.Others
{
    public enum ServerStatus
    {
        OFF,
        ON,
        SAVING
    }

    public enum CellsType
    {
        NOT_WALKABLE = 0,
        INTERACTIVE_OBJECT = 1,
        TELEPORT_CELL = 2,
        UNKNOWN1 = 3,
        WALKABLE = 4,
        UNKNOWN2 = 5,
        PATH1 = 6,
        PATH2 = 7
    }

    internal enum AnimationType
    {
        MOUNT,
        RUNNING,
        WALKING,
        PHANTOM
    }

    public enum DirectionType
    {
        TOP,
        BOTTOM,
        LEFT,
        RIGHT
    }
}