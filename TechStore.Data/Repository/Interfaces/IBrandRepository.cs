using TechStore.Data.Models;

namespace TechStore.Data.Repository.Interfaces
{
    public interface IBrandRepository
        : IRepository<Brand, int>, IAsyncRepository<Brand, int>
    {

    }
}
