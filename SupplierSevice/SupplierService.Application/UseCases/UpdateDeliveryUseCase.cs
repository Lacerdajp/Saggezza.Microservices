using SupplierService.Application.DTOs;
using SupplierService.Domain.Enums;
using SupplierService.Domain.Interfaces;

namespace SupplierService.Application.UseCases;

public class UpdateDeliveryUseCase
{
    private readonly IDeliveryRepository _deliveryRepository;
    private readonly IProductRepository _productRepository;

    public UpdateDeliveryUseCase(IDeliveryRepository deliveryRepository, IProductRepository productRepository)
    {
        _deliveryRepository = deliveryRepository;
        _productRepository = productRepository;
    }

    public async Task<bool> ExecuteAsync(Guid id, UpdateDeliveryRequest request)
    {
        var delivery = await _deliveryRepository.GetByIdAsync(id);
        if (delivery is null)
            return false;

        int quantity = request.Quantity ?? delivery.Quantity;
        if (quantity <= 0)
            throw new ArgumentException("Quantity must be greater than zero.");

        DeliveryStatus status = request.Status ?? delivery.Status;

        if (status == DeliveryStatus.Completed && delivery.Status != DeliveryStatus.Pending)
            throw new InvalidOperationException("A delivery can only be completed when the current status is Pending.");

        if (status == DeliveryStatus.Cancelled && delivery.Status == DeliveryStatus.Completed)
            throw new InvalidOperationException("Cancelling a delivery that has already been completed is not allowed.");

        var product = await _productRepository.GetByIdAsync(delivery.ProductId);
        if (product is null)
            throw new InvalidOperationException("Product not found.");

        delivery.UpdateDetails(quantity, product.Price, status);
        await _deliveryRepository.UpdateAsync(delivery);
        return true;
    }
}
