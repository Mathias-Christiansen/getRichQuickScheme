using Domain.GamblingMachines.Abstractions;
using Domain.GamblingMachines.TileSets;

namespace Domain.GamblingMachines;

public sealed class OneArmedBanditSlotMachine : AbstractSlotMachine<OneArmedBanditTileSet>
{

    //3x3 slot machine
    private const int ColumnSize = 3;

    private readonly OneArmedBanditTileSet[] _tiles = Enum.GetValues<OneArmedBanditTileSet>(); 
    
    protected override int GetWidth()
    {
        //this is 3x3 slot machine
        return 3;
    }

    protected override OneArmedBanditTileSet[] GenerateColumn(int columnNumber)
    {
        var column = new OneArmedBanditTileSet[ColumnSize];
        for (var i = 0; i < ColumnSize; i++)
        {
            var index = Random.Next(0, _tiles.Length);
            var tile = _tiles[index];
            column[i] = tile;
        }

        return column;
    }

    protected override void EvaluateSpin(SlotMachineResult<OneArmedBanditTileSet> spin)
    {
        var first = spin[0][1];
        var second = spin[1][1];
        var third = spin[2][1];
        var multiplier = CalcMultiplier(first, second, third);
        if (multiplier <= 0) return;
        spin.AddMultiplier(new SlotMachineResult<OneArmedBanditTileSet>.MultiplierAndOrigin(multiplier, new []{1,1,1}));
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