using Microsoft.EntityFrameworkCore;
using SupplierService.Domain.Interfaces;
using SupplierService.Infrastructure.Persistence;

namespace SupplierService.Infrastructure.Repositories;

public class Repository<TEntity> : IRepository<TEntity> where TEntity : class
{
    protected readonly SupplierDbContext Context;

    public Repository(SupplierDbContext context)
    {
        Context = context;
    }

    public async Task AddAsync(TEntity entity)
    {
        await Context.Set<TEntity>().AddAsync(entity);
        await Context.SaveChangesAsync();
    }

    public Task<List<TEntity>> GetAllAsync()
    {
        return Context.Set<TEntity>().ToListAsync();
    }

    public Task<TEntity?> GetByIdAsync(Guid id)
    {
        return Context.Set<TEntity>().FindAsync(id).AsTask();
    }

    public async Task UpdateAsync(TEntity entity)
    {
        Context.Set<TEntity>().Update(entity);
        await Context.SaveChangesAsync();
    }

    public async Task DeleteAsync(TEntity entity)
    {
        Context.Set<TEntity>().Remove(entity);
        await Context.SaveChangesAsync();
    }
}
