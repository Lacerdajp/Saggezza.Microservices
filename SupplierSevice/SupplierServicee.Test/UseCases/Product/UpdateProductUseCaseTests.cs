using FluentAssertions;
using Moq;
using SupplierService.Application.DTOs;
using SupplierService.Application.UseCases;
using SupplierService.Domain.Interfaces;
using ProductEntity = SupplierService.Domain.Entities.Product;

namespace SupplierServicee.Test.UseCases.Products;

public class UpdateProductUseCaseTests
{
    private static UpdateProductRequest Request(
        string? name = null,
        string? description = null,
        decimal? price = null,
        string? sku = null) => new()
        {
            Name = name,
            Description = description,
            Price = price,
            Sku = sku
        };

    [Fact]
    public async Task ExecuteAsync_WhenNotFound_ShouldReturnFalse_AndNotUpdate()
    {
        var id = Guid.NewGuid();

        var repo = new Mock<IProductRepository>(MockBehavior.Strict);
        repo.Setup(r => r.GetByIdAsync(id)).ReturnsAsync((ProductEntity?)null);

        var sut = new UpdateProductUseCase(repo.Object);

        var result = await sut.ExecuteAsync(id, Request(name: "New"));

        result.Should().BeFalse();
        repo.Verify(r => r.GetByIdAsync(id), Times.Once);
        repo.Verify(r => r.UpdateAsync(It.IsAny<ProductEntity>()), Times.Never);
        repo.VerifyNoOtherCalls();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public async Task ExecuteAsync_WhenProvidedPriceNotGreaterThanZero_ShouldThrow(decimal price)
    {
        var product = new ProductEntity("P1", "D1", 10m, "SKU-1");

        var repo = new Mock<IProductRepository>(MockBehavior.Strict);
        repo.Setup(r => r.GetByIdAsync(product.Id)).ReturnsAsync(product);

        var sut = new UpdateProductUseCase(repo.Object);

        Func<Task> act = () => sut.ExecuteAsync(product.Id, Request(price: price));

        await act.Should().ThrowAsync<ArgumentException>()
            .WithMessage("Price must be greater than zero.");

        repo.Verify(r => r.GetByIdAsync(product.Id), Times.Once);
        repo.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ExecuteAsync_WhenSkuChangesToExistingOne_IgnoringCase_ShouldThrow_AndNotUpdate()
    {
        var product = new ProductEntity("P1", "D1", 10m, "SKU-1");
        var other = new ProductEntity("P2", "D2", 20m, "SKU-2");

        var repo = new Mock<IProductRepository>(MockBehavior.Strict);
        repo.Setup(r => r.GetByIdAsync(product.Id)).ReturnsAsync(product);
        repo.Setup(r => r.GetAllAsync()).ReturnsAsync([product, other]);

        var sut = new UpdateProductUseCase(repo.Object);

        Func<Task> act = () => sut.ExecuteAsync(product.Id, Request(sku: "sku-2"));

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("A product with this SKU already exists.");

        repo.Verify(r => r.GetByIdAsync(product.Id), Times.Once);
        repo.Verify(r => r.GetAllAsync(), Times.Once);
        repo.Verify(r => r.UpdateAsync(It.IsAny<ProductEntity>()), Times.Never);
        repo.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ExecuteAsync_WhenSkuNotChanged_ShouldUpdate_WithDefaults()
    {
        var product = new ProductEntity("P1", "D1", 10m, "SKU-1");

        var repo = new Mock<IProductRepository>(MockBehavior.Strict);
        repo.Setup(r => r.GetByIdAsync(product.Id)).ReturnsAsync(product);
        repo.Setup(r => r.UpdateAsync(product)).Returns(Task.CompletedTask);

        var sut = new UpdateProductUseCase(repo.Object);

        var result = await sut.ExecuteAsync(product.Id, Request());

        result.Should().BeTrue();
        product.Sku.Should().Be("SKU-1");

        repo.Verify(r => r.GetByIdAsync(product.Id), Times.Once);
        repo.Verify(r => r.UpdateAsync(product), Times.Once);
        repo.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ExecuteAsync_WhenSkuChangesToUnique_ShouldUpdate_AndCheckUniqueness()
    {
        var product = new ProductEntity("P1", "D1", 10m, "SKU-1");
        var other = new ProductEntity("P2", "D2", 20m, "SKU-2");

        var repo = new Mock<IProductRepository>(MockBehavior.Strict);
        repo.Setup(r => r.GetByIdAsync(product.Id)).ReturnsAsync(product);
        repo.Setup(r => r.GetAllAsync()).ReturnsAsync([product, other]);
        repo.Setup(r => r.UpdateAsync(product)).Returns(Task.CompletedTask);

        var sut = new UpdateProductUseCase(repo.Object);

        var result = await sut.ExecuteAsync(product.Id, Request(name: "New", sku: "SKU-NEW"));

        result.Should().BeTrue();
        product.Name.Should().Be("New");
        product.Sku.Should().Be("SKU-NEW");

        repo.Verify(r => r.GetByIdAsync(product.Id), Times.Once);
        repo.Verify(r => r.GetAllAsync(), Times.Once);
        repo.Verify(r => r.UpdateAsync(product), Times.Once);
        repo.VerifyNoOtherCalls();
    }
}
