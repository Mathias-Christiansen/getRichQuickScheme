using Domain.GamblingMachines.Abstractions;

namespace Domain.GamblingMachines.ArrayDisarray;

public class ArrayDisarraySlotMachine : AbstractSlotMachine<ArrayDisarrayResult>
{
    //8x8 grid
    private const int ColumnSize = 8;
    private const int MinWinCount = 4;
    private static readonly ArrayDisarrayTileSet[] Tiles = Enum.GetValues<ArrayDisarrayTileSet>()
        .Where(x => x != ArrayDisarrayTileSet.Empty)
        .ToArray();

    private static readonly ArrayDisarrayTileSet[] FirePowerTiles = new[]
    {
        ArrayDisarrayTileSet.Blue, ArrayDisarrayTileSet.Green, ArrayDisarrayTileSet.Red, ArrayDisarrayTileSet.Queen,
        ArrayDisarrayTileSet.Beetle, ArrayDisarrayTileSet.Lion
    };


    
    public override ArrayDisarrayResult Spin()
    {
        var board = GamblingMachineTools.GenerateInitialBoard(Tiles, ColumnSize*ColumnSize); //TODO fix
        var result = new ArrayDisarrayResult(GamblingMachineTools.Transform2d(board, ColumnSize, ColumnSize));
        var powerSet = new HashSet<int>();
        
        while (true)
        {
            if (powerSet.Count > 0)
            {
                ActivatePowers(board, powerSet);
                powerSet.Clear();
            }
            var winningIndex = FindMatches(board);
            HandleMultiplier(winningIndex, result, board);
            if (winningIndex.Count == 0) break;
            
            ProgressBoard(board, winningIndex, powerSet);
        }

        return result;
    }

    private static void ActivatePowers(ArrayDisarrayTileSet[] board, ICollection<int> powerSet)
    {
        foreach (var power in powerSet)
        {
            var symbol = board[power];
            if (symbol == ArrayDisarrayTileSet.StoneLightning)
            {
                var set = new HashSet<int>();
                var count = GamblingMachineTools.Rand.Next(1, 12);
                while (count > 0)
                {
                    var index = GamblingMachineTools.Rand.Next(0, board.Length);
                    if(set.Contains(index)) continue;
                    count--;
                    set.Add(index);
                    board[index] = ArrayDisarrayTileSet.Wild;
                }

                board[power] = ArrayDisarrayTileSet.Wild;
            }
            else if (symbol == ArrayDisarrayTileSet.StoneFire)
            {
                var newSymbol = FirePowerTiles[GamblingMachineTools.Rand.Next(0, FirePowerTiles.Length)];
                var oldSymbol1 = FirePowerTiles[GamblingMachineTools.Rand.Next(0, FirePowerTiles.Length)];
                var oldSymbol2 = FirePowerTiles[GamblingMachineTools.Rand.Next(0, FirePowerTiles.Length)];
                for (int i = 0; i < board.Length; i++)
                {
                    if (board[i] == oldSymbol1 || board[i] == oldSymbol2) board[i] = newSymbol;
                }

                board[power] = newSymbol;
            }
        }
    }

    private static void ProgressBoard(ArrayDisarrayTileSet[] board, ICollection<ISet<int>> winningIndex, ICollection<int> powerSet)
    {
        foreach (var index in winningIndex.SelectMany(x => x))
        {
            board[index] = ArrayDisarrayTileSet.Empty;
            ClearStones(board, index, powerSet);
        }
        
        for (int i = board.Length - 1; i >= ColumnSize; i--)
        {
            if (board[i] != ArrayDisarrayTileSet.Empty) continue;
            var swindex = i;
            for (int j = i-ColumnSize; j >= 0; j-=ColumnSize)
            {
                if (board[j] == ArrayDisarrayTileSet.Empty || IsStone(board[j])) continue;
                swindex = j;
                break;
            }
            if (swindex == i) continue;
            board[i] = board[swindex];
            board[swindex] = ArrayDisarrayTileSet.Empty;
        }

        for (int i = 0; i < ColumnSize*ColumnSize; i++)
        {
            if (board[i] != ArrayDisarrayTileSet.Empty) continue;
            board[i] = Tiles[GamblingMachineTools.Rand.Next(0, Tiles.Length)];
        }
    }

    private static void ClearStones(ArrayDisarrayTileSet[] board, int index, ICollection<int> powerSet)
    {
        bool up = index >= ColumnSize && IsStone(board[index-ColumnSize]);
        bool right = index % ColumnSize != ColumnSize - 1 && IsStone(board[index+1]);
        bool down = index < ((ColumnSize * ColumnSize) - ColumnSize) - 1 && IsStone(board[index+ColumnSize]);
        bool left = index % ColumnSize != 0 && IsStone(board[index-1]);

        if (up)
        {
            if (board[index - ColumnSize] == ArrayDisarrayTileSet.StoneFire ||
                board[index - ColumnSize] == ArrayDisarrayTileSet.StoneLightning)
                powerSet.Add(index-ColumnSize);
            else board[index - ColumnSize] = ArrayDisarrayTileSet.Empty;
        }

        if (right)
        {
            if (board[index + 1] == ArrayDisarrayTileSet.StoneFire ||
                board[index + 1] == ArrayDisarrayTileSet.StoneLightning)
                powerSet.Add(index+1);
            else board[index + 1] = ArrayDisarrayTileSet.Empty;
        }
        if(down) 
            if (board[index + ColumnSize] == ArrayDisarrayTileSet.StoneFire ||
                board[index + ColumnSize] == ArrayDisarrayTileSet.StoneLightning)
                powerSet.Add(index+ColumnSize);
            else board[index + ColumnSize] = ArrayDisarrayTileSet.Empty;
        if(left) 
            if (board[index - 1] == ArrayDisarrayTileSet.StoneFire ||
                board[index - 1] == ArrayDisarrayTileSet.StoneLightning)
                powerSet.Add(index-1);
            else board[index - 1] = ArrayDisarrayTileSet.Empty;
    }

