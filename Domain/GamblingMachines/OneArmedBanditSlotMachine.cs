namespace Domain.GamblingMachines;

public sealed class OneArmedBanditSlotMachine : AbstractSlotMachine<SlotCardTiles>
{

    //3x3 slot machine
    private const int ColumnSize = 3;

    private readonly SlotCardTiles[] _tiles = Enum.GetValues<SlotCardTiles>(); 
    
    protected override int GetWidth()
    {
        //this is 3x3 slot machine
        return 3;
    }

    protected override SlotCardTiles[] GenerateColumn(int columnNumber)
    {
        var column = new SlotCardTiles[ColumnSize];
        for (var i = 0; i < ColumnSize; i++)
        {
            var index = Random.Next(0, _tiles.Length);
            var tile = _tiles[index];
            column[i] = tile;
        }

        return column;
    }

    protected override void EvaluateSpin(SlotMachineResult<SlotCardTiles> spin)
    {
        var first = spin[0][1];
        var second = spin[1][1];
        var third = spin[2][1];
        var multiplier = CalcMultiplier(first, second, third);
        if (multiplier <= 0) return;
        spin.AddMultiplier(new SlotMachineResult<SlotCardTiles>.MultiplierAndOrigin(multiplier, new []{1,1,1}));
    }

    private float CalcMultiplier(SlotCardTiles s1, SlotCardTiles s2, SlotCardTiles s3)
    {
        if ((MatchTiles(s1, s2) && MatchTiles(s1, s3) && MatchTiles(s2, s3)) is false) return -1;
        if (s1 == SlotCardTiles.Wild && s2 == SlotCardTiles.Wild && s3 == SlotCardTiles.Wild) return GetTileMultiplier(s1);
        return s1 != SlotCardTiles.Wild 
            ? GetTileMultiplier(s1) : s2 != SlotCardTiles.Wild 
                ? GetTileMultiplier(s2) : GetTileMultiplier(s3);
    }

    private float GetTileMultiplier(SlotCardTiles tile)
    {
        return tile switch
        {
            SlotCardTiles.Bonus => 100f,
            SlotCardTiles.Ace => 32f,
            SlotCardTiles.Two => 1.3f,
            SlotCardTiles.Three => 1.35f,
            SlotCardTiles.Four => 1.4f,
            SlotCardTiles.Five => 1.5f,
            SlotCardTiles.Six => 1.6f,
            SlotCardTiles.Seven => 7f,
            SlotCardTiles.Eight => 1.8f,
            SlotCardTiles.Nine => 1.9f,
            SlotCardTiles.Ten => 2f,
            SlotCardTiles.Knight => 4f,
            SlotCardTiles.Queen => 8f,
            SlotCardTiles.King => 16f,
            SlotCardTiles.JokerBby => 64f,
            SlotCardTiles.Wild => 50f,
            _ => throw new ArgumentOutOfRangeException(nameof(tile), tile, null)
        };
    }
    
    private bool MatchTiles(SlotCardTiles s1, SlotCardTiles s2)
    {
        return (s1 == SlotCardTiles.Wild || s2 == SlotCardTiles.Wild || s1 == s2);
    }
    
    
}