using Domain.GamblingMachines.Abstractions;

namespace Domain.GamblingMachines.ThreeLeggedBandit;

public class ThreeLeggedBanditSlotMachine : AbstractSlotMachine<ThreeLeggedBanditResult>
{

    private static readonly Random Rand = new();
    
    //3x3 slot machine
    private const int ColumnSize = 3;

    private readonly ThreeLeggedBanditTileSet[] _tiles = Enum.GetValues<ThreeLeggedBanditTileSet>(); 
    
    private int GetWidth()
    {
        //this is 3x3 slot machine
        return 3;
    }

    private ThreeLeggedBanditTileSet[] GenerateColumn()
    {
        var column = new ThreeLeggedBanditTileSet[ColumnSize];
        for (var i = 0; i < ColumnSize; i++)
        {
            var index = Rand.Next(0, _tiles.Length);
            var tile = _tiles[index];
            column[i] = tile;
        }

        return column;
    }
    
    public override ThreeLeggedBanditResult Spin()
    {
        var width = GetWidth();
        var result = new ThreeLeggedBanditResult(width);
        while (true)
        {
            var column = GenerateColumn();
            if (result.AddColumn(column) is false) break;
        }
        EvaluateSpin(result);
        return result;
    }

    private void EvaluateSpin(ThreeLeggedBanditResult spin)
    {
        EvaluateLine(spin, 0, 0,0 );
        EvaluateLine(spin, 1, 1,1 );
        EvaluateLine(spin, 2, 2,2 );
        EvaluateLine(spin, 0, 1,2 );
        EvaluateLine(spin, 2, 1,0 );
    }

    private void EvaluateLine(ThreeLeggedBanditResult spin, int x, int y, int z)
    {
        var first = spin[0][x];
        var second = spin[1][y];
        var third = spin[2][z];
        var multiplier = CalcMultiplier(first, second, third);
        if (multiplier <= 0) return;
        spin.AddMultiplier(new ThreeLeggedBanditResult.MultiplierAndOrigin(multiplier, new []{x,y,z}));
    }

    private float CalcMultiplier(ThreeLeggedBanditTileSet s1, ThreeLeggedBanditTileSet s2, ThreeLeggedBanditTileSet s3)
    {
        if ((MatchTiles(s1, s2) && MatchTiles(s1, s3) && MatchTiles(s2, s3)) is false) return -1;
        if (s1 == ThreeLeggedBanditTileSet.Wild && s2 == ThreeLeggedBanditTileSet.Wild && s3 == ThreeLeggedBanditTileSet.Wild) return GetTileMultiplier(s1);
        return s1 != ThreeLeggedBanditTileSet.Wild 
            ? GetTileMultiplier(s1) : s2 != ThreeLeggedBanditTileSet.Wild 
                ? GetTileMultiplier(s2) : GetTileMultiplier(s3);
    }

    private float GetTileMultiplier(ThreeLeggedBanditTileSet tile)
    {
        return tile switch
        {
            ThreeLeggedBanditTileSet.Bonus => 100f,
            ThreeLeggedBanditTileSet.Ace => 32f,
            ThreeLeggedBanditTileSet.Two => 0.3f,
            ThreeLeggedBanditTileSet.Three => 0.35f,
            ThreeLeggedBanditTileSet.Four => 0.4f,
            ThreeLeggedBanditTileSet.Five => 0.5f,
            ThreeLeggedBanditTileSet.Six => 0.6f,
            ThreeLeggedBanditTileSet.Seven => 7f,
            ThreeLeggedBanditTileSet.Eight => 0.8f,
            ThreeLeggedBanditTileSet.Nine => 0.9f,
            ThreeLeggedBanditTileSet.Ten => 1f,
            ThreeLeggedBanditTileSet.Knight => 4f,
            ThreeLeggedBanditTileSet.Queen => 8f,
            ThreeLeggedBanditTileSet.King => 16f,
            ThreeLeggedBanditTileSet.JokerBby => 64f,
            ThreeLeggedBanditTileSet.Wild => 50f,
            _ => throw new ArgumentOutOfRangeException(nameof(tile), tile, null)
        };
    }
    
    private bool MatchTiles(ThreeLeggedBanditTileSet s1, ThreeLeggedBanditTileSet s2)
    {
        return (s1 == ThreeLeggedBanditTileSet.Wild || s2 == ThreeLeggedBanditTileSet.Wild || s1 == s2);
    }
    
    
}