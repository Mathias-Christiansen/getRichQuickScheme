using Domain.GamblingMachines.Abstractions;

namespace Domain.GamblingMachines.VegetableFiesta;

public class VegetableFiestaSlotMachine : AbstractSlotMachine<VegetableFiestaResult>
{
    //10x10 grid
    private const int ColumnSize = 10;
    private const int MinWinCount = 6;
    private static readonly VegetableFiestaTileSet[] Tiles = Enum.GetValues<VegetableFiestaTileSet>()
        .Where(x => x != VegetableFiestaTileSet.Empty)
        .ToArray();

    
    public override VegetableFiestaResult Spin()
    {
        var board = GamblingMachineTools.GenerateInitialBoard(Tiles, ColumnSize*ColumnSize);
        var result = new VegetableFiestaResult(GamblingMachineTools.Transform2d(board, ColumnSize, ColumnSize));

        while (true)
        {
            var winningIndex = FindMatches(board);
            HandleMultiplier(winningIndex, result, board);
            if (winningIndex.Count == 0) break;
            ProgressBoard(board, winningIndex);
        }

        return result;
    }

    private static ICollection<ISet<int>> FindMatches(VegetableFiestaTileSet[] board)
    {
        var globalVisited = new HashSet<int>();
        var localVisited = new HashSet<int>();
        var winningIndexes = new List<ISet<int>>();
        for (int i = 0; i < board.Length; i++)
        {
            if(board[i] == VegetableFiestaTileSet.Wild || board[i] == VegetableFiestaTileSet.Scatter) continue;
            localVisited.Clear();
            VisitIndex(i, board[i], board, localVisited, globalVisited);
            if(localVisited.Count < MinWinCount) continue;
            globalVisited.UnionWith(localVisited.Where(x => board[x] != VegetableFiestaTileSet.Wild));
            winningIndexes.Add(new HashSet<int>(localVisited));
        }

        return winningIndexes;
    }

    private static void VisitIndex(int index, VegetableFiestaTileSet symbol, VegetableFiestaTileSet[] board, ISet<int> local, IReadOnlySet<int> global)
    {
        if(local.Contains(index) || global.Contains(index)) return;
        if(board[index] != symbol && board[index] != VegetableFiestaTileSet.Wild) return;
        local.Add(index);
        if(index >= ColumnSize) 
            VisitIndex(index - ColumnSize, symbol, board, local, global); //go up
        if(index < ((ColumnSize*ColumnSize)-ColumnSize)-1) 
            VisitIndex(index + ColumnSize, symbol, board, local, global); //go down
        if(index % ColumnSize != 0) 
            VisitIndex(index - 1, symbol, board, local, global); //go left
        if(index % ColumnSize != ColumnSize-1) 
            VisitIndex(index + 1, symbol, board, local, global); //go right

    }
}