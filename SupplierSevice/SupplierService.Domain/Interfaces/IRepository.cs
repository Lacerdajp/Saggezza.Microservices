namespace SupplierService.Domain.Interfaces;

public interface IRepository<TEntity> where TEntity : class
{
    Task AddAsync(TEntity entity);
    Task<List<TEntity>> GetAllAsync();
    Task<TEntity?> GetByIdAsync(Guid id);
    Task UpdateAsync(TEntity entity);
    Task DeleteAsync(TEntity entity);
}
