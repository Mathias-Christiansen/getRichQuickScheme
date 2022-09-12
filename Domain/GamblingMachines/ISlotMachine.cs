namespace Domain.GamblingMachines;

public interface ISlotMachine
{
    public bool IsTileSet<TTileSet>()
        where TTileSet : struct, Enum;

    public Type GetTileSetType();
}