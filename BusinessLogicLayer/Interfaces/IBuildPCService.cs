using BusinessLogicLayer.DTOs;

namespace BusinessLogicLayer.Interfaces;

public interface IBuildPCService
{
    Task<List<ComponentTypeDto>> GetComponentTypesWithProductsAsync();
    Task<BuildResponseDto> CreateBuildAsync(CreateBuildDto dto);
    Task<BuildResponseDto> GetBuildAsync(long buildId);
    Task<List<BuildResponseDto>> GetUserBuildsAsync(long userId);
}
