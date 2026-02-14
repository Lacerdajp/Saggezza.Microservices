using SupplierService.Domain.Interfaces;

namespace SupplierService.Application.UseCases;

public class DeleteSupplierUseCase
{
    private readonly ISupplierRepository _repository;

    public DeleteSupplierUseCase(ISupplierRepository repository)
    {
        _repository = repository;
    }

    public async Task<bool> ExecuteAsync(Guid id)
    {
        var supplier = await _repository.GetByIdAsync(id);
        if (supplier is null)
            return false;

        await _repository.DeleteAsync(supplier);
        return true;
    }
}
