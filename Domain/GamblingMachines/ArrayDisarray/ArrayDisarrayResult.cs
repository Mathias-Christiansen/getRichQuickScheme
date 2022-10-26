using System.Collections;
using Domain.GamblingMachines.Abstractions;
using Domain.GamblingMachines.ArrayDisarray;

namespace Domain.GamblingMachines.ArrayDisarray;

public class ArrayDisarrayResult : IGamblingResult, IReadOnlyCollection<ArrayDisarrayState>
{
    public ArrayDisarrayTileSet[][] InitialBoard { get; }
    private readonly List<ArrayDisarrayState> _states = new();

    public ArrayDisarrayResult(ArrayDisarrayTileSet[][] initialBoard)
    {
        InitialBoard = initialBoard;
    }

    public float TotalMultiplier()
    {
        return _states.SelectMany(x => x.Multipliers).Sum();
    }

    public void AddState(ArrayDisarrayState state)
    {
        _states.Add(state);
    }

    public IEnumerator<ArrayDisarrayState> GetEnumerator()
    {
        return _states.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public int Count => _states.Count;

    public IEnumerable<float> GetMultipliers()
    {
        return _states.SelectMany(x => x.Multipliers);
    }
    
}