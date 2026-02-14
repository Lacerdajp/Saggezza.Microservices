using SupplierService.Domain.Entities;
using SupplierService.Domain.Interfaces;

namespace SupplierService.Application.UseCases;

public class GetDeliveriesUseCase
{
    private readonly IDeliveryRepository _repository;

    public GetDeliveriesUseCase(IDeliveryRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<Delivery>> ExecuteAsync()
    {
        return await _repository.GetAllAsync();
    }
}
