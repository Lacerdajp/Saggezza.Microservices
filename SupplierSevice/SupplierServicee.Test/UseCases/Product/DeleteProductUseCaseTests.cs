using FluentAssertions;
using Moq;
using SupplierService.Application.UseCases;
using SupplierService.Domain.Interfaces;
using ProductEntity = SupplierService.Domain.Entities.Product;

namespace SupplierServicee.Test.UseCases.Products;

public class DeleteProductUseCaseTests
{
    [Fact]
    public async Task ExecuteAsync_WhenNotFound_ShouldReturnFalse_AndNotDelete()
    {
        var id = Guid.NewGuid();

        var repo = new Mock<IProductRepository>(MockBehavior.Strict);
        repo.Setup(r => r.GetByIdAsync(id)).ReturnsAsync((ProductEntity?)null);

        var sut = new DeleteProductUseCase(repo.Object);

        var result = await sut.ExecuteAsync(id);

        result.Should().BeFalse();
        repo.Verify(r => r.GetByIdAsync(id), Times.Once);
        repo.Verify(r => r.DeleteAsync(It.IsAny<ProductEntity>()), Times.Never);
        repo.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ExecuteAsync_WhenFound_ShouldDelete_AndReturnTrue()
    {
        var product = new ProductEntity("P1", "D1", 10m, "SKU-1");

        var repo = new Mock<IProductRepository>(MockBehavior.Strict);
        repo.Setup(r => r.GetByIdAsync(product.Id)).ReturnsAsync(product);
        repo.Setup(r => r.DeleteAsync(product)).Returns(Task.CompletedTask);

        var sut = new DeleteProductUseCase(repo.Object);

        var result = await sut.ExecuteAsync(product.Id);

        result.Should().BeTrue();
        repo.Verify(r => r.GetByIdAsync(product.Id), Times.Once);
        repo.Verify(r => r.DeleteAsync(product), Times.Once);
        repo.VerifyNoOtherCalls();
    }
}
