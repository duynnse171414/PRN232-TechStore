using BusinessLogicLayer.DTOs;
using BusinessLogicLayer.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace TechStoreWebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class VouchersController : ControllerBase
{
    private readonly IVoucherService _voucherService;

    public VouchersController(IVoucherService voucherService) => _voucherService = voucherService;

    [HttpGet]
    public async Task<IActionResult> GetAll() => Ok(await _voucherService.GetAllAsync());

    [HttpGet("active")]
    public async Task<IActionResult> GetActive() => Ok(await _voucherService.GetActiveAsync());

    [HttpGet("{id:long}")]
    public async Task<IActionResult> GetById(long id)
    {
        var voucher = await _voucherService.GetByIdAsync(id);
        return voucher == null ? NotFound() : Ok(voucher);
    }

    [HttpGet("code/{code}")]
    public async Task<IActionResult> GetByCode(string code)
    {
        var voucher = await _voucherService.GetByCodeAsync(code);
        return voucher == null ? NotFound() : Ok(voucher);
    }

    [HttpPost]
    [Authorize(Roles = "admin,staff")]
    public async Task<IActionResult> Create([FromBody] CreateVoucherDto dto)
    {
        try
        {
            var voucher = await _voucherService.CreateAsync(dto);
            return Ok(voucher);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut("{id:long}")]
    [Authorize(Roles = "admin,staff")]
    public async Task<IActionResult> Update(long id, [FromBody] CreateVoucherDto dto)
    {
        try
        {
            var voucher = await _voucherService.UpdateAsync(id, dto);
            return Ok(voucher);
        }
        catch (KeyNotFoundException)
        {
            return NotFound(new { message = "Voucher không tồn tại." });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpDelete("{id:long}")]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> Delete(long id)
    {
        var deleted = await _voucherService.DeleteAsync(id);
        return deleted ? NoContent() : NotFound();
    }
}
