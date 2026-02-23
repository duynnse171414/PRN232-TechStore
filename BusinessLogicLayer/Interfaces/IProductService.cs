using BusinessLogicLayer.DTOs;

namespace BusinessLogicLayer.Interfaces;

public interface IProductService
{
    Task<(List<ProductDto> Items, int Total)> GetProductsAsync(ProductFilterDto filter);
    Task<ProductDto> GetByIdAsync(long id);
    Task<ProductDto> CreateAsync(CreateProductDto dto);
    Task<ProductDto> UpdateAsync(long id, UpdateProductDto dto);
    Task<bool> DeleteAsync(long id);
}
