using System.Collections;
using Domain.GamblingMachines.Abstractions;
using Domain.GamblingMachines.TileSets;

namespace Domain.GamblingMachines.ThreeLeggedBandit;

public class ThreeLeggedBanditResult : IGamblingResult, IReadOnlyList<ThreeLeggedBanditTileSet[]>
{
    private readonly ThreeLeggedBanditTileSet[][] _store;
    private int _columnCount = 0;
    private readonly List<MultiplierAndOrigin> _multipliers = new();

    
    public ThreeLeggedBanditResult(int width)
    {
        _store = new ThreeLeggedBanditTileSet[width][];
    }
    public record struct MultiplierAndOrigin(float Multiplier, int[] Path);
    
    internal bool AddColumn(ThreeLeggedBanditTileSet[] array)
    {
        if (_columnCount >= _store.Length) return false;
        _store[_columnCount] = array;
        _columnCount++;
        return _columnCount < _store.Length;
    }

    public void AddMultiplier(MultiplierAndOrigin mult)
    {
        _multipliers.Add(mult);
    }

    public IEnumerator<ThreeLeggedBanditTileSet[]> GetEnumerator()
    {
        return _store.Cast<ThreeLeggedBanditTileSet[]>().GetEnumerator();
    }
    
    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public int Count => _columnCount;

    public ThreeLeggedBanditTileSet[] this[int index] => _store[index];
    
    public float TotalMultiplier()
    {
        return _multipliers.Sum(x => x.Multiplier);
    }
}