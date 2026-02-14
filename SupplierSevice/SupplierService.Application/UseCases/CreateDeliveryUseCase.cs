using SupplierService.Application.DTOs;
using SupplierService.Domain.Entities;
using SupplierService.Domain.Interfaces;

namespace SupplierService.Application.UseCases;

public class CreateDeliveryUseCase
{
    private readonly IDeliveryRepository _deliveryRepository;
    private readonly IProductRepository _productRepository;

    public CreateDeliveryUseCase(IDeliveryRepository deliveryRepository, IProductRepository productRepository)
    {
        _deliveryRepository = deliveryRepository;
        _productRepository = productRepository;
    }

    public async Task ExecuteAsync(CreateDeliveryRequest request, Guid createdBy)
    {
        if (request.Quantity <= 0)
            throw new ArgumentException("Quantity must be greater than zero.");

        if (createdBy == Guid.Empty)
            throw new ArgumentException("CreatedBy must be a valid user id.");

        var product = await _productRepository.GetByIdAsync(request.ProductId);
        if (product is null)
            throw new InvalidOperationException("Product not found.");

        var delivery = new Delivery(request.SupplierId, request.ProductId, request.Quantity, product.Price, createdBy);
        await _deliveryRepository.AddAsync(delivery);
    }
}
