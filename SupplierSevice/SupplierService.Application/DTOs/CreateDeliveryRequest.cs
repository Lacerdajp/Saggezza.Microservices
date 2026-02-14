namespace SupplierService.Application.DTOs;

public class CreateDeliveryRequest
{
    public Guid SupplierId { get; set; }
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }
}
