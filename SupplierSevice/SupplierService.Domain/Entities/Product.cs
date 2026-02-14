namespace SupplierService.Domain.Entities;

public class Product
{
    public Guid Id { get; private set; }
    public string Name { get; private set; }
    public string Description { get; private set; }
    public decimal Price { get; private set; }
    public string Sku { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    private Product() { }

    public Product(string name, string description, decimal price, string sku)
    {
        Id = Guid.NewGuid();
        Name = name;
        Description = description;
        Price = price;
        Sku = sku;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = CreatedAt;
    }

    public void UpdateDetails(string name, string description, decimal price, string sku)
    {
        Name = name;
        Description = description;
        Price = price;
        Sku = sku;
        UpdatedAt = DateTime.UtcNow;
    }
}
