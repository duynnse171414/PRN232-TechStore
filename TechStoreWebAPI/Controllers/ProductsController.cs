using BusinessLogicLayer.DTOs;
using BusinessLogicLayer.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace TechStoreWebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IProductService _productService;

    public ProductsController(IProductService productService) => _productService = productService;

    /// <summary>F01, F03, F04 – Danh sách, tìm kiếm, lọc sản phẩm (có phân trang)</summary>
    [HttpGet]
    public async Task<IActionResult> GetProducts([FromQuery] ProductFilterDto filter)
    {
        var (items, total) = await _productService.GetProductsAsync(filter);
        return Ok(new { data = items, total, page = filter.Page, pageSize = filter.PageSize });
    }

    /// <summary>F02 – Chi tiết sản phẩm</summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(long id)
    {
        var product = await _productService.GetByIdAsync(id);
        return product == null ? NotFound() : Ok(product);
    }

    /// <summary>F05 – Thêm sản phẩm (Admin/Staff)</summary>
    [HttpPost]
    [Authorize(Roles = "admin,staff")]
    public async Task<IActionResult> Create([FromBody] CreateProductDto dto)
    {
        var product = await _productService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = product.Id }, product);
    }

    /// <summary>F05 – Cập nhật sản phẩm (Admin/Staff)</summary>
    [HttpPut("{id}")]
    [Authorize(Roles = "admin,staff")]
    public async Task<IActionResult> Update(long id, [FromBody] UpdateProductDto dto)
    {
        var product = await _productService.UpdateAsync(id, dto);
        return product == null ? NotFound() : Ok(product);
    }

    /// <summary>F05 – Xóa sản phẩm (Admin)</summary>
    [HttpDelete("{id}")]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> Delete(long id)
    {
        var deleted = await _productService.DeleteAsync(id);
        return deleted ? NoContent() : NotFound();
    }
}
