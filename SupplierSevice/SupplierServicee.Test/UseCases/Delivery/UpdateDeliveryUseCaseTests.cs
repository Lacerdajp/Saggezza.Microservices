using FluentAssertions;
using Moq;
using SupplierService.Application.DTOs;
using SupplierService.Application.UseCases;
using DeliveryEntity = SupplierService.Domain.Entities.Delivery;
using ProductEntity = SupplierService.Domain.Entities.Product;

namespace SupplierServicee.Test.UseCases.Deliveries;

public class UpdateDeliveryUseCaseTests
{
    private static UpdateDeliveryRequest Request(int? quantity = null, SupplierService.Domain.Enums.DeliveryStatus? status = null) => new()
    {
        Quantity = quantity,
        Status = status
    };

    [Fact]
    public async Task ExecuteAsync_WhenNotFound_ShouldReturnFalse_AndNotUpdate()
    {
        var id = Guid.NewGuid();

        var deliveryRepo = new Mock<SupplierService.Domain.Interfaces.IDeliveryRepository>(MockBehavior.Strict);
        deliveryRepo.Setup(r => r.GetByIdAsync(id)).ReturnsAsync((DeliveryEntity?)null);

        var productRepo = new Mock<SupplierService.Domain.Interfaces.IProductRepository>(MockBehavior.Strict);

        var sut = new UpdateDeliveryUseCase(deliveryRepo.Object, productRepo.Object);

        var result = await sut.ExecuteAsync(id, Request(quantity: 2));

        result.Should().BeFalse();
        deliveryRepo.Verify(r => r.GetByIdAsync(id), Times.Once);
        deliveryRepo.Verify(r => r.UpdateAsync(It.IsAny<DeliveryEntity>()), Times.Never);
        deliveryRepo.VerifyNoOtherCalls();
        productRepo.VerifyNoOtherCalls();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public async Task ExecuteAsync_WhenQuantityNotGreaterThanZero_ShouldThrow(int quantity)
    {
        var delivery = new DeliveryEntity(Guid.NewGuid(), Guid.NewGuid(), 1, 10m, Guid.NewGuid());

        var deliveryRepo = new Mock<SupplierService.Domain.Interfaces.IDeliveryRepository>(MockBehavior.Strict);
        deliveryRepo.Setup(r => r.GetByIdAsync(delivery.Id)).ReturnsAsync(delivery);

        var productRepo = new Mock<SupplierService.Domain.Interfaces.IProductRepository>(MockBehavior.Strict);

        var sut = new UpdateDeliveryUseCase(deliveryRepo.Object, productRepo.Object);

        Func<Task> act = () => sut.ExecuteAsync(delivery.Id, Request(quantity: quantity));

        await act.Should().ThrowAsync<ArgumentException>()
            .WithMessage("Quantity must be greater than zero.");

        deliveryRepo.Verify(r => r.GetByIdAsync(delivery.Id), Times.Once);
        deliveryRepo.VerifyNoOtherCalls();
        productRepo.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ExecuteAsync_WhenCompletingNonPendingDelivery_ShouldThrow()
    {
        var delivery = new DeliveryEntity(Guid.NewGuid(), Guid.NewGuid(), 1, 10m, Guid.NewGuid());
        delivery.Cancel();

        var deliveryRepo = new Mock<SupplierService.Domain.Interfaces.IDeliveryRepository>(MockBehavior.Strict);
        deliveryRepo.Setup(r => r.GetByIdAsync(delivery.Id)).ReturnsAsync(delivery);

        var productRepo = new Mock<SupplierService.Domain.Interfaces.IProductRepository>(MockBehavior.Strict);

        var sut = new UpdateDeliveryUseCase(deliveryRepo.Object, productRepo.Object);

        Func<Task> act = () => sut.ExecuteAsync(delivery.Id, Request(status: SupplierService.Domain.Enums.DeliveryStatus.Completed));

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("A delivery can only be completed when the current status is Pending.");

        deliveryRepo.Verify(r => r.GetByIdAsync(delivery.Id), Times.Once);
        deliveryRepo.VerifyNoOtherCalls();
        productRepo.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ExecuteAsync_WhenCancellingCompletedDelivery_ShouldThrow()
    {
        var delivery = new DeliveryEntity(Guid.NewGuid(), Guid.NewGuid(), 1, 10m, Guid.NewGuid());
        delivery.Complete();

        var deliveryRepo = new Mock<SupplierService.Domain.Interfaces.IDeliveryRepository>(MockBehavior.Strict);
        deliveryRepo.Setup(r => r.GetByIdAsync(delivery.Id)).ReturnsAsync(delivery);

        var productRepo = new Mock<SupplierService.Domain.Interfaces.IProductRepository>(MockBehavior.Strict);

        var sut = new UpdateDeliveryUseCase(deliveryRepo.Object, productRepo.Object);

        Func<Task> act = () => sut.ExecuteAsync(delivery.Id, Request(status: SupplierService.Domain.Enums.DeliveryStatus.Cancelled));

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Cancelling a delivery that has already been completed is not allowed.");

        deliveryRepo.Verify(r => r.GetByIdAsync(delivery.Id), Times.Once);
        deliveryRepo.VerifyNoOtherCalls();
        productRepo.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ExecuteAsync_WhenProductNotFound_ShouldThrow()
    {
        var delivery = new DeliveryEntity(Guid.NewGuid(), Guid.NewGuid(), 1, 10m, Guid.NewGuid());

        var deliveryRepo = new Mock<SupplierService.Domain.Interfaces.IDeliveryRepository>(MockBehavior.Strict);
        deliveryRepo.Setup(r => r.GetByIdAsync(delivery.Id)).ReturnsAsync(delivery);

        var productRepo = new Mock<SupplierService.Domain.Interfaces.IProductRepository>(MockBehavior.Strict);
        productRepo.Setup(r => r.GetByIdAsync(delivery.ProductId)).ReturnsAsync((ProductEntity?)null);

        var sut = new UpdateDeliveryUseCase(deliveryRepo.Object, productRepo.Object);

        Func<Task> act = () => sut.ExecuteAsync(delivery.Id, Request(quantity: 2));

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Product not found.");

        deliveryRepo.Verify(r => r.GetByIdAsync(delivery.Id), Times.Once);
        productRepo.Verify(r => r.GetByIdAsync(delivery.ProductId), Times.Once);
        deliveryRepo.VerifyNoOtherCalls();
        productRepo.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ExecuteAsync_WhenValid_ShouldUpdateDelivery_UsingLatestProductPrice()
    {
        var product = new ProductEntity("P1", "D1", 7.5m, "SKU-1");
        var delivery = new DeliveryEntity(Guid.NewGuid(), product.Id, 1, 10m, Guid.NewGuid());

        var deliveryRepo = new Mock<SupplierService.Domain.Interfaces.IDeliveryRepository>(MockBehavior.Strict);
        deliveryRepo.Setup(r => r.GetByIdAsync(delivery.Id)).ReturnsAsync(delivery);
        deliveryRepo.Setup(r => r.UpdateAsync(delivery)).Returns(Task.CompletedTask);

        var productRepo = new Mock<SupplierService.Domain.Interfaces.IProductRepository>(MockBehavior.Strict);
        productRepo.Setup(r => r.GetByIdAsync(delivery.ProductId)).ReturnsAsync(product);

        var sut = new UpdateDeliveryUseCase(deliveryRepo.Object, productRepo.Object);

        var newQuantity = 3;
        var newStatus = SupplierService.Domain.Enums.DeliveryStatus.Completed;

        var result = await sut.ExecuteAsync(delivery.Id, Request(quantity: newQuantity, status: newStatus));

        result.Should().BeTrue();
        delivery.Quantity.Should().Be(newQuantity);
        delivery.Status.Should().Be(newStatus);
        delivery.TotalAmount.Should().Be(newQuantity * product.Price);

        deliveryRepo.Verify(r => r.GetByIdAsync(delivery.Id), Times.Once);
        productRepo.Verify(r => r.GetByIdAsync(delivery.ProductId), Times.Once);
        deliveryRepo.Verify(r => r.UpdateAsync(delivery), Times.Once);
        deliveryRepo.VerifyNoOtherCalls();
        productRepo.VerifyNoOtherCalls();
    }
}
