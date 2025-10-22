namespace SocNetwork.Models.Repository
{
    public interface IUnitOfWork : IDisposable
    {
        Task <int> SaveChangesAsync(bool ensureAutoHistory = false);
        IRepository<TEntity> GetRepository<TEntity>(bool hasCustomRepository = true) where TEntity : class;
        IQueryable<T> Query<T>() where T : class;
    }
}
