using System.Collections;
using Domain.GamblingMachines.Abstractions;

namespace Domain.GamblingMachines.VegetableFiesta;

public class VegetableFiestaResult : IGamblingResult, IReadOnlyCollection<VegetableFiestaState>
{
    public VegetableFiestaTileSet[][] InitialBoard { get; }
    private readonly List<VegetableFiestaState> _states = new();

    public VegetableFiestaResult(VegetableFiestaTileSet[][] initialBoard)
    {
        InitialBoard = initialBoard;
    }

    public float TotalMultiplier()
    {
        return _states.SelectMany(x => x.Multipliers).Sum();
    }

    public void AddState(VegetableFiestaState state)
    {
        _states.Add(state);
    }

    public IEnumerator<VegetableFiestaState> GetEnumerator()
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