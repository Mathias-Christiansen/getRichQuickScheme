namespace Domain.GamblingMachines;

public abstract class AbstractSlotMachine<TSlotTile> : ISlotMachine
    where TSlotTile : struct, Enum
{

    protected static readonly Random Random = new();

    public SlotMachineResult<TSlotTile> Spin()
    {
        var width = GetWidth();
        var result = new SlotMachineResult<TSlotTile>(width);
        var i = 0;
        while (true)
        {
            var column = GenerateColumn(i);
            i++;
            if (result.AddColumn(column) is false) break;
        }
        EvaluateSpin(result);
        return result;
    }

    protected abstract int GetWidth();

    protected abstract TSlotTile[] GenerateColumn(int columnNumber);

    protected abstract void EvaluateSpin(SlotMachineResult<TSlotTile> spin);
    public bool IsTileSet<TTileSet>() where TTileSet : struct, Enum
    {
        return typeof(TTileSet) == typeof(TSlotTile);
    }

    public Type GetTileSetType()
    {
        return typeof(TSlotTile);
    }
}