using Microsoft.EntityFrameworkCore;
using SupplierService.Domain.Entities;
using SupplierService.Domain.Interfaces;
using SupplierService.Infrastructure.Persistence;

namespace SupplierService.Infrastructure.Repositories;

public class DeliveryRepository : Repository<Delivery>, IDeliveryRepository
{
    public DeliveryRepository(SupplierDbContext context) : base(context)
    {
    }
}
