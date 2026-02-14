using SharedKernel.ValueObjects;
using SupplierService.Application.DTOs;
using SupplierService.Domain.Interfaces;

namespace SupplierService.Application.UseCases;

public class UpdateSupplierUseCase
{
    private readonly ISupplierRepository _repository;

    public UpdateSupplierUseCase(ISupplierRepository repository)
    {
        _repository = repository;
    }

    public async Task<bool> ExecuteAsync(Guid id, UpdateSupplierRequest request)
    {
        var supplier = await _repository.GetByIdAsync(id);
        if (supplier is null)
            return false;

        var name = string.IsNullOrWhiteSpace(request.Name) ? supplier.Name : request.Name;

        var cnpj = string.IsNullOrWhiteSpace(request.Cnpj) ? supplier.Cnpj.Value : Cnpj.Create(request.Cnpj).Value;
        var email = string.IsNullOrWhiteSpace(request.Email) ? supplier.Email.Value : request.Email;
        var phone = string.IsNullOrWhiteSpace(request.Phone) ? supplier.Phone.Value : request.Phone;

        if (supplier.Cnpj.Value != cnpj)
        {
            var existing = await _repository.GetAllAsync();
            if (existing.Any(s => s.Id != supplier.Id && s.Cnpj.Value == cnpj))
                throw new InvalidOperationException("A supplier with this CNPJ already exists.");
        }

        // Entity enforces value-object validation
        supplier.UpdateDetails(name, cnpj, email, phone);
        await _repository.UpdateAsync(supplier);
        return true;
    }
}
