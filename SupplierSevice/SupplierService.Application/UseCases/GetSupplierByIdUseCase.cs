using SupplierService.Domain.Entities;
using SupplierService.Domain.Interfaces;

namespace SupplierService.Application.UseCases;

public class GetSupplierByIdUseCase
{
    private readonly ISupplierRepository _repository;

    public GetSupplierByIdUseCase(ISupplierRepository repository)
    {
        _repository = repository;
    }

    public Task<Supplier?> ExecuteAsync(Guid id)
    {
        return _repository.GetByIdAsync(id);
    }
}
