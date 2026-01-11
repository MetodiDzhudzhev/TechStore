namespace TechStore.Data.Repository.Interfaces
{
    public interface IRepository<TEntity, TKey>
    {
        TEntity? GetById(TKey id);

        TEntity? SingleOrDefault(Func<TEntity, bool> predicate);

        TEntity? FirstOrDefault(Func<TEntity, bool> predicate);

        IEnumerable<TEntity> GetAll();

        int Count();

        IQueryable<TEntity> GetAllAttached();

        void Add(TEntity item);

        void AddRange(IEnumerable<TEntity> items);

        void Delete(TEntity entity);

        void HardDelete(TEntity entity);

        void Update(TEntity item);

        void SaveChanges();
    }
}
