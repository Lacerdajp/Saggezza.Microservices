using FluentAssertions;
using Moq;
using SupplierService.Application.DTOs;
using SupplierService.Application.UseCases;
using SupplierService.Domain.Entities;
using SupplierService.Domain.Interfaces;

namespace SupplierServicee.Test.UseCases.Suppliers;

public class UpdateSupplierUseCaseTests
{
    private static UpdateSupplierRequest Request(
        string? name = null,
        string? cnpj = null,
        string? email = null,
        string? phone = null) => new()
        {
            Name = name,
            Cnpj = cnpj,
            Email = email,
            Phone = phone
        };

    [Fact]
    public async Task ExecuteAsync_WhenSupplierNotFound_ShouldReturnFalse_AndNotUpdate()
    {
        var id = Guid.NewGuid();

        var repo = new Mock<ISupplierRepository>(MockBehavior.Strict);
        repo.Setup(r => r.GetByIdAsync(id)).ReturnsAsync((Supplier?)null);

        var sut = new UpdateSupplierUseCase(repo.Object);

        var result = await sut.ExecuteAsync(id, Request(name: "New"));

        result.Should().BeFalse();
        repo.Verify(r => r.GetByIdAsync(id), Times.Once);
        repo.Verify(r => r.UpdateAsync(It.IsAny<Supplier>()), Times.Never);
        repo.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ExecuteAsync_WhenNoFieldsProvided_ShouldKeepExistingValues_AndUpdate()
    {
        var supplier = new Supplier("S1", "12345678000195", "s1@acme.com", "11911111111");

        var repo = new Mock<ISupplierRepository>(MockBehavior.Strict);
        repo.Setup(r => r.GetByIdAsync(supplier.Id)).ReturnsAsync(supplier);
        repo.Setup(r => r.UpdateAsync(supplier)).Returns(Task.CompletedTask);

        var sut = new UpdateSupplierUseCase(repo.Object);

        var result = await sut.ExecuteAsync(supplier.Id, Request());

        result.Should().BeTrue();
        supplier.Name.Should().Be("S1");
        supplier.Cnpj.Value.Should().Be("12345678000195");
        supplier.Email.Value.Should().Be("s1@acme.com");
        supplier.Phone.Value.Should().Be("11911111111");

        repo.Verify(r => r.GetByIdAsync(supplier.Id), Times.Once);
        repo.Verify(r => r.UpdateAsync(supplier), Times.Once);
        repo.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ExecuteAsync_WhenChangingCnpj_ToExistingOne_ShouldThrow_AndNotUpdate()
    {
        var supplier = new Supplier("S1", "12345678000195", "s1@acme.com", "11911111111");
        var other = new Supplier("S2", "98765432000198", "s2@acme.com", "11922222222");

        var repo = new Mock<ISupplierRepository>(MockBehavior.Strict);
        repo.Setup(r => r.GetByIdAsync(supplier.Id)).ReturnsAsync(supplier);
        repo.Setup(r => r.GetAllAsync()).ReturnsAsync([supplier, other]);

        var sut = new UpdateSupplierUseCase(repo.Object);

        Func<Task> act = () => sut.ExecuteAsync(supplier.Id, Request(cnpj: other.Cnpj.Value));

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("A supplier with this CNPJ already exists.");

        repo.Verify(r => r.GetByIdAsync(supplier.Id), Times.Once);
        repo.Verify(r => r.GetAllAsync(), Times.Once);
        repo.Verify(r => r.UpdateAsync(It.IsAny<Supplier>()), Times.Never);
        repo.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ExecuteAsync_WhenChangingCnpj_ToUniqueOne_ShouldUpdate()
    {
        var supplier = new Supplier("S1", "12345678000195", "s1@acme.com", "11911111111");
        var other = new Supplier("S2", "98765432000198", "s2@acme.com", "11922222222");

        var newCnpj = "11222333000181";

        var repo = new Mock<ISupplierRepository>(MockBehavior.Strict);
        repo.Setup(r => r.GetByIdAsync(supplier.Id)).ReturnsAsync(supplier);
        repo.Setup(r => r.GetAllAsync()).ReturnsAsync([supplier, other]);
        repo.Setup(r => r.UpdateAsync(supplier)).Returns(Task.CompletedTask);

        var sut = new UpdateSupplierUseCase(repo.Object);

        var result = await sut.ExecuteAsync(supplier.Id, Request(name: "NewName", cnpj: newCnpj));

        result.Should().BeTrue();
        supplier.Name.Should().Be("NewName");
        supplier.Cnpj.Value.Should().Be(newCnpj);

        repo.Verify(r => r.GetByIdAsync(supplier.Id), Times.Once);
        repo.Verify(r => r.GetAllAsync(), Times.Once);
        repo.Verify(r => r.UpdateAsync(supplier), Times.Once);
        repo.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ExecuteAsync_WhenCnpjInvalid_ShouldThrow()
    {
        var supplier = new Supplier("S1", "12345678000195", "s1@acme.com", "11911111111");

        var repo = new Mock<ISupplierRepository>(MockBehavior.Strict);
        repo.Setup(r => r.GetByIdAsync(supplier.Id)).ReturnsAsync(supplier);

        var sut = new UpdateSupplierUseCase(repo.Object);

        Func<Task> act = () => sut.ExecuteAsync(supplier.Id, Request(cnpj: "123"));

        await act.Should().ThrowAsync<Exception>();

        repo.Verify(r => r.GetByIdAsync(supplier.Id), Times.Once);
        repo.VerifyNoOtherCalls();
    }
}
