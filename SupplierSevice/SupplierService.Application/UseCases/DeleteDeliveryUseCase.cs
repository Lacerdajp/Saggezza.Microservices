using SupplierService.Domain.Interfaces;

namespace SupplierService.Application.UseCases;

public class DeleteDeliveryUseCase
{
    private readonly IDeliveryRepository _repository;

    public DeleteDeliveryUseCase(IDeliveryRepository repository)
    {
        _repository = repository;
    }

    public async Task<bool> ExecuteAsync(Guid id)
    {
        var delivery = await _repository.GetByIdAsync(id);
        if (delivery is null)
            return false;

        await _repository.DeleteAsync(delivery);
        return true;
    }
}
