using SupplierService.Domain.Enums;

namespace SupplierService.Application.DTOs;

public class UpdateDeliveryRequest
{
    public int? Quantity { get; set; }
    public DeliveryStatus? Status { get; set; }
}
