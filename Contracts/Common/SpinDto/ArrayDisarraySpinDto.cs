using Contracts.Common.TileSetsDto;

namespace Contracts.Common.SpinDto;

public class ArrayDisarraySpinDto : SpinResultsDto<ArrayDisarrayTileSetDto>
{
    public List<ArrayDisarrayStateDto> States { get; set; } = new();
}

public record ArrayDisarrayStateDto(ArrayDisarrayTileSetDto[][] Grid, float[] Multipliers);