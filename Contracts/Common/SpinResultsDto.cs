namespace Contracts.Common;

public enum SlotCardTilesDto
{
    Bonus = 0,
    Ace = 1,
    Two = 2,
    Three = 3,
    Four = 4,
    Five = 5,
    Six = 6, 
    Seven = 7,
    Eight = 8,
    Nine = 9,
    Ten = 10,
    Knight = 11,
    Queen = 12,
    King = 13,
    JokerBby = 14,
    Wild = 15
}

public class SpinResultsDto<TTileSet> 
    where TTileSet : struct, Enum 
{
    public decimal NewBalance { get; set; }
    public decimal TotalWon { get; set; }
    public MultiplierAndOriginDto[] Multipliers { get; set; } = Array.Empty<MultiplierAndOriginDto>();

    public TTileSet[][] Grid { get; set; } = Array.Empty<TTileSet[]>();

}

public record MultiplierAndOriginDto(float Multiplier, int[] Path);