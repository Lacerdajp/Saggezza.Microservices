using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SupplierService.Application.DTOs;
using SupplierService.Application.UseCases;

namespace SupplierService.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ProductController : ControllerBase
{
    private readonly CreateProductUseCase _createUseCase;
    private readonly GetProductsUseCase _getAllUseCase;
    private readonly GetProductByIdUseCase _getByIdUseCase;
    private readonly UpdateProductUseCase _updateUseCase;
    private readonly DeleteProductUseCase _deleteUseCase;

    public ProductController(
        CreateProductUseCase createUseCase,
        GetProductsUseCase getAllUseCase,
        GetProductByIdUseCase getByIdUseCase,
        UpdateProductUseCase updateUseCase,
        DeleteProductUseCase deleteUseCase)
    {
        _createUseCase = createUseCase;
        _getAllUseCase = getAllUseCase;
        _getByIdUseCase = getByIdUseCase;
        _updateUseCase = updateUseCase;
        _deleteUseCase = deleteUseCase;
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create(CreateProductRequest request)
    {
        await _createUseCase.ExecuteAsync(request);
        return Ok("Product created successfully");
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var products = await _getAllUseCase.ExecuteAsync();
        return Ok(products);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var product = await _getByIdUseCase.ExecuteAsync(id);
        return product is null ? NotFound() : Ok(product);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(Guid id, UpdateProductRequest request)
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
