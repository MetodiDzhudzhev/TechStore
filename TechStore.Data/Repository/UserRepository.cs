using Microsoft.EntityFrameworkCore;
using TechStore.Data.Models;
using TechStore.Data.Repository.Interfaces;

namespace TechStore.Data.Repository
{
    public class UserRepository : BaseRepository<User, Guid>, IUserRepository
    {
        public UserRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<int> GetCountAsync()
        {
            return await this
                .GetAllAttached()
                .AsNoTracking()
                .CountAsync();
        }
    }
}
