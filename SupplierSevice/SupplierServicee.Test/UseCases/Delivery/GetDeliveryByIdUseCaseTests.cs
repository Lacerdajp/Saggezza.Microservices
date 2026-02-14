using FluentAssertions;
using Moq;
using SupplierService.Application.UseCases;
using SupplierService.Domain.Interfaces;
using DeliveryEntity = SupplierService.Domain.Entities.Delivery;

namespace SupplierServicee.Test.UseCases.Deliveries;

public class GetDeliveryByIdUseCaseTests
{
    [Fact]
    public async Task ExecuteAsync_ShouldReturnDelivery_FromRepository()
    {
        var delivery = new DeliveryEntity(Guid.NewGuid(), Guid.NewGuid(), 1, 10m, Guid.NewGuid());

        var repo = new Mock<IDeliveryRepository>(MockBehavior.Strict);
        repo.Setup(r => r.GetByIdAsync(delivery.Id)).ReturnsAsync(delivery);

        var sut = new GetDeliveryByIdUseCase(repo.Object);

        var result = await sut.ExecuteAsync(delivery.Id);

        result.Should().BeSameAs(delivery);
        repo.Verify(r => r.GetByIdAsync(delivery.Id), Times.Once);
        repo.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ExecuteAsync_WhenNotFound_ShouldReturnNull()
    {
        var id = Guid.NewGuid();

        var repo = new Mock<IDeliveryRepository>(MockBehavior.Strict);
        repo.Setup(r => r.GetByIdAsync(id)).ReturnsAsync((DeliveryEntity?)null);

        var sut = new GetDeliveryByIdUseCase(repo.Object);

        var result = await sut.ExecuteAsync(id);

        result.Should().BeNull();
        repo.Verify(r => r.GetByIdAsync(id), Times.Once);
        repo.VerifyNoOtherCalls();
    }
}