    private static bool IsStone(ArrayDisarrayTileSet symbol)
    {
        return symbol is ArrayDisarrayTileSet.Stone or ArrayDisarrayTileSet.StoneFire or ArrayDisarrayTileSet.StoneLightning;
    }
    
    private static void HandleMultiplier(ICollection<ISet<int>> winningIndex, ArrayDisarrayResult result,
        ArrayDisarrayTileSet[] board)
    {
        var multipliers = new List<float>();
        foreach (var winSet in winningIndex)
        {
            var symbol = winSet
                .Select(x => board[x])
                .FirstOrDefault(x => x != ArrayDisarrayTileSet.Wild, ArrayDisarrayTileSet.Wild);
            multipliers.Add(CalcMultiplier(winSet.Count, symbol));
        }
        result.AddState(new ArrayDisarrayState(
            GamblingMachineTools.Transform2d(board, ColumnSize, ColumnSize), 
            multipliers.Where(x => x > 0).ToArray())
        );
    }

    private static float CalcMultiplier(int count, ArrayDisarrayTileSet symbol)
    {
        if (count < MinWinCount) return -1f;
        var localCount = (int)Math.Pow(count - MinWinCount + 1, 2);
        return (symbol switch
        {
            ArrayDisarrayTileSet.Empty => -1f,
            ArrayDisarrayTileSet.Stone => -1f,
            ArrayDisarrayTileSet.Wild => -1f,
            ArrayDisarrayTileSet.StoneLightning => -1f,
            ArrayDisarrayTileSet.StoneFire => -1f,
            ArrayDisarrayTileSet.Blue => 0.3f,
            ArrayDisarrayTileSet.Green => 0.3f,
            ArrayDisarrayTileSet.Red => 0.3f,
            ArrayDisarrayTileSet.Queen => 0.4f,
            ArrayDisarrayTileSet.Beetle => 0.4f,
            ArrayDisarrayTileSet.Lion => 1f,
            ArrayDisarrayTileSet.Scatter => -1f,
            _ => throw new ArgumentOutOfRangeException(nameof(symbol), symbol, null)
        }) * localCount;
    }

    private static ICollection<ISet<int>> FindMatches(ArrayDisarrayTileSet[] board)
    {
        var globalVisited = new HashSet<int>();
        var localVisited = new HashSet<int>();
        var winningIndexes = new List<ISet<int>>();
        for (int i = 0; i < board.Length; i++)
        {
            if(board[i] == ArrayDisarrayTileSet.Wild || board[i] == ArrayDisarrayTileSet.Scatter) continue;
            
            localVisited.Clear();
            VisitIndex(i, board[i], board, localVisited, globalVisited);
            if(localVisited.Count < MinWinCount) continue;
            
            globalVisited.UnionWith(localVisited.Where(x => board[x] != ArrayDisarrayTileSet.Wild));
            winningIndexes.Add(new HashSet<int>(localVisited));
        }

        return winningIndexes;
    }

    private static void VisitIndex(int index, ArrayDisarrayTileSet symbol, ArrayDisarrayTileSet[] board, ISet<int> local, IReadOnlySet<int> global)
    {
        if(local.Contains(index) || global.Contains(index)) return;
        if(board[index] != symbol && board[index] != ArrayDisarrayTileSet.Wild) return;
        local.Add(index);

        bool up = index >= ColumnSize;
        bool right = index % ColumnSize != ColumnSize - 1;
        bool down = index < ((ColumnSize * ColumnSize) - ColumnSize) - 1;
        bool left = index % ColumnSize != 0;
        
        if(up) 
            VisitIndex(index - ColumnSize, symbol, board, local, global); //go up
        if(up && right)
            VisitIndex(index - ColumnSize + 1, symbol, board, local, global); //go up-right
        if(right) 
            VisitIndex(index + 1, symbol, board, local, global); //go right
        if(right && down)
            VisitIndex(index + ColumnSize + 1, symbol, board, local, global); //go down-rigth
        if(down) 
            VisitIndex(index + ColumnSize, symbol, board, local, global); //go down
        if(down && left)
            VisitIndex(index + ColumnSize - 1, symbol, board, local, global); //go down-left
        if(left) 
            VisitIndex(index - 1, symbol, board, local, global); //go left
        if(left && up)
            VisitIndex(index - ColumnSize - 1, symbol, board, local, global); //go up-left

    }
}