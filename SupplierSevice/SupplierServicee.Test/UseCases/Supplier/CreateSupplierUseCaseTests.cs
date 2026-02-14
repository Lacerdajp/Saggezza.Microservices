using FluentAssertions;
using Moq;
using SupplierService.Application.DTOs;
using SupplierService.Application.UseCases;
using SupplierService.Domain.Interfaces;
using SupplierEntity = SupplierService.Domain.Entities.Supplier;

namespace SupplierServicee.Test.UseCases.Suppliers;

public class CreateSupplierUseCaseTests
{
    private static CreateSupplierRequest ValidRequest(string? cnpj = null) => new()
    {
        Name = "ACME LTDA",
        Cnpj = cnpj ?? "12345678000195",
        Email = "contato@acme.com",
        Phone = "11999999999"
    };

    [Fact]
    public async Task ExecuteAsync_WhenCnpjIsUnique_ShouldAddSupplier()
    {
        var repo = new Mock<ISupplierRepository>(MockBehavior.Strict);
        repo.Setup(r => r.GetAllAsync()).ReturnsAsync([]);
        repo.Setup(r => r.AddAsync(It.IsAny<SupplierEntity>())).Returns(Task.CompletedTask);

        var sut = new CreateSupplierUseCase(repo.Object);
        var request = ValidRequest();

        await sut.ExecuteAsync(request);

        repo.Verify(r => r.GetAllAsync(), Times.Once);
        repo.Verify(r => r.AddAsync(It.Is<SupplierEntity>(s =>
            s.Name == request.Name &&
            s.Cnpj.Value == request.Cnpj &&
            s.Email.Value == request.Email &&
            s.Phone.Value == request.Phone
        )), Times.Once);
        repo.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ExecuteAsync_WhenCnpjAlreadyExists_ShouldThrow_AndNotAdd()
    {
        var existing = new SupplierEntity("Old", "12345678000195", "old@acme.com", "11911111111");

        var repo = new Mock<ISupplierRepository>(MockBehavior.Strict);
        repo.Setup(r => r.GetAllAsync()).ReturnsAsync([existing]);

        var sut = new CreateSupplierUseCase(repo.Object);

        Func<Task> act = () => sut.ExecuteAsync(ValidRequest(cnpj: existing.Cnpj.Value));

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("A supplier with this CNPJ already exists.");

        repo.Verify(r => r.GetAllAsync(), Times.Once);
        repo.Verify(r => r.AddAsync(It.IsAny<SupplierEntity>()), Times.Never);
        repo.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ExecuteAsync_WhenCnpjInvalid_ShouldThrow_AndNotHitRepository()
    {
        var repo = new Mock<ISupplierRepository>(MockBehavior.Strict);
        var sut = new CreateSupplierUseCase(repo.Object);

        var request = ValidRequest(cnpj: "123");

        Func<Task> act = () => sut.ExecuteAsync(request);

        await act.Should().ThrowAsync<Exception>();

        repo.VerifyNoOtherCalls();
    }
}
