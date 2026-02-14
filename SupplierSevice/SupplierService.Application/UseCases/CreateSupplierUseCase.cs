using SharedKernel.ValueObjects;
using SupplierService.Application.DTOs;
using SupplierService.Domain.Entities;
using SupplierService.Domain.Interfaces;

namespace SupplierService.Application.UseCases;

public class CreateSupplierUseCase
{
    private readonly ISupplierRepository _repository;

    public CreateSupplierUseCase(ISupplierRepository repository)
    {
        _repository = repository;
    }

    public async Task ExecuteAsync(CreateSupplierRequest request)
    {
        var cnpjDigits = Cnpj.Create(request.Cnpj).Value;

        var existing = await _repository.GetAllAsync();
        if (existing.Any(s => s.Cnpj.Value == cnpjDigits))
            throw new InvalidOperationException("A supplier with this CNPJ already exists.");

        var supplier = new Supplier(
            request.Name,
            cnpjDigits,
            request.Email,
            request.Phone
        );

        await _repository.AddAsync(supplier);
    }
}
