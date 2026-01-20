using Microsoft.EntityFrameworkCore;
using TechStore.Data.Models;
using TechStore.Data.Models.Enums;
using TechStore.Data.Repository.Interfaces;
using TechStore.Data.UnitOfWork;
using TechStore.Services.Core.Interfaces;
using TechStore.Web.ViewModels.Order;
using TechStore.Web.ViewModels.Payment;

namespace TechStore.Services.Core
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository orderRepository;
        private readonly IUserRepository userRepository;
        private readonly ICartRepository cartRepository;
        private readonly IProductRepository productRepository;
        private readonly IUnitOfWork unitOfWork;

        public OrderService(IOrderRepository orderRepository, 
            IUserRepository userRepository,
            ICartRepository cartRepository,
            IProductRepository productRepository,
            IUnitOfWork unitOfWork)
        {
            this.orderRepository = orderRepository;
            this.userRepository = userRepository;
            this.cartRepository = cartRepository;
            this.productRepository = productRepository;
            this.unitOfWork = unitOfWork;
        }

        public async Task<OrderDeliveryDetailsViewModel?> GetCheckoutDeliveryDetailsAsync(string userId)
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
                RecipientName = user.FullName ?? string.Empty,
                ShippingAddress = user.Address ?? string.Empty,
                PhoneNumber = user.PhoneNumber ?? string.Empty,
                Email = user.Email ?? string.Empty,
            };
        }
        public async Task<long?> CreateOrderAsync(string userId, CreateOrderViewModel model)
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

            Cart? cart = await cartRepository.GetCartWithProductsAsync(user.Id);

            if (cart == null || !cart.Products.Any())
            {
                return null;
            }

            foreach (CartProduct product in cart.Products)
            {
                if (product.Quantity > product.Product.QuantityInStock)
                {
                    return null;
                }
            }

            await unitOfWork.BeginTransactionAsync();

            try
            {
                Order order = new Order
                {
                    RecipientName = model.RecipientName,
                    OrderDate = DateTime.UtcNow,
                    ShippingAddress = model.ShippingAddress,
                    Status = Status.PendingPayment,
                    PaymentMethod = PaymentMethod.Unknown,
                    UserId = user.Id,
                    PhoneNumber = model.PhoneNumber,
                    Email = model.Email,
                    OrdersProducts = cart.Products.Select(cp => new OrderProduct
                    {
                        ProductId = cp.ProductId,
                        Quantity = cp.Quantity,
                        UnitPrice = cp.Product.Price
                    }).ToList()
                };

                await orderRepository.AddAsync(order);

                foreach (CartProduct cartProduct in cart.Products)
                {
                    cartProduct.Product.QuantityInStock -= cartProduct.Quantity;
                    productRepository.Update(cartProduct.Product);
                }

                cart.Products.Clear();
                cartRepository.Update(cart);

                await unitOfWork.SaveChangesAsync();
                await unitOfWork.CommitAsync();

                return order.Id;
            }

            catch (Exception)
            {
                await unitOfWork.RollbackAsync();
                return null;
            }
        }

        public async Task<PaymentSummaryViewModel?> GetPaymentSummaryAsync(string userId, long orderId)
        {
            bool isUserIdValid = Guid.TryParse(userId, out Guid currentUserId);
            if (!isUserIdValid)
            {
                return null;
            }

            Order? order = await orderRepository.GetOrderDetailsAsync(currentUserId, orderId);

            if (order == null || order.Status != Status.PendingPayment)
            {
                return null;
            }

            decimal totalAmount = order.OrdersProducts
                .Sum(op => op.Quantity * op.UnitPrice);

            return new PaymentSummaryViewModel
            {
                OrderId = order.Id,
                TotalAmount = totalAmount,
                ShippingAddress = order.ShippingAddress,
                RecipientName = order.RecipientName,
                Email = order.Email
            };
        }

        public async Task<bool> AttachStripeSessionAsync(long orderId, string sessionId)
        {
            Order? order = await orderRepository.GetByIdAsync(orderId);

            if (order == null)
            {
                return false;
            }

            if (order.Status != Status.PendingPayment)
            {
                return false;
            }

            order.StripeSessionId = sessionId;

            orderRepository.Update(order);
            await orderRepository.SaveChangesAsync();

            return true;
        }

        public async Task<PaymentSuccessViewModel?> GetPaymentSuccessAsync(string userId, long orderId)
        {
            bool isUserIdValid = Guid.TryParse(userId, out Guid currentUserId);
            if (!isUserIdValid)
            {
                return null;
            }

            var order = await orderRepository
                .GetAllAttached()
                .Where(o => o.Id == orderId && o.UserId == currentUserId)
                .Include(o => o.OrdersProducts)
                .ThenInclude(op => op.Product)
                .FirstOrDefaultAsync();

            if (order == null)
            {
                return null;
            }

            return new PaymentSuccessViewModel
            {
                OrderId = order.Id,
                TotalAmount = order.OrdersProducts.Sum(op => op.UnitPrice * op.Quantity),
                Products = order.OrdersProducts.ToList()
            };
        }

        public async Task MarkOrderAsPaidByOrderIdAsync(long orderId)
        {
            Order? order = await orderRepository.GetByIdAsync(orderId);

            if (order == null)
            {
                return;
            }
            if (order.Status != Status.PendingPayment)
            {
                return;
            }

            order.Status = Status.Processing;
            order.PaymentMethod = PaymentMethod.Card;

            orderRepository.Update(order);
            await orderRepository.SaveChangesAsync();
        }

        public async Task<List<OrderProduct>?> GetOrderProductsAsync(string userId, long orderId)
        {
            bool isUserIdValid = Guid.TryParse(userId, out Guid currentUserId);

            if (!isUserIdValid)
            {
                return null;
            }

            Order? order = await orderRepository.GetOrderDetailsAsync(currentUserId, orderId);

            if (order == null)
            {
                return null;
            }

            return order.OrdersProducts.ToList();
        }

        public async Task<bool> MarkOrderAsCashOnDeliveryAsync(string userId, long orderId)
        {
            bool isUserIdValid = Guid.TryParse(userId, out Guid currentUserId);

            if (!isUserIdValid)
            {
                return false;
            }

            Order? order = await orderRepository.GetOrderDetailsAsync(currentUserId, orderId);

            if (order == null)
            {
                return false;
            }

            if (order.Status != Status.PendingPayment)
            {   
                return false;
            }

            order.PaymentMethod = PaymentMethod.CashOnDelivery;
            order.Status = Status.Processing;

            orderRepository.Update(order);

            await orderRepository.SaveChangesAsync();
            return true;
        }
    }
}
