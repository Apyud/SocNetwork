namespace SocNetwork.Models.Repository
{
    public interface IRepository<T> where T :class
    {
        IEnumerable<T> GetAll();
        T Get(object id);
        void Create(T item);
        void Update(T item);
        void Delete(object id);
        Task<IEnumerable<T>> GetAllAsync();
        Task<T> GetAsync(object id);
        Task CreateAsync(T item);
        Task DeleteAsync(object id);
    }
}
