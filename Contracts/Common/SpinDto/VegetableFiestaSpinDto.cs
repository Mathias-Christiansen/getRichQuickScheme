using Contracts.Common.TileSetsDto;

namespace Contracts.Common.SpinDto;

public class VegetableFiestaSpinDto : SpinResultsDto<VegetableFiestaTileSetDto>
{
    public List<VegetableFiestaStateDto> States { get; set; } = new();
}

public record VegetableFiestaStateDto(VegetableFiestaTileSetDto[][] Grid, float[] Multipliers);