using SharedKernel.ValueObjects;

namespace SupplierService.Domain.Entities;

public class Supplier
{
    public Guid Id { get; private set; }
    public string Name { get; private set; }

    public Cnpj Cnpj { get; private set; }
    public Email Email { get; private set; }
    public PhoneNumber Phone { get; private set; }

    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    private Supplier() { }

    public Supplier(
        string name,
        string cnpj,
        string email,
        string phone)
    {
        Id = Guid.NewGuid();
        Name = name;
        Cnpj = Cnpj.Create(cnpj);
        Email = Email.Create(email);
        Phone = PhoneNumber.Create(phone);
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = CreatedAt;
    }

    public void UpdateDetails(string name, string cnpj, string email, string phone)
    {
        Name = name;
        Cnpj = Cnpj.Create(cnpj);
        Email = Email.Create(email);
        Phone = PhoneNumber.Create(phone);
        UpdatedAt = DateTime.UtcNow;
    }
}
