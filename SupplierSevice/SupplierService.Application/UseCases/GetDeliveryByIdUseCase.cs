using SupplierService.Domain.Entities;
using SupplierService.Domain.Interfaces;

namespace SupplierService.Application.UseCases;

public class GetDeliveryByIdUseCase
{
    private readonly IDeliveryRepository _repository;

    public GetDeliveryByIdUseCase(IDeliveryRepository repository)
    {
        _repository = repository;
    }

    public Task<Delivery?> ExecuteAsync(Guid id)
    {
        return _repository.GetByIdAsync(id);
    }
}
