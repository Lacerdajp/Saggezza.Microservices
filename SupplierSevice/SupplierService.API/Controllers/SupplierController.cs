using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SupplierService.Application.DTOs;
using SupplierService.Application.UseCases;

namespace SupplierService.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class SupplierController : ControllerBase
{
    private readonly CreateSupplierUseCase _createUseCase;
    private readonly GetSuppliersUseCase _getAllUseCase;
    private readonly GetSupplierByIdUseCase _getByIdUseCase;
    private readonly UpdateSupplierUseCase _updateUseCase;
    private readonly DeleteSupplierUseCase _deleteUseCase;

    public SupplierController(
        CreateSupplierUseCase createUseCase,
        GetSuppliersUseCase getAllUseCase,
        GetSupplierByIdUseCase getByIdUseCase,
        UpdateSupplierUseCase updateUseCase,
        DeleteSupplierUseCase deleteUseCase)
    {
        _createUseCase = createUseCase;
        _getAllUseCase = getAllUseCase;
        _getByIdUseCase = getByIdUseCase;
        _updateUseCase = updateUseCase;
        _deleteUseCase = deleteUseCase;
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create(CreateSupplierRequest request)
    {
        await _createUseCase.ExecuteAsync(request);
        return Ok("Supplier created successfully");
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var suppliers = await _getAllUseCase.ExecuteAsync();
        return Ok(suppliers);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var supplier = await _getByIdUseCase.ExecuteAsync(id);
        return supplier is null ? NotFound() : Ok(supplier);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(Guid id, UpdateSupplierRequest request)
    {
        var updated = await _updateUseCase.ExecuteAsync(id, request);
        return updated ? NoContent() : NotFound();
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var deleted = await _deleteUseCase.ExecuteAsync(id);
        return deleted ? NoContent() : NotFound();
    }
}
