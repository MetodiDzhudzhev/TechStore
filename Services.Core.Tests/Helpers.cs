using MockQueryable.Moq;

namespace TechStore.Services.Core.Tests
{
    public static class MockHelper
    {
        public static IQueryable<T> CreateMockQueryable<T>(List<T> data) where T : class
        {
            return data.AsQueryable().BuildMock();
        }
    }
}
