using Microsoft.EntityFrameworkCore.Infrastructure;
using SocNetwork.Models.Db;

namespace SocNetwork.Models.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _appContext;
        private Dictionary<Type, object> _repositories;
        private bool _disposed = false;

        public UnitOfWork(ApplicationDbContext app)
        {
            this._appContext = app;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public IRepository<TEntity> GetRepository<TEntity>(bool hasCustomRepository = true) where TEntity : class
        {
            if (_repositories == null)
            {
                _repositories = new Dictionary<Type, object>();
            }

            // УПРОЩАЕМ ЛОГИКУ - убираем GetService
            var type = typeof(TEntity);

            // Для User используем UserRepository
            if (type == typeof(User) && hasCustomRepository)
            {
                if (!_repositories.ContainsKey(type))
                {
                    _repositories[type] = new UserRepository(_appContext);
                }
            }
            // Для Message используем MessageRepository
            else if (type == typeof(Message) && hasCustomRepository)
            {
                if (!_repositories.ContainsKey(type))
                {
                    _repositories[type] = new MessageRepository(_appContext);
                }
            }
            // Для остальных сущностей используем базовый Repository
            else if (!_repositories.ContainsKey(type))
            {
                _repositories[type] = new Repository<TEntity>(_appContext);
            }

            return (IRepository<TEntity>)_repositories[type];
        }

        public int SaveChanges(bool ensureAutoHistory = false)
        {
            return _appContext.SaveChanges();
        }

        public async Task<int> SaveChangesAsync(bool ensureAutoHistory = false)
        {
            return await _appContext.SaveChangesAsync();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed && disposing)
            {
                _appContext?.Dispose();
            }
            _disposed = true;
        }
    }
}

