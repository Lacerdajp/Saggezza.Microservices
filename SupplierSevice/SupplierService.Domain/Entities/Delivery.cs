using SupplierService.Domain.Enums;

namespace SupplierService.Domain.Entities;

public class Delivery
{
    public Guid Id { get; private set; }
    public Guid SupplierId { get; private set; }
    public Guid ProductId { get; private set; }
    public int Quantity { get; private set; }
    public decimal TotalAmount { get; private set; }
    public DeliveryStatus Status { get; private set; }
    public DateTime CreatedAt { get; private set; }

    public Guid CreatedBy { get; private set; }

    public DateTime UpdatedAt { get; private set; }

    private Delivery() { }

    public Delivery(Guid supplierId, Guid productId, int quantity, decimal productUnitPrice, Guid createdBy)
    {
        if (quantity <= 0)
            throw new ArgumentException("Quantity must be greater than zero.");

        if (productUnitPrice <= 0)
            throw new ArgumentException("Product unit price must be greater than zero.");

        if (createdBy == Guid.Empty)
            throw new ArgumentException("CreatedBy must be a valid user id.");

        Id = Guid.NewGuid();
        SupplierId = supplierId;
        ProductId = productId;
        Quantity = quantity;
        TotalAmount = CalculateTotalAmount(quantity, productUnitPrice);
        Status = DeliveryStatus.Pending;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = CreatedAt;
        CreatedBy = createdBy;
    }

    public void UpdateDetails(int quantity, decimal productUnitPrice, DeliveryStatus status)
    {
        if (quantity <= 0)
            throw new ArgumentException("Quantity must be greater than zero.");

        if (productUnitPrice <= 0)
            throw new ArgumentException("Product unit price must be greater than zero.");

        Quantity = quantity;
        TotalAmount = CalculateTotalAmount(quantity, productUnitPrice);
        Status = status;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Complete()
    {
        Status = DeliveryStatus.Completed;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Cancel()
    {
        Status = DeliveryStatus.Cancelled;
        UpdatedAt = DateTime.UtcNow;
    }

    private static decimal CalculateTotalAmount(int quantity, decimal productUnitPrice)
        => quantity * productUnitPrice;
}
