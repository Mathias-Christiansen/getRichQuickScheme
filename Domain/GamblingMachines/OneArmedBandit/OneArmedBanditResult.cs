using System.Collections;
using Domain.GamblingMachines.Abstractions;
using Domain.GamblingMachines.TileSets;

namespace Domain.GamblingMachines.OneArmedBandit;

public class OneArmedBanditResult : IGamblingResult, IReadOnlyList<OneArmedBanditTileSet[]>
{
    
    private readonly OneArmedBanditTileSet[][] _store;
    private int _columnCount = 0;
    private readonly List<MultiplierAndOrigin> _multipliers = new();

    
    public OneArmedBanditResult(int width)
    {
        _store = new OneArmedBanditTileSet[width][];
    }
    public record struct MultiplierAndOrigin(float Multiplier, int[] Path);
    
    internal bool AddColumn(OneArmedBanditTileSet[] array)
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

    public IEnumerator<OneArmedBanditTileSet[]> GetEnumerator()
    {
        return _store.Cast<OneArmedBanditTileSet[]>().GetEnumerator();
    }
    
    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public int Count => _columnCount;

    public OneArmedBanditTileSet[] this[int index] => _store[index];
    
    public float TotalMultiplier()
    {
        return _multipliers.Sum(x => x.Multiplier);
    }

}