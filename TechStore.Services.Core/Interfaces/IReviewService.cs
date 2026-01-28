using TechStore.Web.ViewModels.Review;

namespace TechStore.Services.Core.Interfaces
{
    public interface IReviewService
    {
        Task<bool> Add(string userId, ReviewFormInputModel inputModel);
        Task<ReviewsPanelViewModel> GetPanelAsync(Guid productId, Guid? userId, int page, int pageSize);
        Task<ReviewsStatsViewModel> GetStatsAsync(Guid productId);
    }
}
