using FluentAssertions;
using Moq;
using SupplierService.Application.DTOs;
using SupplierService.Application.UseCases;
using SupplierService.Domain.Interfaces;
using DeliveryEntity = SupplierService.Domain.Entities.Delivery;
using ProductEntity = SupplierService.Domain.Entities.Product;

namespace SupplierServicee.Test.UseCases.Deliveries;

public class CreateDeliveryUseCaseTests
{
    private static CreateDeliveryRequest ValidRequest(Guid supplierId, Guid productId, int quantity = 1) => new()
    {
        SupplierId = supplierId,
        ProductId = productId,
        Quantity = quantity
    };

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public async Task ExecuteAsync_WhenQuantityNotGreaterThanZero_ShouldThrow_AndNotHitRepositories(int quantity)
    {
        var deliveryRepo = new Mock<IDeliveryRepository>(MockBehavior.Strict);
        var productRepo = new Mock<IProductRepository>(MockBehavior.Strict);

        var sut = new CreateDeliveryUseCase(deliveryRepo.Object, productRepo.Object);

        Func<Task> act = () => sut.ExecuteAsync(
            ValidRequest(Guid.NewGuid(), Guid.NewGuid(), quantity),
            createdBy: Guid.NewGuid());

        await act.Should().ThrowAsync<ArgumentException>()
            .WithMessage("Quantity must be greater than zero.");

        deliveryRepo.VerifyNoOtherCalls();
        productRepo.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ExecuteAsync_WhenCreatedByEmpty_ShouldThrow_AndNotHitRepositories()
    {
        var deliveryRepo = new Mock<IDeliveryRepository>(MockBehavior.Strict);
        var productRepo = new Mock<IProductRepository>(MockBehavior.Strict);

        var sut = new CreateDeliveryUseCase(deliveryRepo.Object, productRepo.Object);

        Func<Task> act = () => sut.ExecuteAsync(
            ValidRequest(Guid.NewGuid(), Guid.NewGuid(), 1),
            createdBy: Guid.Empty);

        await act.Should().ThrowAsync<ArgumentException>()
            .WithMessage("CreatedBy must be a valid user id.");

        deliveryRepo.VerifyNoOtherCalls();
        productRepo.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ExecuteAsync_WhenProductNotFound_ShouldThrow_AndNotAddDelivery()
    {
        var supplierId = Guid.NewGuid();
        var productId = Guid.NewGuid();

        var deliveryRepo = new Mock<IDeliveryRepository>(MockBehavior.Strict);
        var productRepo = new Mock<IProductRepository>(MockBehavior.Strict);
        productRepo.Setup(r => r.GetByIdAsync(productId)).ReturnsAsync((ProductEntity?)null);

        var sut = new CreateDeliveryUseCase(deliveryRepo.Object, productRepo.Object);

        Func<Task> act = () => sut.ExecuteAsync(
            ValidRequest(supplierId, productId, 1),
            createdBy: Guid.NewGuid());

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Product not found.");

        productRepo.Verify(r => r.GetByIdAsync(productId), Times.Once);
        deliveryRepo.Verify(r => r.AddAsync(It.IsAny<DeliveryEntity>()), Times.Never);
        deliveryRepo.VerifyNoOtherCalls();
        productRepo.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ExecuteAsync_WhenValid_ShouldAddDelivery_WithProductPrice()
    {
        var supplierId = Guid.NewGuid();
        var product = new ProductEntity("P1", "D1", 12.5m, "SKU-1");

        var deliveryRepo = new Mock<IDeliveryRepository>(MockBehavior.Strict);
        deliveryRepo.Setup(r => r.AddAsync(It.IsAny<DeliveryEntity>())).Returns(Task.CompletedTask);

        var productRepo = new Mock<IProductRepository>(MockBehavior.Strict);
        productRepo.Setup(r => r.GetByIdAsync(product.Id)).ReturnsAsync(product);

        var sut = new CreateDeliveryUseCase(deliveryRepo.Object, productRepo.Object);

        var quantity = 3;
        var createdBy = Guid.NewGuid();

        await sut.ExecuteAsync(ValidRequest(supplierId, product.Id, quantity), createdBy);

        productRepo.Verify(r => r.GetByIdAsync(product.Id), Times.Once);
        deliveryRepo.Verify(r => r.AddAsync(It.Is<DeliveryEntity>(d =>
            d.SupplierId == supplierId &&
            d.ProductId == product.Id &&
            d.Quantity == quantity &&
            d.CreatedBy == createdBy &&
            d.TotalAmount == quantity * product.Price
        )), Times.Once);

        deliveryRepo.VerifyNoOtherCalls();
        productRepo.VerifyNoOtherCalls();
    }
}
