using TechStore.Data.Models;
using TechStore.Data.Repository.Interfaces;
using TechStore.Services.Core.Interfaces;
using TechStore.Web.ViewModels.Order;

namespace TechStore.Services.Core
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository orderRepository;
        private readonly IUserRepository userRepository;

        public OrderService(IOrderRepository orderRepository, 
            IUserRepository userRepository)
        {
            this.orderRepository = orderRepository;
            this.userRepository = userRepository;
        }

        public async Task<OrderDeliveryDetailsViewModel?> GetOrderDeliveryDetailsAsync(string userId)
        {
            bool isUserIdValid = Guid.TryParse(userId, out Guid currentUserId);
            if (!isUserIdValid)
            {
                return null;
            }

            User? user = await userRepository.GetByIdAsync(currentUserId);

            if (user == null)
            {
                return null;
            }

            return new OrderDeliveryDetailsViewModel
            {
                FullName = user.FullName,
                ShippingAddress = user.Address,
                PhoneNumber = user.PhoneNumber,
                Email = user.Email,
            };
        }
    }
}
