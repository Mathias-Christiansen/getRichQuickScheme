using System.Collections;
using Domain.GamblingMachines.Abstractions;
using Domain.GamblingMachines.TileSets;

namespace Domain.GamblingMachines.PlanetoidBaroness;

public class PlanetoidBaronessResult : IGamblingResult, IReadOnlyCollection<PlanetoidBaronessState>
{
    public PlanetoidBaronessTileSet[][] InitialBoard { get; }
    private readonly List<PlanetoidBaronessState> _states = new();

    public PlanetoidBaronessResult(PlanetoidBaronessTileSet[][] initialBoard)
    {
        InitialBoard = initialBoard;
    }

    public float TotalMultiplier()
    {
        return _states.SelectMany(x => x.Multipliers).Sum();
    }

    public void AddState(PlanetoidBaronessState state)
    {
        _states.Add(state);
    }

    public IEnumerator<PlanetoidBaronessState> GetEnumerator()
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