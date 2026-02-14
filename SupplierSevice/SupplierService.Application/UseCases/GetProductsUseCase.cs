using SupplierService.Domain.Entities;
using SupplierService.Domain.Interfaces;

namespace SupplierService.Application.UseCases;

public class GetProductsUseCase
{
    private readonly IProductRepository _repository;

    public GetProductsUseCase(IProductRepository repository)
    {
        _repository = repository;
    }

    public Task<List<Product>> ExecuteAsync()
    {
        return _repository.GetAllAsync();
    }
}
