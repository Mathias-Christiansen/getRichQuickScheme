using Domain.GamblingMachines.Abstractions;

namespace Domain.GamblingMachines.OneArmedBandit;

public sealed class OneArmedBanditSlotMachine : AbstractSlotMachine<OneArmedBanditResult>
{

    private static readonly Random Rand = new();

    //3x3 slot machine
    private const int ColumnSize = 3;

    private readonly OneArmedBanditTileSet[] _tiles = Enum.GetValues<OneArmedBanditTileSet>(); 
    
    private static int GetWidth()
    {
        //this is 3x3 slot machine
        return 3;
    }

    private OneArmedBanditTileSet[] GenerateColumn()
    {
        var column = new OneArmedBanditTileSet[ColumnSize];
        for (var i = 0; i < ColumnSize; i++)
        {
            var index = Rand.Next(0, _tiles.Length);
            var tile = _tiles[index];
            column[i] = tile;
        }

        return column;
    }

    public override OneArmedBanditResult Spin()
    {
        var width = GetWidth();
        var result = new OneArmedBanditResult(width);
        while (true)
        {
            var column = GenerateColumn();
            if (result.AddColumn(column) is false) break;
        }
        EvaluateSpin(result);
        return result;
    }

    private void EvaluateSpin(OneArmedBanditResult spin)
    {
        var first = spin[0][1];
        var second = spin[1][1];
        var third = spin[2][1];
        var multiplier = CalcMultiplier(first, second, third);
        if (multiplier <= 0) return;
        spin.AddMultiplier(new OneArmedBanditResult.MultiplierAndOrigin(multiplier, new []{1,1,1}));
    }

    private float CalcMultiplier(OneArmedBanditTileSet s1, OneArmedBanditTileSet s2, OneArmedBanditTileSet s3)
    {
        if ((MatchTiles(s1, s2) && MatchTiles(s1, s3) && MatchTiles(s2, s3)) is false) return -1;
        if (s1 == OneArmedBanditTileSet.Wild && s2 == OneArmedBanditTileSet.Wild && s3 == OneArmedBanditTileSet.Wild) return GetTileMultiplier(s1);
        return s1 != OneArmedBanditTileSet.Wild 
            ? GetTileMultiplier(s1) : s2 != OneArmedBanditTileSet.Wild 
                ? GetTileMultiplier(s2) : GetTileMultiplier(s3);
    }

    private float GetTileMultiplier(OneArmedBanditTileSet tile)
    {
        return tile switch
        {
            OneArmedBanditTileSet.Bonus => 100f,
            OneArmedBanditTileSet.Ace => 32f,
            OneArmedBanditTileSet.Seven => 7f,
            OneArmedBanditTileSet.Ten => 2f,
            OneArmedBanditTileSet.Knight => 4f,
            OneArmedBanditTileSet.Queen => 8f,
            OneArmedBanditTileSet.King => 16f,
            OneArmedBanditTileSet.JokerBby => 64f,
            OneArmedBanditTileSet.Wild => 50f,
            _ => throw new ArgumentOutOfRangeException(nameof(tile), tile, null)
        };
    }
    
    private bool MatchTiles(OneArmedBanditTileSet s1, OneArmedBanditTileSet s2)
    {
        return (s1 == OneArmedBanditTileSet.Wild || s2 == OneArmedBanditTileSet.Wild || s1 == s2);
    }
    
    
}