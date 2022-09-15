using Domain.GamblingMachines.Abstractions;
using Domain.GamblingMachines.TileSets;

namespace Domain.GamblingMachines;

public class PlanetoidBaronessSlotMachine : AbstractSlotMachine<PlanetoidBaronessTileSet>
{
    
    //5x5 grid
    private const int ColumnSize = 5;
    private readonly PlanetoidBaronessTileSet[] _tiles = Enum.GetValues<PlanetoidBaronessTileSet>(); 
    
    protected override int GetWidth()
    {
        return 5;
    }

    protected override PlanetoidBaronessTileSet[] GenerateColumn(int columnNumber)
    {
        var column = new PlanetoidBaronessTileSet[ColumnSize];
        for (var i = 0; i < ColumnSize; i++)
        {
            var index = Random.Next(0, _tiles.Length);
            var tile = _tiles[index];
            column[i] = tile;
        }

        return column;
    }

    protected override void EvaluateSpin(SlotMachineResult<PlanetoidBaronessTileSet> spin)
    {
        throw new NotImplementedException();
    }
    
    private void EvaluateLine(SlotMachineResult<PlanetoidBaronessTileSet> spin, PlanetoidBaronessTileSet[] line)
    {
        if (line.All(x => x == PlanetoidBaronessTileSet.Wild))
        {
            var mult = CalcMultiplier(line.Length, PlanetoidBaronessTileSet.Wild);
            spin.AddMultiplier(new SlotMachineResult<PlanetoidBaronessTileSet>.MultiplierAndOrigin(mult, Array.Empty<int>()));
            return;
        }

        foreach (var symbol in line.Where(x=> x != PlanetoidBaronessTileSet.Wild).Distinct())
        {
            var x = EvaluateSymbol(symbol, line);
            if (x < 3) continue;
            var mult = CalcMultiplier(line.Length, symbol);
            spin.AddMultiplier(new SlotMachineResult<PlanetoidBaronessTileSet>.MultiplierAndOrigin(mult, Array.Empty<int>()));
        }
        
    }

    private static float CalcMultiplier(int length, PlanetoidBaronessTileSet symbol)
    {
        return 0f;
    }

    private static int EvaluateSymbol(PlanetoidBaronessTileSet symbol, PlanetoidBaronessTileSet[] line)
    {
        var acc = 0;
        var max = 0;
        for (int i = 0; i < line.Length-1; i++)
        {
            if (MatchTiles(line[i], symbol))
            {
                acc++;
                max = acc > max ? acc : max;
            }
            else
            {
                acc = 0;
            }
        }

        return max < 3 ? -1 : max;
    }
    
    private static bool MatchTiles(PlanetoidBaronessTileSet s1, PlanetoidBaronessTileSet s2)
    {
        return (s1 == PlanetoidBaronessTileSet.Wild || s2 == PlanetoidBaronessTileSet.Wild || s1 == s2);
    }
}