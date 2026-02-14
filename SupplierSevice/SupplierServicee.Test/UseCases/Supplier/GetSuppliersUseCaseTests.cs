using FluentAssertions;
using Moq;
using SupplierService.Application.UseCases;
using SupplierService.Domain.Interfaces;
using SupplierEntity = SupplierService.Domain.Entities.Supplier;

namespace SupplierServicee.Test.UseCases.Suppliers;

public class GetSuppliersUseCaseTests
{
    [Fact]
    public async Task ExecuteAsync_ShouldReturnAllSuppliers_FromRepository()
    {
        var suppliers = new List<SupplierEntity>
        {
            new("S1", "12345678000195", "s1@acme.com", "11911111111"),
            new("S2", "98765432000198", "s2@acme.com", "11922222222")
        };

        var repo = new Mock<ISupplierRepository>(MockBehavior.Strict);
        repo.Setup(r => r.GetAllAsync()).ReturnsAsync(suppliers);

        var sut = new GetSuppliersUseCase(repo.Object);

        var result = await sut.ExecuteAsync();

        result.Should().BeSameAs(suppliers);
        repo.Verify(r => r.GetAllAsync(), Times.Once);
        repo.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ExecuteAsync_WhenNoSuppliers_ShouldReturnEmptyList()
    {
        var repo = new Mock<ISupplierRepository>(MockBehavior.Strict);
        repo.Setup(r => r.GetAllAsync()).ReturnsAsync([]);

        var sut = new GetSuppliersUseCase(repo.Object);

        var result = await sut.ExecuteAsync();

        result.Should().NotBeNull();
        result.Should().BeEmpty();

        repo.Verify(r => r.GetAllAsync(), Times.Once);
        repo.VerifyNoOtherCalls();
    }
}
