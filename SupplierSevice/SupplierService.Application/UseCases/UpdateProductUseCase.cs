using SupplierService.Application.DTOs;
using SupplierService.Domain.Interfaces;

namespace SupplierService.Application.UseCases;

public class UpdateProductUseCase
{
    private readonly IProductRepository _repository;

    public UpdateProductUseCase(IProductRepository repository)
    {
        _repository = repository;
    }

    public async Task<bool> ExecuteAsync(Guid id, UpdateProductRequest request)
    {
        var product = await _repository.GetByIdAsync(id);
        if (product is null)
            return false;

        var name = string.IsNullOrWhiteSpace(request.Name) ? product.Name : request.Name;
        var description = string.IsNullOrWhiteSpace(request.Description) ? product.Description : request.Description;

        var price = request.Price ?? product.Price;
        if (price <= 0)
            throw new ArgumentException("Price must be greater than zero.");

        var sku = string.IsNullOrWhiteSpace(request.Sku) ? product.Sku : request.Sku;

        if (!sku.Equals(product.Sku, StringComparison.OrdinalIgnoreCase))
        {
            var existing = await _repository.GetAllAsync();
            if (existing.Any(p => p.Id != product.Id && p.Sku.Equals(sku, StringComparison.OrdinalIgnoreCase)))
                throw new InvalidOperationException("A product with this SKU already exists.");
        }

        product.UpdateDetails(name, description, price, sku);
        await _repository.UpdateAsync(product);
        return true;
    }
}
