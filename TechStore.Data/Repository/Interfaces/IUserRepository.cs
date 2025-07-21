using TechStore.Data.Models;

namespace TechStore.Data.Repository.Interfaces
{
    public interface IUserRepository 
        : IRepository<User, Guid>, IAsyncRepository<User, Guid>
    {
    }
}
