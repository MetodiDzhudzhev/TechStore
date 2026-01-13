using Microsoft.EntityFrameworkCore.Storage;

namespace TechStore.Data.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext dbContext;
        private IDbContextTransaction? transaction;

        public UnitOfWork(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task BeginTransactionAsync()
        {
            transaction = await dbContext.Database.BeginTransactionAsync();
        }

        public async Task CommitAsync()
        {
            if (this.transaction == null)
            {
                throw new InvalidOperationException("No active transaction");
            }

            await transaction.CommitAsync();
        }

        public async Task RollbackAsync()
        {
            if (this.transaction != null)
            {
                await transaction.RollbackAsync();
            }
        }

        public async Task SaveChangesAsync()
        {
            await dbContext.SaveChangesAsync();
        }
    }
}
