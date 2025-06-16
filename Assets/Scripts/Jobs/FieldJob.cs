using Harvey.Farm.FieldScripts;

public struct FieldJob
{
    public readonly FieldTile Tile;
    public readonly JobType Type;
    public readonly Field Field;

    public FieldJob(FieldTile tile, JobType type, Field field)
    {
        Tile = tile;
        Type = type;
        Field = field;
    }
}
