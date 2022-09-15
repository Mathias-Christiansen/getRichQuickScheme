using Domain.GamblingMachines.Abstractions;
using Domain.GamblingMachines.TileSets;

namespace Domain.GamblingMachines;

public class ThreeArmedBanditSlotMachine : AbstractSlotMachine<ThreeArmedBanditTileSet>
{

    //3x3 slot machine
    private const int ColumnSize = 3;

    private readonly ThreeArmedBanditTileSet[] _tiles = Enum.GetValues<ThreeArmedBanditTileSet>(); 
    
    protected override int GetWidth()
    {
        //this is 3x3 slot machine
        return 3;
    }

    protected override ThreeArmedBanditTileSet[] GenerateColumn(int columnNumber)
    {
        var column = new ThreeArmedBanditTileSet[ColumnSize];
        for (var i = 0; i < ColumnSize; i++)
        {
            var index = Random.Next(0, _tiles.Length);
            var tile = _tiles[index];
            column[i] = tile;
        }

        return column;
    }

    protected override void EvaluateSpin(SlotMachineResult<ThreeArmedBanditTileSet> spin)
    {
        EvaluateLine(spin, 0, 0,0 );
        EvaluateLine(spin, 1, 1,1 );
        EvaluateLine(spin, 2, 2,2 );
        EvaluateLine(spin, 0, 1,2 );
        EvaluateLine(spin, 2, 1,0 );
    }

    private void EvaluateLine(SlotMachineResult<ThreeArmedBanditTileSet> spin, int x, int y, int z)
    {
        var first = spin[0][x];
        var second = spin[1][y];
        var third = spin[2][z];
        var multiplier = CalcMultiplier(first, second, third);
        if (multiplier <= 0) return;
        spin.AddMultiplier(new SlotMachineResult<ThreeArmedBanditTileSet>.MultiplierAndOrigin(multiplier, new []{x,y,z}));
    }

    private float CalcMultiplier(ThreeArmedBanditTileSet s1, ThreeArmedBanditTileSet s2, ThreeArmedBanditTileSet s3)
    {
        if ((MatchTiles(s1, s2) && MatchTiles(s1, s3) && MatchTiles(s2, s3)) is false) return -1;
        if (s1 == ThreeArmedBanditTileSet.Wild && s2 == ThreeArmedBanditTileSet.Wild && s3 == ThreeArmedBanditTileSet.Wild) return GetTileMultiplier(s1);
        return s1 != ThreeArmedBanditTileSet.Wild 
            ? GetTileMultiplier(s1) : s2 != ThreeArmedBanditTileSet.Wild 
                ? GetTileMultiplier(s2) : GetTileMultiplier(s3);
    }

    private float GetTileMultiplier(ThreeArmedBanditTileSet tile)
    {
        return tile switch
        {
            ThreeArmedBanditTileSet.Bonus => 100f,
            ThreeArmedBanditTileSet.Ace => 32f,
            ThreeArmedBanditTileSet.Two => 0.3f,
            ThreeArmedBanditTileSet.Three => 0.35f,
            ThreeArmedBanditTileSet.Four => 0.4f,
            ThreeArmedBanditTileSet.Five => 0.5f,
            ThreeArmedBanditTileSet.Six => 0.6f,
            ThreeArmedBanditTileSet.Seven => 7f,
            ThreeArmedBanditTileSet.Eight => 0.8f,
            ThreeArmedBanditTileSet.Nine => 0.9f,
            ThreeArmedBanditTileSet.Ten => 1f,
            ThreeArmedBanditTileSet.Knight => 4f,
            ThreeArmedBanditTileSet.Queen => 8f,
            ThreeArmedBanditTileSet.King => 16f,
            ThreeArmedBanditTileSet.JokerBby => 64f,
            ThreeArmedBanditTileSet.Wild => 50f,
            _ => throw new ArgumentOutOfRangeException(nameof(tile), tile, null)
        };
    }
    
    private bool MatchTiles(ThreeArmedBanditTileSet s1, ThreeArmedBanditTileSet s2)
    {
        return (s1 == ThreeArmedBanditTileSet.Wild || s2 == ThreeArmedBanditTileSet.Wild || s1 == s2);
    }
    
    
}