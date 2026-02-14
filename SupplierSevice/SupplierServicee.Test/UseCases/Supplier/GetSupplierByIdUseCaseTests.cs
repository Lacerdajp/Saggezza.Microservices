using FluentAssertions;
using Moq;
using SupplierService.Application.UseCases;
using SupplierService.Domain.Interfaces;
using SupplierEntity = SupplierService.Domain.Entities.Supplier;

namespace SupplierServicee.Test.UseCases.Suppliers;

public class GetSupplierByIdUseCaseTests
{
    [Fact]
    public async Task ExecuteAsync_ShouldReturnSupplier_FromRepository()
    {
        var supplier = new SupplierEntity("S1", "12345678000195", "s1@acme.com", "11911111111");

        var repo = new Mock<ISupplierRepository>(MockBehavior.Strict);
        repo.Setup(r => r.GetByIdAsync(supplier.Id)).ReturnsAsync(supplier);

        var sut = new GetSupplierByIdUseCase(repo.Object);

        var result = await sut.ExecuteAsync(supplier.Id);

        result.Should().BeSameAs(supplier);
        repo.Verify(r => r.GetByIdAsync(supplier.Id), Times.Once);
        repo.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ExecuteAsync_WhenNotFound_ShouldReturnNull()
    {
        var id = Guid.NewGuid();

        var repo = new Mock<ISupplierRepository>(MockBehavior.Strict);
        repo.Setup(r => r.GetByIdAsync(id)).ReturnsAsync((SupplierEntity?)null);

        var sut = new GetSupplierByIdUseCase(repo.Object);

        var result = await sut.ExecuteAsync(id);

        result.Should().BeNull();
        repo.Verify(r => r.GetByIdAsync(id), Times.Once);
        repo.VerifyNoOtherCalls();
    }
}
