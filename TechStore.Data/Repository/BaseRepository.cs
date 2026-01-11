using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Reflection;
using TechStore.Data.Repository.Interfaces;
using static TechStore.Data.Common.ExceptionMessages;


namespace TechStore.Data.Repository
{
    public abstract class BaseRepository<TEntity, TKey>
        : IRepository<TEntity, TKey>, IAsyncRepository<TEntity, TKey>
        where TEntity : class
    {
        protected readonly ApplicationDbContext DbContext;
        protected readonly DbSet<TEntity> DbSet;

        protected BaseRepository(ApplicationDbContext dbContext)
        {
            this.DbContext = dbContext;
            this.DbSet = this.DbContext.Set<TEntity>();
        }

        public void Add(TEntity item)
        {
            this.DbSet.Add(item);
        }

        public async Task AddAsync(TEntity item)
        {
            await this.DbSet.AddAsync(item);
        }

        public void AddRange(IEnumerable<TEntity> items)
        {
            this.DbSet.AddRange(items);
        }

        public async Task AddRangeAsync(IEnumerable<TEntity> items)
        {
            await this.DbSet.AddRangeAsync(items);
        }

        public int Count()
        {
            return this.DbSet
                .Count();
        }

        public Task<int> CountAsync()
        {
            return this.DbSet
                .CountAsync();
        }

        public void Delete(TEntity entity)
        {
            this.PerformSoftDeleteOfEntity(entity);
            this.Update(entity);
        }

        public TEntity? FirstOrDefault(Func<TEntity, bool> predicate)
        {
            return this.DbSet
                .FirstOrDefault(predicate);
        }

        public Task<TEntity?> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return this.DbSet
                .FirstOrDefaultAsync(predicate);
        }

        public IEnumerable<TEntity> GetAll()
        {
            return this.DbSet
                .ToArray();
        }

        public async Task<IEnumerable<TEntity>> GetAllAsync()
        {
            TEntity[] entities = await this.DbSet
                .ToArrayAsync();

            return entities;
        }

        public IQueryable<TEntity> GetAllAttached()
        {
            return this.DbSet
                .AsQueryable();
        }

        public TEntity? GetById(TKey id)
        {
            return this.DbSet.Find(id);
        }

        public ValueTask<TEntity?> GetByIdAsync(TKey id)
        {
            return this.DbSet.FindAsync(id);
        }

        public void HardDelete(TEntity entity)
        {
            this.DbSet.Remove(entity);
        }
        public void SaveChanges()
        {
            this.DbContext.SaveChanges();
        }

        public async Task SaveChangesAsync()
        {
            await this.DbContext.SaveChangesAsync();
        }

        public TEntity? SingleOrDefault(Func<TEntity, bool> predicate)
        {
            return this.DbSet
                .SingleOrDefault(predicate);
        }

        public Task<TEntity?> SingleOrDefaultAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return this.DbSet
                .SingleOrDefaultAsync(predicate);
        }

        public void Update(TEntity item)
        {
            var entry = DbContext.Entry(item);

            if (entry.State == EntityState.Detached)
            {
                DbSet.Attach(item);
            }

            entry.State = EntityState.Modified;
        }

        private void PerformSoftDeleteOfEntity(TEntity entity)
        {
            PropertyInfo? isDeletedProperty =
                this.GetIsDeletedProperty(entity);
            if (isDeletedProperty == null)
            {
                throw new InvalidOperationException(SoftDeleteOnNonSoftDeletableEntity);
            }

            isDeletedProperty.SetValue(entity, true);
        }

        private PropertyInfo? GetIsDeletedProperty(TEntity entity)
        {
            return typeof(TEntity)
                .GetProperties()
                .FirstOrDefault(pi => pi.PropertyType == typeof(bool) &&
                                                 pi.Name == "IsDeleted");
        }
    }
}
