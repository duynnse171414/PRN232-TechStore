using BusinessLogicLayer.DTOs;

namespace BusinessLogicLayer.Interfaces;

public interface IVoucherService
{
    Task<List<VoucherDto>> GetAllAsync();
    Task<List<VoucherDto>> GetActiveAsync();
    Task<VoucherDto> GetByIdAsync(long id);
    Task<VoucherDto> GetByCodeAsync(string code);
    Task<VoucherDto> CreateAsync(CreateVoucherDto dto);
    Task<VoucherDto> UpdateAsync(long id, CreateVoucherDto dto);
    Task<bool> DeleteAsync(long id);
}
