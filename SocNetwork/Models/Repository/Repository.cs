using Microsoft.EntityFrameworkCore;
using SocNetwork.Models.Db;

namespace SocNetwork.Models.Repository
{
    public class Repository<T> : IRepository<T> where T : class
    {
        protected readonly ApplicationDbContext _db;
        protected readonly DbSet<T> _dbSet;

        public Repository(ApplicationDbContext db)
        {
            _db = db;
            _dbSet = db.Set<T>();
        }

        // Синхронные методы
        public virtual IEnumerable<T> GetAll()
        {
            return _dbSet.ToList();
        }

        public virtual T Get(object id)
        {
            return _dbSet.Find(id);
        }

        public virtual void Create(T item)
        {
            _dbSet.Add(item);
        }

        public virtual void Update(T item)
        {
            _dbSet.Update(item);
        }

        public virtual void Delete(object id)
        {
            var entity = Get(id);
            if (entity != null)
                _dbSet.Remove(entity);
        }

        // Асинхронные методы
        public virtual async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public virtual async Task<T> GetAsync(object id)
        {
            return await _dbSet.FindAsync(id);
        }

        public virtual async Task CreateAsync(T item)
        {
            await _dbSet.AddAsync(item);
        }

        public virtual async Task DeleteAsync(object id)
        {
            var entity = await GetAsync(id);
            if (entity != null)
                _dbSet.Remove(entity);
        }
    }
}