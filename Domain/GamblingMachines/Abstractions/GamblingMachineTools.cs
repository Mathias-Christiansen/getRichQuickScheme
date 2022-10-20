namespace Domain.GamblingMachines.Abstractions;

internal static class GamblingMachineTools
{
    
    public static readonly Random Rand = new();
    
    public static T[] GenerateInitialBoard<T>(IReadOnlyList<T> tiles, int size)
    {
        var array = new T[size];

        for (int i = 0; i < size; i++)
        {
            var tile = tiles[Rand.Next(1, tiles.Count)];
            array[i] = tile;
        }

        return array;
    }
    
    public static T[][] Transform2d<T>(IReadOnlyList<T> board, int columnSize, int rowSize)
    {
        var output = new T[rowSize][];
        for (int i = 0; i < rowSize; i++)
        {
            output[i] = new T[columnSize];
            
        }
        for (int i = 0; i < rowSize; i++)
        {
            for (int j = 0; j < columnSize; j++)
            {
                output[j][i] = board[i * rowSize + j];
            }
        }

        return output;
    }
}