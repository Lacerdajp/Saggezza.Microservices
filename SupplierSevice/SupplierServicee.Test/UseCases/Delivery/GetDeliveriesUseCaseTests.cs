using FluentAssertions;
using Moq;
using SupplierService.Application.UseCases;
using SupplierService.Domain.Interfaces;
using DeliveryEntity = SupplierService.Domain.Entities.Delivery;

namespace SupplierServicee.Test.UseCases.Deliveries;

public class GetDeliveriesUseCaseTests
{
    [Fact]
    public async Task ExecuteAsync_ShouldReturnAllDeliveries_FromRepository()
    {
        var deliveries = new List<DeliveryEntity>
        {
            new(Guid.NewGuid(), Guid.NewGuid(), 1, 10m, Guid.NewGuid()),
            new(Guid.NewGuid(), Guid.NewGuid(), 2, 20m, Guid.NewGuid()),
        };

        var repo = new Mock<IDeliveryRepository>(MockBehavior.Strict);
        repo.Setup(r => r.GetAllAsync()).ReturnsAsync(deliveries);

        var sut = new GetDeliveriesUseCase(repo.Object);

        var result = await sut.ExecuteAsync();

        result.Should().BeSameAs(deliveries);
        repo.Verify(r => r.GetAllAsync(), Times.Once);
        repo.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ExecuteAsync_WhenNoDeliveries_ShouldReturnEmptyList()
    {
        var repo = new Mock<IDeliveryRepository>(MockBehavior.Strict);
        repo.Setup(r => r.GetAllAsync()).ReturnsAsync([]);

        var sut = new GetDeliveriesUseCase(repo.Object);

        var result = await sut.ExecuteAsync();

        result.Should().NotBeNull();
        result.Should().BeEmpty();
        repo.Verify(r => r.GetAllAsync(), Times.Once);
        repo.VerifyNoOtherCalls();
    }
}
