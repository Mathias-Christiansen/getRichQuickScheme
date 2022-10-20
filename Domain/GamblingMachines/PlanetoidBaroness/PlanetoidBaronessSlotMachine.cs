using Domain.GamblingMachines.Abstractions;
using Domain.GamblingMachines.PlanetoidBaroness;
using Domain.GamblingMachines.TileSets;
using OneOf.Types;

namespace Domain.GamblingMachines;

public class PlanetoidBaronessSlotMachine : AbstractSlotMachine<PlanetoidBaronessResult>
{
    
    //5x5 grid
    private const int ColumnSize = 5;
    private const int MinWinCount = 3;
    private static readonly PlanetoidBaronessTileSet[] Tiles = Enum.GetValues<PlanetoidBaronessTileSet>()
        .Where(x => x != PlanetoidBaronessTileSet.Empty)
        .ToArray();


    public override PlanetoidBaronessResult Spin()
    {
        var board = GamblingMachineTools.GenerateInitialBoard(Tiles, ColumnSize*ColumnSize);
        var result = new PlanetoidBaronessResult(GamblingMachineTools.Transform2d(board, ColumnSize, ColumnSize));
        var round = 0;
        
        while (true)
        {
            round++;
            var winningIndex = FindMatches(board);
            HandleMultiplier(winningIndex, result, board, round);
            if (winningIndex.Count == 0) break;
            ProgressBoard(board, winningIndex);
        }

        return result;
    }
    
    private static void ProgressBoard(PlanetoidBaronessTileSet[] board, ICollection<ISet<int>> winningIndex)
    {
        foreach (var index in winningIndex.SelectMany(x => x))
        {
            board[index] = PlanetoidBaronessTileSet.Empty;
        }

        foreach (var winSet in winningIndex)
        {
            if (winSet.Count != MinWinCount) continue;
            var index = winSet.OrderBy(x => x).Skip(1).First();
            board[index] = PlanetoidBaronessTileSet.Wild;
        }

        for (int i = board.Length - 1; i >= ColumnSize; i--)
        {
            if (board[i] != PlanetoidBaronessTileSet.Empty) continue;
            var swindex = i;
            for (int j = i-ColumnSize; j >= 0; j-=ColumnSize)
            {
                if (board[j] == PlanetoidBaronessTileSet.Empty) continue;
                swindex = j;
                break;
            }
            if (swindex == i) continue;
            board[i] = board[swindex];
            board[swindex] = PlanetoidBaronessTileSet.Empty;
        }
    }

    
    private static void EvaluateLine(ICollection<ISet<int>> bigFatW,IEnumerable<(PlanetoidBaronessTileSet tile, int index)> line)
    {
        var list = line as (PlanetoidBaronessTileSet tile, int index)[] ?? line.ToArray();
        if (list.Select(x => x.tile).All(x => x == PlanetoidBaronessTileSet.Empty)) return;

        if (list.Select(x => x.tile).All(x => x == PlanetoidBaronessTileSet.Wild))
        {
            bigFatW.Add(new HashSet<int>(list.Select(y => y.index)));
            return;
        } 
        
        foreach (var (tile, _) in list
                     .Where(x=> x.tile != PlanetoidBaronessTileSet.Wild && x.tile != PlanetoidBaronessTileSet.Empty)
                     .DistinctBy(y => y.tile))
        {
            var wins = CountMatches(tile, list);
            if (wins.Count < MinWinCount) continue;
            bigFatW.Add(wins);
        }
        
    }

    private static ICollection<ISet<int>> FindMatches(PlanetoidBaronessTileSet[] board)
    {
        var winningIndexes = new List<ISet<int>>();
        for (int i = 0; i < ColumnSize; i++)
        {
            EvaluateLine(winningIndexes, board.Select((tile, index) => (tile, index)).Skip(i*ColumnSize).Take(ColumnSize));
        }

        for (int j = 0; j < ColumnSize; j++)
        {
            EvaluateLine(winningIndexes,
                board.Select((tile, index) => (tile, index)).Where((_, i) => (i - j) % ColumnSize == 0));
        }

        return winningIndexes;

    }

    private static void HandleMultiplier(ICollection<ISet<int>> winningIndex, PlanetoidBaronessResult result, PlanetoidBaronessTileSet[] board, int round)
    {
        var multipliers = new List<float>();
        foreach (var winSet in winningIndex)
        {
            var symbol = winSet
                .Select(x => board[x])
                .FirstOrDefault(x => x != PlanetoidBaronessTileSet.Wild, PlanetoidBaronessTileSet.Wild);
            multipliers.Add(CalcMultiplier(winSet.Count, symbol) * round);
        }
        result.AddState(new PlanetoidBaronessState(GamblingMachineTools.Transform2d(board, ColumnSize, ColumnSize), multipliers.Where(x => x > 0).ToArray()));
    }

    private static float CalcMultiplier(int length, PlanetoidBaronessTileSet symbol)
    {

        switch ((symbol, length))
        {
            case (PlanetoidBaronessTileSet.Wild, 5): return 100.0f;
            case (PlanetoidBaronessTileSet.Love, 5): return 20.0f;
            case (PlanetoidBaronessTileSet.Love, 4): return 2.0f;
            case (PlanetoidBaronessTileSet.Love, 3): return 0.6f;
            case (PlanetoidBaronessTileSet.Star, 5): return 20.0f;
            case (PlanetoidBaronessTileSet.Star, 4): return 2.0f;
            case (PlanetoidBaronessTileSet.Star, 3): return 0.6f;
            case (PlanetoidBaronessTileSet.Storm, 5): return 20.0f;
            case (PlanetoidBaronessTileSet.Storm, 4): return 2.0f;
            case (PlanetoidBaronessTileSet.Storm, 3): return 0.6f;
            case (PlanetoidBaronessTileSet.Bell, 5): return 6.0f;
            case (PlanetoidBaronessTileSet.Bell, 4): return 0.6f;
            case (PlanetoidBaronessTileSet.Bell, 3): return 0.3f;
            case (PlanetoidBaronessTileSet.Heart, 5): return 6.0f;
            case (PlanetoidBaronessTileSet.Heart, 4): return 0.6f;
            case (PlanetoidBaronessTileSet.Heart, 3): return 0.3f;
            case (PlanetoidBaronessTileSet.Moon, 5): return 4.0f;
            case (PlanetoidBaronessTileSet.Moon, 4): return 0.4f;
            case (PlanetoidBaronessTileSet.Moon, 3): return 0.2f;
            case (PlanetoidBaronessTileSet.Circle, 5): return 4.0f;
            case (PlanetoidBaronessTileSet.Circle, 4): return 0.4f;
            case (PlanetoidBaronessTileSet.Circle, 3): return 0.2f;
            default: return -1.0f;
        }
    }

    private static ISet<int> CountMatches(PlanetoidBaronessTileSet symbol, IEnumerable<(PlanetoidBaronessTileSet tile, int index)> line)
    {
        var localResults = new HashSet<int>();

        foreach (var (tile, index) in line)
        {
            if (MatchTiles(symbol, tile)) localResults.Add(index);
            else if (localResults.Count >= MinWinCount) return localResults;
            else localResults.Clear();
        }

        return localResults;
    }
    
    
    private static bool MatchTiles(PlanetoidBaronessTileSet s1, PlanetoidBaronessTileSet s2)
    {
        return (s1 == PlanetoidBaronessTileSet.Wild || s2 == PlanetoidBaronessTileSet.Wild || s1 == s2);
    }
}