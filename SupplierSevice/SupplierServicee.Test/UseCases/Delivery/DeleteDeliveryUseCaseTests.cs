using FluentAssertions;
using Moq;
using SupplierService.Application.UseCases;
using SupplierService.Domain.Interfaces;
using DeliveryEntity = SupplierService.Domain.Entities.Delivery;

namespace SupplierServicee.Test.UseCases.Deliveries;

public class DeleteDeliveryUseCaseTests
{
    [Fact]
    public async Task ExecuteAsync_WhenNotFound_ShouldReturnFalse_AndNotDelete()
    {
        var id = Guid.NewGuid();

        var repo = new Mock<IDeliveryRepository>(MockBehavior.Strict);
        repo.Setup(r => r.GetByIdAsync(id)).ReturnsAsync((DeliveryEntity?)null);

        var sut = new DeleteDeliveryUseCase(repo.Object);

        var result = await sut.ExecuteAsync(id);

        result.Should().BeFalse();
        repo.Verify(r => r.GetByIdAsync(id), Times.Once);
        repo.Verify(r => r.DeleteAsync(It.IsAny<DeliveryEntity>()), Times.Never);
        repo.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ExecuteAsync_WhenFound_ShouldDelete_AndReturnTrue()
    {
        var delivery = new DeliveryEntity(Guid.NewGuid(), Guid.NewGuid(), 1, 10m, Guid.NewGuid());

        var repo = new Mock<IDeliveryRepository>(MockBehavior.Strict);
        repo.Setup(r => r.GetByIdAsync(delivery.Id)).ReturnsAsync(delivery);
        repo.Setup(r => r.DeleteAsync(delivery)).Returns(Task.CompletedTask);

        var sut = new DeleteDeliveryUseCase(repo.Object);

        var result = await sut.ExecuteAsync(delivery.Id);

        result.Should().BeTrue();
        repo.Verify(r => r.GetByIdAsync(delivery.Id), Times.Once);
        repo.Verify(r => r.DeleteAsync(delivery), Times.Once);
        repo.VerifyNoOtherCalls();
    }
}
