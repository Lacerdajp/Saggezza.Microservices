using FluentAssertions;
using Moq;
using SupplierService.Application.UseCases;
using SupplierService.Domain.Interfaces;
using ProductEntity = SupplierService.Domain.Entities.Product;

namespace SupplierServicee.Test.UseCases.Products;

public class GetProductByIdUseCaseTests
{
    [Fact]
    public async Task ExecuteAsync_ShouldReturnProduct_FromRepository()
    {
        var product = new ProductEntity("P1", "D1", 10m, "SKU-1");

        var repo = new Mock<IProductRepository>(MockBehavior.Strict);
        repo.Setup(r => r.GetByIdAsync(product.Id)).ReturnsAsync(product);

        var sut = new GetProductByIdUseCase(repo.Object);

        var result = await sut.ExecuteAsync(product.Id);

        result.Should().BeSameAs(product);
        repo.Verify(r => r.GetByIdAsync(product.Id), Times.Once);
        repo.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ExecuteAsync_WhenNotFound_ShouldReturnNull()
    {
        var id = Guid.NewGuid();

        var repo = new Mock<IProductRepository>(MockBehavior.Strict);
        repo.Setup(r => r.GetByIdAsync(id)).ReturnsAsync((ProductEntity?)null);

        var sut = new GetProductByIdUseCase(repo.Object);

        var result = await sut.ExecuteAsync(id);

        result.Should().BeNull();
        repo.Verify(r => r.GetByIdAsync(id), Times.Once);
        repo.VerifyNoOtherCalls();
    }
}
