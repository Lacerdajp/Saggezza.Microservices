using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SupplierService.Application.DTOs;
using SupplierService.Application.UseCases;

namespace SupplierService.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class DeliveryController : ControllerBase
{
    private readonly CreateDeliveryUseCase _createUseCase;
    private readonly GetDeliveriesUseCase _getUseCase;
    private readonly GetDeliveryByIdUseCase _getByIdUseCase;
    private readonly UpdateDeliveryUseCase _updateUseCase;
    private readonly DeleteDeliveryUseCase _deleteUseCase;

    public DeliveryController(
        CreateDeliveryUseCase createUseCase,
        GetDeliveriesUseCase getUseCase,
        GetDeliveryByIdUseCase getByIdUseCase,
        UpdateDeliveryUseCase updateUseCase,
        DeleteDeliveryUseCase deleteUseCase)
    {
        _createUseCase = createUseCase;
        _getUseCase = getUseCase;
        _getByIdUseCase = getByIdUseCase;
        _updateUseCase = updateUseCase;
        _deleteUseCase = deleteUseCase;
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateDeliveryRequest request)
    {
        var userIdValue = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
        if (!Guid.TryParse(userIdValue, out var userId) || userId == Guid.Empty)
            return Unauthorized("User id claim missing or invalid.");

        await _createUseCase.ExecuteAsync(request, userId);
        return Ok("Delivery created successfully");
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var deliveries = await _getUseCase.ExecuteAsync();
        return Ok(deliveries);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var delivery = await _getByIdUseCase.ExecuteAsync(id);
        return delivery is null ? NotFound() : Ok(delivery);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, UpdateDeliveryRequest request)
    {
        var updated = await _updateUseCase.ExecuteAsync(id, request);
        return updated ? NoContent() : NotFound();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var deleted = await _deleteUseCase.ExecuteAsync(id);
        return deleted ? NoContent() : NotFound();
    }
}
