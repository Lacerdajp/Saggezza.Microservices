using SupplierService.Domain.Entities;
using SupplierService.Domain.Interfaces;

namespace SupplierService.Application.UseCases;

public class GetSuppliersUseCase
{
    private readonly ISupplierRepository _repository;

    public GetSuppliersUseCase(ISupplierRepository repository)
    {
        _repository = repository;
    }

    public Task<List<Supplier>> ExecuteAsync()
    {
        return _repository.GetAllAsync();
    }
}
