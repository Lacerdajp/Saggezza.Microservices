using SupplierService.Application.DTOs;
using SupplierService.Domain.Entities;
using SupplierService.Domain.Interfaces;

namespace SupplierService.Application.UseCases;

public class CreateProductUseCase
{
    private readonly IProductRepository _repository;

    public CreateProductUseCase(IProductRepository repository)
    {
        _repository = repository;
    }

    public async Task ExecuteAsync(CreateProductRequest request)
    {
        if (request.Price <= 0)
            throw new ArgumentException("Price must be greater than zero.");

        var existing = await _repository.GetAllAsync();
        if (existing.Any(p => p.Sku.Equals(request.Sku, StringComparison.OrdinalIgnoreCase)))
            throw new InvalidOperationException("A product with this SKU already exists.");

        var product = new Product(request.Name, request.Description, request.Price, request.Sku);
        await _repository.AddAsync(product);
    }
}
