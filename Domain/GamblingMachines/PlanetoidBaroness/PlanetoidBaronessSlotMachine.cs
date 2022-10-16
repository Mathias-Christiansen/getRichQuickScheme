using Domain.GamblingMachines.Abstractions;
using Domain.GamblingMachines.PlanetoidBaroness;
using Domain.GamblingMachines.TileSets;
using OneOf.Types;

namespace Domain.GamblingMachines;

public class PlanetoidBaronessSlotMachine : AbstractSlotMachine<PlanetoidBaronessResult>
{
    
    //5x5 grid
    private const int ColumnSize = 5;
    private static readonly PlanetoidBaronessTileSet[] _tiles = Enum.GetValues<PlanetoidBaronessTileSet>();
    private static readonly Random Rand = new();

    private static PlanetoidBaronessTileSet[][] GenerateInitialBoard()
    {
        var array = new PlanetoidBaronessTileSet[][ColumnSize];

        for (int i = 0; i < ColumnSize; i++)
        {
            array[i] = new PlanetoidBaronessTileSet[ColumnSize];
            for (int j = 0; j < ColumnSize; j++)
            {
                var index = Rand.Next(1, _tiles.Length);
                var tile = _tiles[index];
                array[i][j] = tile;
            }
        }

        return array;
    }

    public PlanetoidBaronessResult Spin()
    {
        var board = GenerateInitialBoard();
        var result = new PlanetoidBaronessResult(board);
        
        while (true)
        {
            var (nextState, state) = ProgressBoard(board);
            if (!didWin(state)) break;
            board = nextState;
            result.AddState(state);
        }

        return result;
    }

    private (PlanetoidBaronessTileSet[][] nextState, PlanetoidBaronessState state) ProgressBoard(PlanetoidBaronessTileSet[][] board)
    {
        
    }

    private bool didWin(PlanetoidBaronessState state)
    {
        return true; //todo
    }
    
    private (float? r1, float? r2) EvaluateLine(Dictionary<int, PlanetoidBaronessTileSet> transform, Func<int, int> indexFunc, PlanetoidBaronessTileSet[] line)
    {
        if (line.All(x => x == PlanetoidBaronessTileSet.Empty)) return (null, null);
        
        if (line.All(x => x == PlanetoidBaronessTileSet.Wild))
        {
            for (int i = 0; i < line.Length; i++)
            {
                transform[indexFunc.Invoke(i)] = transform.TryGetValue(indexFunc.Invoke(i), out var c)
                    ? c
                    : PlanetoidBaronessTileSet.Empty;
            }
            
            return (CalcMultiplier(line.Length, PlanetoidBaronessTileSet.Wild), null);
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