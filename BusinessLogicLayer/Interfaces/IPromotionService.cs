using BusinessLogicLayer.DTOs;

namespace BusinessLogicLayer.Interfaces;

public interface IPromotionService
{
    Task<List<PromotionDto>> GetAllAsync();
    Task<List<PromotionDto>> GetActiveAsync();
    Task<PromotionDto> GetByIdAsync(long id);
    Task<PromotionDto> CreateAsync(CreatePromotionDto dto);
    Task<PromotionDto> UpdateAsync(long id, CreatePromotionDto dto);
    Task<bool> DeleteAsync(long id);
}
