using Microsoft.EntityFrameworkCore;
using SupplierService.Domain.Entities;
using SupplierService.Domain.Interfaces;
using SupplierService.Infrastructure.Persistence;

namespace SupplierService.Infrastructure.Repositories;

public class ProductRepository : Repository<Product>, IProductRepository
{
    public ProductRepository(SupplierDbContext context) : base(context)
    {
    }
}
