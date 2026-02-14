using FluentAssertions;
using Moq;
using SupplierService.Application.UseCases;
using SupplierService.Domain.Interfaces;
using SupplierEntity = SupplierService.Domain.Entities.Supplier;

namespace SupplierServicee.Test.UseCases.Suppliers;

public class DeleteSupplierUseCaseTests
{
    [Fact]
    public async Task ExecuteAsync_WhenNotFound_ShouldReturnFalse_AndNotDelete()
    {
        var id = Guid.NewGuid();

        var repo = new Mock<ISupplierRepository>(MockBehavior.Strict);
        repo.Setup(r => r.GetByIdAsync(id)).ReturnsAsync((SupplierEntity?)null);

        var sut = new DeleteSupplierUseCase(repo.Object);

        var result = await sut.ExecuteAsync(id);

        result.Should().BeFalse();
        repo.Verify(r => r.GetByIdAsync(id), Times.Once);
        repo.Verify(r => r.DeleteAsync(It.IsAny<SupplierEntity>()), Times.Never);
        repo.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ExecuteAsync_WhenFound_ShouldDelete_AndReturnTrue()
    {
        var supplier = new SupplierEntity("S1", "12345678000195", "s1@acme.com", "11911111111");

        var repo = new Mock<ISupplierRepository>(MockBehavior.Strict);
        repo.Setup(r => r.GetByIdAsync(supplier.Id)).ReturnsAsync(supplier);
        repo.Setup(r => r.DeleteAsync(supplier)).Returns(Task.CompletedTask);

        var sut = new DeleteSupplierUseCase(repo.Object);

        var result = await sut.ExecuteAsync(supplier.Id);

        result.Should().BeTrue();
        repo.Verify(r => r.GetByIdAsync(supplier.Id), Times.Once);
        repo.Verify(r => r.DeleteAsync(supplier), Times.Once);
        repo.VerifyNoOtherCalls();
    }
}
