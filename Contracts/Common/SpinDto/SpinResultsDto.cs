namespace Contracts.Common.SpinDto;

public class SpinResultsDto<TTileSet> 
    where TTileSet : struct, Enum 
{
    public decimal NewBalance { get; set; }
    public decimal TotalWon { get; set; }
    public MultiplierAndOriginDto[] Multipliers { get; set; } = Array.Empty<MultiplierAndOriginDto>();

    public TTileSet[][] Grid { get; set; } = Array.Empty<TTileSet[]>();

}

public record MultiplierAndOriginDto(float Multiplier, int[] Path);