using Microsoft.EntityFrameworkCore;
using SupplierService.Domain.Entities;
using SupplierService.Domain.Interfaces;
using SupplierService.Infrastructure.Persistence;

namespace SupplierService.Infrastructure.Repositories;

public class SupplierRepository : Repository<Supplier>, ISupplierRepository
{
    public SupplierRepository(SupplierDbContext context) : base(context)
    {
    }
}
