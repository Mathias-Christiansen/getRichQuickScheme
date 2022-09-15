using System.Collections;

namespace Domain.GamblingMachines.Abstractions;

public class SlotMachineResult<TSlotTile> : IReadOnlyList<TSlotTile[]>
    where TSlotTile : struct, Enum
{

    public record struct MultiplierAndOrigin(float Multiplier, int[] Path);
    
    
    private readonly TSlotTile[][] _store;
    private int _columnCount = 0;
    private readonly List<MultiplierAndOrigin> _multipliers = new();

    public SlotMachineResult(int width)
    {
        _store = new TSlotTile[width][];
    }

    public float SumMultiplier() => _multipliers.Select(x => x.Multiplier).Sum();

    internal void AddMultiplier(MultiplierAndOrigin multiplier)
    {
        _multipliers.Add(multiplier);
    }

    public IReadOnlyList<MultiplierAndOrigin> GetMultipliers() => _multipliers;

    internal bool AddColumn(TSlotTile[] array)
    {
        if (_columnCount >= _store.Length) return false;
        _store[_columnCount] = array;
        _columnCount++;
        return _columnCount < _store.Length;
    }

    public IEnumerator<TSlotTile[]> GetEnumerator()
    {
        return _store.Cast<TSlotTile[]>().GetEnumerator();
    }
    
    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public int Count => _columnCount;

    public TSlotTile[] this[int index] => _store[index];
}