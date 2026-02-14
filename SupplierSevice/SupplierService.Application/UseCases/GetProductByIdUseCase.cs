using SupplierService.Domain.Entities;
using SupplierService.Domain.Interfaces;

namespace SupplierService.Application.UseCases;

public class GetProductByIdUseCase
{
    private readonly IProductRepository _repository;

    public GetProductByIdUseCase(IProductRepository repository)
    {
        _repository = repository;
    }

    public Task<Product?> ExecuteAsync(Guid id)
    {
        return _repository.GetByIdAsync(id);
    }
}
