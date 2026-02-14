using FluentAssertions;
using Moq;
using SupplierService.Application.UseCases;
using SupplierService.Domain.Interfaces;
using ProductEntity = SupplierService.Domain.Entities.Product;

namespace SupplierServicee.Test.UseCases.Products;

public class GetProductsUseCaseTests
{
    [Fact]
    public async Task ExecuteAsync_ShouldReturnAllProducts_FromRepository()
    {
        var products = new List<ProductEntity>
        {
            new("P1", "D1", 10m, "SKU-1"),
            new("P2", "D2", 20m, "SKU-2")
        };

        var repo = new Mock<IProductRepository>(MockBehavior.Strict);
        repo.Setup(r => r.GetAllAsync()).ReturnsAsync(products);

        var sut = new GetProductsUseCase(repo.Object);

        var result = await sut.ExecuteAsync();

        result.Should().BeSameAs(products);
        repo.Verify(r => r.GetAllAsync(), Times.Once);
        repo.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ExecuteAsync_WhenNoProducts_ShouldReturnEmptyList()
    {
        var repo = new Mock<IProductRepository>(MockBehavior.Strict);
        repo.Setup(r => r.GetAllAsync()).ReturnsAsync([]);

        var sut = new GetProductsUseCase(repo.Object);

        var result = await sut.ExecuteAsync();

        result.Should().NotBeNull();
        result.Should().BeEmpty();
        repo.Verify(r => r.GetAllAsync(), Times.Once);
        repo.VerifyNoOtherCalls();
    }
}
