using Microsoft.EntityFrameworkCore.Infrastructure;
using SocNetwork.Models.Db;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SocNetwork.Models.Repository
{
    public class UnitOfWork : IUnitOfWork, IDisposable
    {
        private readonly ApplicationDbContext _appContext;
        private Dictionary<Type, object> _repositories;
        private bool _disposed = false;

        public UnitOfWork(ApplicationDbContext app)
        {
            _appContext = app;
        }

        public IRepository<TEntity> GetRepository<TEntity>(bool hasCustomRepository = true) where TEntity : class
        {
            if (_repositories == null)
            {
                _repositories = new Dictionary<Type, object>();
            }

            var type = typeof(TEntity);

            // Кастомные репозитории
            if (type == typeof(User) && hasCustomRepository)
            {
                if (!_repositories.ContainsKey(type))
                    _repositories[type] = new UserRepository(_appContext);
            }
            else if (type == typeof(Message) && hasCustomRepository)
            {
                if (!_repositories.ContainsKey(type))
                    _repositories[type] = new MessageRepository(_appContext);
            }
            else if (type == typeof(FriendShip))
            {
                if (!_repositories.ContainsKey(type))
                    _repositories[type] = new FriendShipRepository(_appContext);
            }
            else if (!_repositories.ContainsKey(type))
            {
                _repositories[type] = new Repository<TEntity>(_appContext);
            }

            return (IRepository<TEntity>)_repositories[type];
        }

        // ✅ Новый метод: универсальный доступ к Query<T>()
        public IQueryable<TEntity> Query<TEntity>() where TEntity : class
        {
            // Используем существующий репозиторий — и его метод Query()
            return GetRepository<TEntity>().Query();
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

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
