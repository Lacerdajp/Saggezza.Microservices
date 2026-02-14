using FluentAssertions;
using Moq;
using SupplierService.Application.DTOs;
using SupplierService.Application.UseCases;
using SupplierService.Domain.Interfaces;
using ProductEntity = SupplierService.Domain.Entities.Product;

namespace SupplierServicee.Test.UseCases.Products;

public class CreateProductUseCaseTests
{
    private static CreateProductRequest ValidRequest(string? sku = null, decimal price = 10m) => new()
    {
        Name = "Product",
        Description = "Desc",
        Price = price,
        Sku = sku ?? "SKU-001"
    };

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public async Task ExecuteAsync_WhenPriceNotGreaterThanZero_ShouldThrow_AndNotHitRepository(decimal price)
    {
        var repo = new Mock<IProductRepository>(MockBehavior.Strict);
        var sut = new CreateProductUseCase(repo.Object);

        Func<Task> act = () => sut.ExecuteAsync(ValidRequest(price: price));

        await act.Should().ThrowAsync<ArgumentException>()
            .WithMessage("Price must be greater than zero.");

        repo.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ExecuteAsync_WhenSkuAlreadyExists_IgnoringCase_ShouldThrow_AndNotAdd()
    {
        var existing = new ProductEntity("P", "D", 10m, "SKU-001");

        var repo = new Mock<IProductRepository>(MockBehavior.Strict);
        repo.Setup(r => r.GetAllAsync()).ReturnsAsync([existing]);

        var sut = new CreateProductUseCase(repo.Object);

        Func<Task> act = () => sut.ExecuteAsync(ValidRequest(sku: "sku-001"));

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("A product with this SKU already exists.");

        repo.Verify(r => r.GetAllAsync(), Times.Once);
        repo.Verify(r => r.AddAsync(It.IsAny<ProductEntity>()), Times.Never);
        repo.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ExecuteAsync_WhenValid_ShouldAddProduct()
    {
        var repo = new Mock<IProductRepository>(MockBehavior.Strict);
        repo.Setup(r => r.GetAllAsync()).ReturnsAsync([]);
        repo.Setup(r => r.AddAsync(It.IsAny<ProductEntity>())).Returns(Task.CompletedTask);

        var sut = new CreateProductUseCase(repo.Object);
        var request = ValidRequest();

        await sut.ExecuteAsync(request);

        repo.Verify(r => r.GetAllAsync(), Times.Once);
        repo.Verify(r => r.AddAsync(It.Is<ProductEntity>(p =>
            p.Name == request.Name &&
            p.Description == request.Description &&
            p.Price == request.Price &&
            p.Sku == request.Sku
        )), Times.Once);
        repo.VerifyNoOtherCalls();
    }
}
