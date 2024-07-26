namespace JobApplicationPortal.Models.Interfaces
{
    public interface IRepository<TEntity>
    {
        Task<TEntity> GetByIdAsync(int id);
        Task<List<TEntity>> GetAllAsync();
        Task<TEntity> AddAsync(TEntity entity);
        Task<TEntity> UpdateAsync(TEntity entity);
        Task DeleteAsync(int id);
        Task SaveChangesAsync();
    }
}
