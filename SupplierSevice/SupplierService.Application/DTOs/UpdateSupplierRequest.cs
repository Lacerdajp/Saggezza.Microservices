namespace SupplierService.Application.DTOs;

public class UpdateSupplierRequest
{
    public string? Name { get; set; }
    public string? Cnpj { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
}
