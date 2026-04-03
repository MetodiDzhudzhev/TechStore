using MockQueryable.Moq;
using Moq;
using NUnit.Framework;
using TechStore.Data.Models;
using TechStore.Data.Models.Enums;
using TechStore.Data.Repository.Interfaces;
using TechStore.Data.UnitOfWork;
using TechStore.Web.ViewModels.Order;

namespace TechStore.Services.Core.Tests
{
    [TestFixture]
    public class OrderServiceTests
    {
        private Mock<IOrderRepository> mockOrderRepository = null!;
        private Mock<IUserRepository> mockUserRepository = null!;
        private Mock<ICartRepository> mockCartRepository = null!;
        private Mock<IProductRepository> mockProductRepository = null!;
        private Mock<IUnitOfWork> mockUnitOfWork = null!;
        private OrderService orderService = null!;

        [SetUp]
        public void SetUp()
        {
            mockOrderRepository = new Mock<IOrderRepository>();
            mockUserRepository = new Mock<IUserRepository>();
            mockCartRepository = new Mock<ICartRepository>();
            mockProductRepository = new Mock<IProductRepository>();
            mockUnitOfWork = new Mock<IUnitOfWork>();

            orderService = new OrderService(
                mockOrderRepository.Object,
                mockUserRepository.Object,
                mockCartRepository.Object,
                mockProductRepository.Object,
                mockUnitOfWork.Object);
        }

        [Test]
        public void PassAlways()
        {
            // Test that will always pass to show that the SetUp is working
            Assert.Pass();
        }

        [Test]
        public async Task GetCheckoutDeliveryDetailsAsync_ShouldReturnNull_WhenUserDoesNotExist()
        {
            var userId = Guid.NewGuid();

            SetupUser(userId, null);

            var result = await orderService.GetCheckoutDeliveryDetailsAsync(userId);

            Assert.That(result, Is.Null);
        }

        [Test]
        public async Task GetCheckoutDeliveryDetailsAsync_ShouldReturnCorrectModel_WhenUserExists()
        {
            var userId = Guid.NewGuid();

            var user = CreateUser(userId);

            SetupUser(userId, user);

            var result = await orderService.GetCheckoutDeliveryDetailsAsync(userId);

            Assert.That(result, Is.Not.Null);

            Assert.That(result!.RecipientName, Is.EqualTo(user.FullName));
            Assert.That(result.ShippingAddress, Is.EqualTo(user.Address));
            Assert.That(result.PhoneNumber, Is.EqualTo(user.PhoneNumber));
            Assert.That(result.Email, Is.EqualTo(user.Email));
        }

        [Test]
        public async Task GetCheckoutDeliveryDetailsAsync_ShouldReturnEmptyStrings_WhenUserFieldsAreNull()
        {
            var userId = Guid.NewGuid();

            var user = new User
            {
                Id = userId,
                FullName = null,
                Address = null,
                PhoneNumber = null,
                Email = null
            };

            SetupUser(userId, user);

            var result = await orderService.GetCheckoutDeliveryDetailsAsync(userId);

            Assert.That(result, Is.Not.Null);

            Assert.That(result!.RecipientName, Is.EqualTo(string.Empty));
            Assert.That(result.ShippingAddress, Is.EqualTo(string.Empty));
            Assert.That(result.PhoneNumber, Is.EqualTo(string.Empty));
            Assert.That(result.Email, Is.EqualTo(string.Empty));
        }

        [Test]
        public async Task CreateOrderAsync_ShouldReturnNull_WhenCartIsNull()
        {
            var userId = Guid.NewGuid();

            SetupCart(userId, null);

            var model = new CreateOrderViewModel();

            var result = await orderService.CreateOrderAsync(userId, model);

            Assert.That(result, Is.Null);
        }

        [Test]
        public async Task CreateOrderAsync_ShouldReturnNull_WhenCartIsEmpty()
        {
            var userId = Guid.NewGuid();

            var cart = CreateCart(userId, new List<CartProduct>());

            SetupCart(userId, cart);

            var result = await orderService.CreateOrderAsync(userId, new CreateOrderViewModel());

            Assert.That(result, Is.Null);
        }

        [Test]
        public async Task CreateOrderAsync_ShouldReturnNullAndRollback_WhenInsufficientStock()
        {
            var userId = Guid.NewGuid();

            var product = CreateProduct(Guid.NewGuid(), stock: 1);

            var cartProduct = CreateCartProduct(product, quantity: 5);

            var cart = CreateCart(userId, new List<CartProduct> { cartProduct });

            SetupCart(userId, cart);
            SetupTransactionSuccess();
            SetupRollback();

            var result = await orderService.CreateOrderAsync(userId, new CreateOrderViewModel());

            Assert.That(result, Is.Null);

            mockUnitOfWork.Verify(u => u.RollbackAsync(), Times.Once);
            mockOrderRepository.Verify(r => r.AddAsync(It.IsAny<Order>()), Times.Never);
        }

        [Test]
        public async Task CreateOrderAsync_ShouldRollback_WhenExceptionOccurs()
        {
            var userId = Guid.NewGuid();

            var product = CreateProduct(Guid.NewGuid(), stock: 10);
            var cartProduct = CreateCartProduct(product, 1);

            var cart = CreateCart(userId, new List<CartProduct> { cartProduct });

            SetupCart(userId, cart);

            mockUnitOfWork.Setup(u => u.BeginTransactionAsync()).Returns(Task.CompletedTask);
            mockUnitOfWork.Setup(u => u.SaveChangesAsync()).ThrowsAsync(new Exception());
            SetupRollback();

            var result = await orderService.CreateOrderAsync(userId, new CreateOrderViewModel());

            Assert.That(result, Is.Null);

            mockUnitOfWork.Verify(u => u.RollbackAsync(), Times.Once);
            mockUnitOfWork.Verify(u => u.CommitAsync(), Times.Never);
        }

        [Test]
        public async Task CreateOrderAsync_ShouldCreateOrderAndCommit_WhenValid()
        {
            var userId = Guid.NewGuid();

            var product = CreateProduct(Guid.NewGuid(), stock: 10, price: 100);

            var cartProduct = CreateCartProduct(product, 2);

            var cart = CreateCart(userId, new List<CartProduct> { cartProduct });

            SetupCart(userId, cart);
            SetupTransactionSuccess();

            mockOrderRepository
            .Setup(r => r.AddAsync(It.IsAny<Order>()))
            .Callback<Order>(o => o.Id = 1);

            var model = new CreateOrderViewModel
            {
                RecipientName = "John",
                ShippingAddress = "Address",
                PhoneNumber = "123",
                Email = "mail@test.com"
            };

            var result = await orderService.CreateOrderAsync(userId, model);

            Assert.That(result, Is.EqualTo(1));
            Assert.That(product.QuantityInStock, Is.EqualTo(8));
            Assert.That(cart.Products, Is.Empty);

            mockOrderRepository.Verify(r => r.AddAsync(It.IsAny<Order>()), Times.Once);
            mockProductRepository.Verify(r => r.Update(product), Times.Once);
            mockCartRepository.Verify(r => r.Update(cart), Times.Once);
            mockUnitOfWork.Verify(u => u.SaveChangesAsync(), Times.Once);
            mockUnitOfWork.Verify(u => u.CommitAsync(), Times.Once);
        }

        [Test]
        public async Task CreateOrderAsync_ShouldTrimInputFields()
        {
            var userId = Guid.NewGuid();

            var product = CreateProduct(Guid.NewGuid(), 10, 100);
            var cart = CreateCart(userId, new List<CartProduct>
            {
                CreateCartProduct(product, 1)
            });

            SetupCart(userId, cart);
            SetupTransactionSuccess();

            Order? addedOrder = null;

            mockOrderRepository
            .Setup(r => r.AddAsync(It.IsAny<Order>()))
            .Callback<Order>(o =>
            {
                addedOrder = o;
                o.Id = 1;
            });

            var model = new CreateOrderViewModel
            {
                RecipientName = " John ",
                ShippingAddress = " Address ",
                PhoneNumber = " 123 ",
                Email = " mail@test.com "
            };

            await orderService.CreateOrderAsync(userId, model);

            Assert.That(addedOrder, Is.Not.Null);
            Assert.That(addedOrder!.RecipientName, Is.EqualTo("John"));
            Assert.That(addedOrder.ShippingAddress, Is.EqualTo("Address"));
            Assert.That(addedOrder.PhoneNumber, Is.EqualTo("123"));
            Assert.That(addedOrder.Email, Is.EqualTo("mail@test.com"));
        }

        [Test]
        public async Task CreateOrderAsync_ShouldUpdateStock_ForMultipleProducts()
        {
            var userId = Guid.NewGuid();

            var product1 = CreateProduct(Guid.NewGuid(), stock: 10);
            var product2 = CreateProduct(Guid.NewGuid(), stock: 20);

            var cart = CreateCart(userId, new List<CartProduct>
            {
                CreateCartProduct(product1, 2),
                CreateCartProduct(product2, 5)
            });

            SetupCart(userId, cart);
            SetupTransactionSuccess();

            mockOrderRepository
                .Setup(r => r.AddAsync(It.IsAny<Order>()))
                .Callback<Order>(o => o.Id = 1);

            var model = new CreateOrderViewModel
            {
                RecipientName = "Test",
                ShippingAddress = "Address",
                PhoneNumber = "123",
                Email = "test@test.com"
            };

            var result = await orderService.CreateOrderAsync(userId, model);

            Assert.That(result, Is.Not.Null);
            Assert.That(product1.QuantityInStock, Is.EqualTo(8));
            Assert.That(product2.QuantityInStock, Is.EqualTo(15));
        }

        [Test]
        public async Task GetPaymentSummaryAsync_ShouldReturnNull_WhenOrderDoesNotExist()
        {
            var userId = Guid.NewGuid();
            var orderId = 1L;

            SetupOrderDetails(userId, orderId, null);

            var result = await orderService.GetPaymentSummaryAsync(userId, orderId);

            Assert.That(result, Is.Null);
        }

        [Test]
        public async Task GetPaymentSummaryAsync_ShouldReturnNull_WhenOrderIsNotPendingPayment()
        {
            var userId = Guid.NewGuid();
            var orderId = 1L;

            var order = CreateOrder(orderId, userId, Status.Processing);

            SetupOrderDetails(userId, orderId, order);

            var result = await orderService.GetPaymentSummaryAsync(userId, orderId);

            Assert.That(result, Is.Null);
        }

        [Test]
        public async Task GetPaymentSummaryAsync_ShouldReturnCorrectSummary_WhenValid()
        {
            var userId = Guid.NewGuid();
            var orderId = 1L;

            var product = CreateProduct(Guid.NewGuid(), price: 100);

            var order = CreateOrder(orderId, userId, Status.PendingPayment);

            order.OrdersProducts = new List<OrderProduct>
            {
                CreateOrderProduct(product, quantity: 2, price: 100)
            };

            SetupOrderDetails(userId, orderId, order);

            var result = await orderService.GetPaymentSummaryAsync(userId, orderId);

            Assert.That(result, Is.Not.Null);

            Assert.That(result!.OrderId, Is.EqualTo(order.Id));
            Assert.That(result.ShippingAddress, Is.EqualTo(order.ShippingAddress));
            Assert.That(result.RecipientName, Is.EqualTo(order.RecipientName));
            Assert.That(result.Email, Is.EqualTo(order.Email));
        }

        [Test]
        public async Task GetPaymentSummaryAsync_ShouldCalculateTotalAmountCorrectly()
        {
            var userId = Guid.NewGuid();
            var orderId = 1L;

            var product1 = CreateProduct(Guid.NewGuid(), price: 100);
            var product2 = CreateProduct(Guid.NewGuid(), price: 50);

            var order = CreateOrder(orderId, userId, Status.PendingPayment);

            order.OrdersProducts = new List<OrderProduct>
            {
                CreateOrderProduct(product1, quantity: 2, price: 100),
        	    CreateOrderProduct(product2, quantity: 3, price: 50)
    	    };

            SetupOrderDetails(userId, orderId, order);

            var result = await orderService.GetPaymentSummaryAsync(userId, orderId);

            Assert.That(result, Is.Not.Null);
            Assert.That(result!.TotalAmount, Is.EqualTo(350));
        }

        [Test]
        public async Task AttachStripeSessionAsync_ShouldReturnFalse_WhenOrderDoesNotExist()
        {
            var orderId = 1L;

            SetupOrder(orderId, null);

            var result = await orderService.AttachStripeSessionAsync(orderId, "session_123");

            Assert.That(result, Is.False);

            mockOrderRepository.Verify(r => r.Update(It.IsAny<Order>()), Times.Never);
            mockOrderRepository.Verify(r => r.SaveChangesAsync(), Times.Never);
        }

        [Test]
        public async Task AttachStripeSessionAsync_ShouldReturnFalse_WhenOrderIsNotPendingPayment()
        {
            var orderId = 1L;

            var order = CreateOrder(orderId, Guid.NewGuid(), Status.Processing);

            SetupOrder(orderId, order);

            var result = await orderService.AttachStripeSessionAsync(orderId, "session_123");

            Assert.That(result, Is.False);

            mockOrderRepository.Verify(r => r.Update(It.IsAny<Order>()), Times.Never);
            mockOrderRepository.Verify(r => r.SaveChangesAsync(), Times.Never);
        }

        [Test]
        public async Task AttachStripeSessionAsync_ShouldAttachSessionAndReturnTrue_WhenValid()
        {
            var orderId = 1L;
            var sessionId = "session_123";

            var order = CreateOrder(orderId, Guid.NewGuid(), Status.PendingPayment);

            SetupOrder(orderId, order);

            var result = await orderService.AttachStripeSessionAsync(orderId, sessionId);

            Assert.That(result, Is.True);

            Assert.That(order.StripeSessionId, Is.EqualTo(sessionId));

            mockOrderRepository.Verify(r => r.Update(order), Times.Once);
            mockOrderRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Test]
        public async Task GetPaymentSuccessAsync_ShouldReturnNull_WhenOrderDoesNotExist()
        {
            var userId = Guid.NewGuid();
            var orderId = 1L;

            SetupOrdersQueryable(new List<Order>());

            var result = await orderService.GetPaymentSuccessAsync(userId, orderId);

            Assert.That(result, Is.Null);
        }

        [Test]
        public async Task GetPaymentSuccessAsync_ShouldReturnNull_WhenOrderIsCancelled()
        {
            var userId = Guid.NewGuid();
            var orderId = 1L;

            var order = CreateOrder(orderId, userId, Status.Cancelled);

            SetupOrdersQueryable(new List<Order> { order });

            var result = await orderService.GetPaymentSuccessAsync(userId, orderId);

            Assert.That(result, Is.Null);
        }

        [Test]
        public async Task GetPaymentSuccessAsync_ShouldReturnNull_WhenOrderBelongsToAnotherUser()
        {
            var userId = Guid.NewGuid();
            var otherUserId = Guid.NewGuid();
            var orderId = 1L;

            var order = CreateOrder(orderId, otherUserId, Status.Processing);

            SetupOrdersQueryable(new List<Order> { order });

            var result = await orderService.GetPaymentSuccessAsync(userId, orderId);

            Assert.That(result, Is.Null);
        }

        [Test]
        public async Task GetPaymentSuccessAsync_ShouldReturnCorrectModel_WhenValid()
        {
            var userId = Guid.NewGuid();
            var orderId = 1L;

            var product = CreateProduct(Guid.NewGuid(), price: 100);

            var order = CreateOrder(orderId, userId, Status.Processing);

            order.OrdersProducts = new List<OrderProduct>
            {
                CreateOrderProduct(product, 2, 100)
            };

            SetupOrdersQueryable(new List<Order> { order });

            var result = await orderService.GetPaymentSuccessAsync(userId, orderId);

            Assert.That(result, Is.Not.Null);

            Assert.That(result!.OrderId, Is.EqualTo(order.Id));
            Assert.That(result.Products.Count, Is.EqualTo(1));
        }

        [Test]
        public async Task GetPaymentSuccessAsync_ShouldCalculateTotalAmountCorrectly()
        {
            var userId = Guid.NewGuid();
            var orderId = 1L;

            var product1 = CreateProduct(Guid.NewGuid(), price: 100);
            var product2 = CreateProduct(Guid.NewGuid(), price: 50);

            var order = CreateOrder(orderId, userId, Status.Processing);

            order.OrdersProducts = new List<OrderProduct>
            {
                CreateOrderProduct(product1, 2, 100),
                CreateOrderProduct(product2, 3, 50)
            };

            SetupOrdersQueryable(new List<Order> { order });

            var result = await orderService.GetPaymentSuccessAsync(userId, orderId);

            Assert.That(result, Is.Not.Null);
            Assert.That(result!.TotalAmount, Is.EqualTo(350));
        }

        [Test]
        public async Task MarkOrderAsPaidByOrderIdAsync_ShouldDoNothing_WhenOrderDoesNotExist()
        {
            var orderId = 1L;

            SetupOrder(orderId, null);

            await orderService.MarkOrderAsPaidByOrderIdAsync(orderId);

            mockOrderRepository.Verify(r => r.Update(It.IsAny<Order>()), Times.Never);
            mockOrderRepository.Verify(r => r.SaveChangesAsync(), Times.Never);
        }

        [Test]
        public async Task MarkOrderAsPaidByOrderIdAsync_ShouldDoNothing_WhenOrderIsNotPendingPayment()
        {
            var orderId = 1L;

            var order = CreateOrder(orderId, Guid.NewGuid(), Status.Processing);

            SetupOrder(orderId, order);

            await orderService.MarkOrderAsPaidByOrderIdAsync(orderId);

            Assert.That(order.Status, Is.EqualTo(Status.Processing));

            mockOrderRepository.Verify(r => r.Update(It.IsAny<Order>()), Times.Never);
            mockOrderRepository.Verify(r => r.SaveChangesAsync(), Times.Never);
        }

        [Test]
        public async Task MarkOrderAsPaidByOrderIdAsync_ShouldUpdateStatusAndPaymentMethod_WhenValid()
        {
            var orderId = 1L;

            var order = CreateOrder(orderId, Guid.NewGuid(), Status.PendingPayment);

            SetupOrder(orderId, order);

            await orderService.MarkOrderAsPaidByOrderIdAsync(orderId);

            Assert.That(order.Status, Is.EqualTo(Status.Processing));
            Assert.That(order.PaymentMethod, Is.EqualTo(PaymentMethod.Card));

            mockOrderRepository.Verify(r => r.Update(order), Times.Once);
            mockOrderRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Test]
        public async Task GetOrderProductsAsync_ShouldReturnNull_WhenOrderDoesNotExist()
        {
            var userId = Guid.NewGuid();
            var orderId = 1L;

            SetupOrderDetails(userId, orderId, null);

            var result = await orderService.GetOrderProductsAsync(userId, orderId);

            Assert.That(result, Is.Null);
        }

        [Test]
        public async Task GetOrderProductsAsync_ShouldReturnProducts_WhenOrderExists()
        {
            var userId = Guid.NewGuid();
            var orderId = 1L;

            var product = CreateProduct(Guid.NewGuid(), price: 100);

            var order = CreateOrder(orderId, userId);

            var orderProduct = CreateOrderProduct(product, 2, 100);

            order.OrdersProducts = new List<OrderProduct> { orderProduct };

            SetupOrderDetails(userId, orderId, order);

            var result = await orderService.GetOrderProductsAsync(userId, orderId);

            Assert.That(result, Is.Not.Null);
            Assert.That(result!.Count, Is.EqualTo(1));
            Assert.That(result.First().ProductId, Is.EqualTo(product.Id));
            Assert.That(result.First().Quantity, Is.EqualTo(2));
        }

        [Test]
        public async Task GetOrderProductsAsync_ShouldReturnEmptyList_WhenNoProducts()
        {
            var userId = Guid.NewGuid();
            var orderId = 1L;

            var order = CreateOrder(orderId, userId);
            order.OrdersProducts = new List<OrderProduct>();

            SetupOrderDetails(userId, orderId, order);

            var result = await orderService.GetOrderProductsAsync(userId, orderId);

            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.Empty);
        }

        [Test]
        public async Task MarkOrderAsCashOnDeliveryAsync_ShouldReturnFalse_WhenOrderDoesNotExist()
        {
            var userId = Guid.NewGuid();
            var orderId = 1L;

            SetupOrderDetails(userId, orderId, null);

            var result = await orderService.MarkOrderAsCashOnDeliveryAsync(userId, orderId);

            Assert.That(result, Is.False);

            mockOrderRepository.Verify(r => r.Update(It.IsAny<Order>()), Times.Never);
            mockOrderRepository.Verify(r => r.SaveChangesAsync(), Times.Never);
        }

        [Test]
        public async Task MarkOrderAsCashOnDeliveryAsync_ShouldReturnFalse_WhenOrderIsNotPendingPayment()
        {
            var userId = Guid.NewGuid();
            var orderId = 1L;

            var order = CreateOrder(orderId, userId, Status.Processing);

            SetupOrderDetails(userId, orderId, order);

            var result = await orderService.MarkOrderAsCashOnDeliveryAsync(userId, orderId);

            Assert.That(result, Is.False);

            Assert.That(order.Status, Is.EqualTo(Status.Processing));

            mockOrderRepository.Verify(r => r.Update(It.IsAny<Order>()), Times.Never);
            mockOrderRepository.Verify(r => r.SaveChangesAsync(), Times.Never);
        }

        [Test]
        public async Task MarkOrderAsCashOnDeliveryAsync_ShouldUpdateOrderAndReturnTrue_WhenValid()
        {
            var userId = Guid.NewGuid();
            var orderId = 1L;

            var order = CreateOrder(orderId, userId, Status.PendingPayment);

            SetupOrderDetails(userId, orderId, order);

            var result = await orderService.MarkOrderAsCashOnDeliveryAsync(userId, orderId);

            Assert.That(result, Is.True);

            Assert.That(order.Status, Is.EqualTo(Status.Processing));
            Assert.That(order.PaymentMethod, Is.EqualTo(PaymentMethod.CashOnDelivery));

            mockOrderRepository.Verify(r => r.Update(order), Times.Once);
            mockOrderRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Test]
        public async Task GetMyOrdersPagedAsync_ShouldReturnEmpty_WhenNoOrders()
        {
            var userId = Guid.NewGuid();

            SetupOrdersCountByUser(userId, 0);

            var result = await orderService.GetMyOrdersPagedAsync(userId, 1, 5);

            Assert.That(result.Orders, Is.Empty);
            Assert.That(result.CurrentPage, Is.EqualTo(1));
            Assert.That(result.TotalPages, Is.EqualTo(0));
        }

        [Test]
        public async Task GetMyOrdersPagedAsync_ShouldReturnOrders_WhenValid()
        {
            var userId = Guid.NewGuid();

            var product = CreateProduct(Guid.NewGuid());

            var order = CreateOrder(1, userId);
            order.OrdersProducts = new List<OrderProduct>
            {
                CreateOrderProduct(product, 2, 100)
            };

            SetupOrdersCountByUser(userId, 1);
            SetupPagedOrders(userId, 1, 5, new List<Order> { order });

            var result = await orderService.GetMyOrdersPagedAsync(userId, 1, 5);

            Assert.That(result.Orders.Count, Is.EqualTo(1));

            var item = result.Orders.First();

            Assert.That(item.Id, Is.EqualTo(order.Id));
            Assert.That(item.RecipientName, Is.EqualTo(order.RecipientName));
            Assert.That(item.Products.Count, Is.EqualTo(1));
        }

        [Test]
        public async Task GetMyOrdersPagedAsync_ShouldCalculateTotalPagesCorrectly()
        {
            var userId = Guid.NewGuid();

            SetupOrdersCountByUser(userId, 10);

            SetupPagedOrders(userId, 1, 5, new List<Order>());

            var result = await orderService.GetMyOrdersPagedAsync(userId, 1, 5);

            Assert.That(result.TotalPages, Is.EqualTo(2));
        }

        [Test]
        public async Task GetMyOrdersPagedAsync_ShouldClampPage_WhenPageExceedsTotalPages()
        {
            var userId = Guid.NewGuid();

            SetupOrdersCountByUser(userId, 1);

            SetupPagedOrders(userId, 1, 5, new List<Order>());

            var result = await orderService.GetMyOrdersPagedAsync(userId, 10, 5);

            Assert.That(result.CurrentPage, Is.EqualTo(1));
        }

        [Test]
        public async Task GetMyOrdersPagedAsync_ShouldNormalizePaging_WhenInvalidInput()
        {
            var userId = Guid.NewGuid();

            SetupOrdersCountByUser(userId, 0);

            var result = await orderService.GetMyOrdersPagedAsync(userId, 0, 0);

            Assert.That(result.CurrentPage, Is.EqualTo(1));
        }

        [Test]
        public async Task GetMyOrdersPagedAsync_ShouldMapProductsCorrectly()
        {
            var userId = Guid.NewGuid();

            var product = CreateProduct(Guid.NewGuid());
            product.Name = "Test Product";
            product.Description = "Desc";

            var order = CreateOrder(1, userId);
            order.OrdersProducts = new List<OrderProduct>
            {
                CreateOrderProduct(product, 3, 50)
            };

            SetupOrdersCountByUser(userId, 1);
            SetupPagedOrders(userId, 1, 5, new List<Order> { order });

            var result = await orderService.GetMyOrdersPagedAsync(userId, 1, 5);

            var item = result.Orders.First();
            var productVm = item.Products.First();

            Assert.That(productVm.ProductName, Is.EqualTo(product.Name));
            Assert.That(productVm.Description, Is.EqualTo(product.Description));
            Assert.That(productVm.Price, Is.EqualTo(50));
            Assert.That(productVm.Quantity, Is.EqualTo(3));
        }

        [Test]
        public async Task GetEditPageAsync_ShouldReturnNull_WhenOrderDoesNotExist()
        {
            var orderId = 1L;

            SetupOrderDetails(orderId, null);

            var result = await orderService.GetEditPageAsync(orderId);

            Assert.That(result, Is.Null);
        }

        [Test]
        public async Task GetEditPageAsync_ShouldReturnCorrectModel_WhenOrderExists()
        {
            var orderId = 1L;

            var order = CreateOrder(orderId, Guid.NewGuid(), Status.PendingPayment);

            SetupOrderDetails(orderId, order);

            var result = await orderService.GetEditPageAsync(orderId);

            Assert.That(result, Is.Not.Null);

            Assert.That(result!.Id, Is.EqualTo(order.Id));
            Assert.That(result.PaymentMethod, Is.EqualTo(order.PaymentMethod));
            Assert.That(result.CurrentStatus, Is.EqualTo(order.Status));
        }

        [Test]
        public async Task GetEditPageAsync_ShouldSetAllowedStatuses_WhenPendingPayment()
        {
            var order = CreateOrder(1, Guid.NewGuid(), Status.PendingPayment);

            SetupOrderDetails(order.Id, order);

            var result = await orderService.GetEditPageAsync(order.Id);

            Assert.That(result!.AllowedStatuses.Count, Is.EqualTo(1));
            Assert.That(result.AllowedStatuses, Does.Contain(Status.Cancelled));
        }

        [Test]
        public async Task GetEditPageAsync_ShouldSetAllowedStatuses_WhenProcessing()
        {
            var order = CreateOrder(1, Guid.NewGuid(), Status.Processing);

            SetupOrderDetails(order.Id, order);

            var result = await orderService.GetEditPageAsync(order.Id);

            Assert.That(result!.AllowedStatuses, Does.Contain(Status.Shipped));
            Assert.That(result.AllowedStatuses, Does.Contain(Status.Cancelled));
        }

        [Test]
        public async Task GetEditPageAsync_ShouldDisableShippingEdit_WhenDelivered()
        {
            var order = CreateOrder(1, Guid.NewGuid(), Status.Delivered);

            SetupOrderDetails(order.Id, order);

            var result = await orderService.GetEditPageAsync(order.Id);

            Assert.That(result!.CanEditShipping, Is.False);
        }

        [Test]
        public async Task GetEditPageAsync_ShouldAllowShippingEdit_WhenPendingOrProcessing()
        {
            var order = CreateOrder(1, Guid.NewGuid(), Status.PendingPayment);

            SetupOrderDetails(order.Id, order);

            var result = await orderService.GetEditPageAsync(order.Id);

            Assert.That(result!.CanEditShipping, Is.True);
        }

        [Test]
        public async Task GetEditPageAsync_ShouldPopulateNestedModelsCorrectly()
        {
            var order = CreateOrder(1, Guid.NewGuid(), Status.PendingPayment);

            SetupOrderDetails(order.Id, order);

            var result = await orderService.GetEditPageAsync(order.Id);

            Assert.That(result, Is.Not.Null);

            Assert.That(result!.Status.Id, Is.EqualTo(order.Id));
            Assert.That(result.Status.NewStatus, Is.EqualTo(order.Status));

            Assert.That(result.Shipping.Id, Is.EqualTo(order.Id));
            Assert.That(result.Shipping.RecipientName, Is.EqualTo(order.RecipientName));
            Assert.That(result.Shipping.ShippingAddress, Is.EqualTo(order.ShippingAddress));
        }

        [Test]
        public async Task EditStatusAsync_ShouldReturnFalse_WhenOrderDoesNotExist()
        {
            var orderId = 1L;

            SetupOrderDetails(orderId, null);

            var result = await orderService.EditStatusAsync(orderId, Status.Cancelled);

            Assert.That(result, Is.False);
        }

        [Test]
        public async Task EditStatusAsync_ShouldReturnFalse_WhenOrderIsDelivered()
        {
            var order = CreateOrder(1, Guid.NewGuid(), Status.Delivered);

            SetupOrderDetails(order.Id, order);

            var result = await orderService.EditStatusAsync(order.Id, Status.Cancelled);

            Assert.That(result, Is.False);
        }

        [Test]
        public async Task EditStatusAsync_ShouldReturnFalse_WhenOrderIsCancelled()
        {
            var order = CreateOrder(1, Guid.NewGuid(), Status.Cancelled);

            SetupOrderDetails(order.Id, order);

            var result = await orderService.EditStatusAsync(order.Id, Status.Processing);

            Assert.That(result, Is.False);
        }

        [Test]
        public async Task EditStatusAsync_ShouldReturnFalse_WhenTransitionIsNotAllowed()
        {
            var order = CreateOrder(1, Guid.NewGuid(), Status.PendingPayment);

            SetupOrderDetails(order.Id, order);

            var result = await orderService.EditStatusAsync(order.Id, Status.Shipped);

            Assert.That(result, Is.False);
        }

        [Test]
        public async Task EditStatusAsync_ShouldUpdateStatusAndReturnTrue_WhenValidTransition()
        {
            var order = CreateOrder(1, Guid.NewGuid(), Status.Processing);

            SetupOrderDetails(order.Id, order);
            SetupTransactionSuccess();

            var result = await orderService.EditStatusAsync(order.Id, Status.Shipped);

            Assert.That(result, Is.True);
            Assert.That(order.Status, Is.EqualTo(Status.Shipped));

            mockOrderRepository.Verify(r => r.Update(order), Times.Once);
            mockUnitOfWork.Verify(u => u.CommitAsync(), Times.Once);
        }

        [Test]
        public async Task EditStatusAsync_ShouldRestoreStock_WhenCancelled()
        {
            var product = CreateProduct(Guid.NewGuid(), stock: 5);

            var order = CreateOrder(1, Guid.NewGuid(), Status.Processing);

            order.OrdersProducts = new List<OrderProduct>
            {
                new OrderProduct
                {
                    Product = product,
                    Quantity = 3
                }
            };

            SetupOrderDetails(order.Id, order);
            SetupTransactionSuccess();

            var result = await orderService.EditStatusAsync(order.Id, Status.Cancelled);

            Assert.That(result, Is.True);
            Assert.That(product.QuantityInStock, Is.EqualTo(8));
        }

        [Test]
        public async Task EditStatusAsync_ShouldRollbackAndReturnFalse_WhenExceptionOccurs()
        {
            var order = CreateOrder(1, Guid.NewGuid(), Status.Processing);

            SetupOrderDetails(order.Id, order);

            mockUnitOfWork.Setup(u => u.BeginTransactionAsync()).Returns(Task.CompletedTask);
            mockUnitOfWork.Setup(u => u.SaveChangesAsync()).ThrowsAsync(new Exception());
            SetupRollback();

            var result = await orderService.EditStatusAsync(order.Id, Status.Shipped);

            Assert.That(result, Is.False);

            mockUnitOfWork.Verify(u => u.RollbackAsync(), Times.Once);
        }

        [Test]
        public async Task EditShippingDetailsAsync_ShouldReturnFalse_WhenOrderDoesNotExist()
        {
            var model = new OrderEditShippingDetailsInputModel
            {
                Id = 1,
                RecipientName = "Test",
                ShippingAddress = "Address",
                PhoneNumber = "123",
                Email = "test@test.com"
            };

            mockOrderRepository
                .Setup(r => r.GetByIdAsync(model.Id))
                .ReturnsAsync((Order?)null);

            var result = await orderService.EditShippingDetailsAsync(model);

            Assert.That(result, Is.False);

            mockOrderRepository.Verify(r => r.Update(It.IsAny<Order>()), Times.Never);
            mockOrderRepository.Verify(r => r.SaveChangesAsync(), Times.Never);
        }

        [Test]
        public async Task EditShippingDetailsAsync_ShouldReturnFalse_WhenStatusIsNotEditable()
        {
            var order = CreateOrder(1, Guid.NewGuid(), Status.Shipped);

            mockOrderRepository
                .Setup(r => r.GetByIdAsync(order.Id))
                .ReturnsAsync(order);

            var model = new OrderEditShippingDetailsInputModel
            {
                Id = order.Id,
                RecipientName = "Test",
                ShippingAddress = "Address",
                PhoneNumber = "123",
                Email = "test@test.com"
            };

            var result = await orderService.EditShippingDetailsAsync(model);

            Assert.That(result, Is.False);

            mockOrderRepository.Verify(r => r.Update(It.IsAny<Order>()), Times.Never);
        }

        [Test]
        public async Task EditShippingDetailsAsync_ShouldUpdateAndReturnTrue_WhenValid()
        {
            var order = CreateOrder(1, Guid.NewGuid(), Status.PendingPayment);

            mockOrderRepository
                .Setup(r => r.GetByIdAsync(order.Id))
                .ReturnsAsync(order);

            var model = new OrderEditShippingDetailsInputModel
            {
                Id = order.Id,
                RecipientName = "  John Doe  ",
                ShippingAddress = "  Some Address  ",
                PhoneNumber = " 123456 ",
                Email = " test@test.com "
            };

            var result = await orderService.EditShippingDetailsAsync(model);

            Assert.That(result, Is.True);

            Assert.That(order.RecipientName, Is.EqualTo("John Doe")); //Trimmed
            Assert.That(order.ShippingAddress, Is.EqualTo("Some Address")); //Trimmed
            Assert.That(order.PhoneNumber, Is.EqualTo("123456")); //Trimmed
            Assert.That(order.Email, Is.EqualTo("test@test.com")); //Trimmed

            mockOrderRepository.Verify(r => r.Update(order), Times.Once);
            mockOrderRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Test]
        public async Task GetManageOrdersPageAsync_ShouldReturnEmpty_WhenNoOrders()
        {
            SetupOrderCount(0);

            var result = await orderService.GetManageOrdersPageAsync(1, 10);

            Assert.That(result.Orders, Is.Empty);
            Assert.That(result.CurrentPage, Is.EqualTo(1));
            Assert.That(result.TotalPages, Is.EqualTo(0));
        }

        [Test]
        public async Task GetManageOrdersPageAsync_ShouldNormalizePaging_WhenInvalidInput()
        {
            SetupOrderCount(0);

            var result = await orderService.GetManageOrdersPageAsync(0, 0);

            Assert.That(result.CurrentPage, Is.EqualTo(1));
        }

        [Test]
        public async Task GetManageOrdersPageAsync_ShouldClampPage_WhenPageExceedsTotalPages()
        {
            var order = CreateOrder(1, Guid.NewGuid(), Status.PendingPayment);

            SetupOrderCount(1);
            SetupPagedOrders(1, 5, new List<Order> { order });

            var result = await orderService.GetManageOrdersPageAsync(10, 5);

            Assert.That(result.CurrentPage, Is.EqualTo(1));
        }

        [Test]
        public async Task GetManageOrdersPageAsync_ShouldReturnCorrectNumberOfOrders()
        {
            var order1 = CreateOrder(1, Guid.NewGuid(), Status.PendingPayment);
            var order2 = CreateOrder(2, Guid.NewGuid(), Status.Processing);

            SetupOrderCount(2);
            SetupPagedOrders(1, 5, new List<Order> { order1, order2 });

            var result = await orderService.GetManageOrdersPageAsync(1, 5);

            Assert.That(result.Orders.Count, Is.EqualTo(2));
        }

        [Test]
        public async Task GetManageOrdersPageAsync_ShouldMapOrdersCorrectly()
        {
            var product = CreateProduct(Guid.NewGuid());

            var order = CreateOrder(1, Guid.NewGuid(), Status.PendingPayment);

            order.OrdersProducts = new List<OrderProduct>
            {
                new OrderProduct
                {
                    Product = product,
                    Quantity = 2,
                    UnitPrice = 100
                }
            };

            SetupOrderCount(1);
            SetupPagedOrders(1, 5, new List<Order> { order });

            var result = (await orderService.GetManageOrdersPageAsync(1, 5)).Orders.ToList();

            var item = result[0];

            Assert.That(item.Id, Is.EqualTo(order.Id));
            Assert.That(item.RecipientName, Is.EqualTo(order.RecipientName));
            Assert.That(item.Status, Is.EqualTo(order.Status.ToString()));
            Assert.That(item.Products.Count, Is.EqualTo(1));
        }

        [Test]
        public async Task GetManageOrdersPageAsync_ShouldCalculateTotalPagesCorrectly()
        {
            SetupOrderCount(12);
            SetupPagedOrders(1, 5, new List<Order>());

            var result = await orderService.GetManageOrdersPageAsync(1, 5);

            Assert.That(result.TotalPages, Is.EqualTo(3));
        }

        private User CreateUser(Guid id)
        {
            return new User
            {
                Id = id,
                FullName = "Test User",
                Address = "Test Address",
                PhoneNumber = "123456",
                Email = "test@abv.bg"
            };
        }

        private Product CreateProduct(Guid id, int stock = 10, decimal price = 100)
        {
            return new Product
            {
                Id = id,
                Name = "Test Product",
                Description = "Test Description",
                QuantityInStock = stock,
                Price = price,
                ImageUrl = "img.png"
            };
        }

        private Cart CreateCart(Guid userId, List<CartProduct>? products = null)
        {
            return new Cart
            {
                Id = userId,
                Products = products ?? new List<CartProduct>()
            };
        }

        private CartProduct CreateCartProduct(Product product, int quantity)
        {
            return new CartProduct
            {
                ProductId = product.Id,
                Product = product,
                Quantity = quantity
            };
        }

        private Order CreateOrder(long id, Guid userId, Status status = Status.PendingPayment)
        {
            return new Order
            {
                Id = id,
                UserId = userId,
                RecipientName = "Test User",
                ShippingAddress = "Address",
                PhoneNumber = "123",
                Email = "test@abv.bg",
                OrderDate = DateTime.UtcNow,
                Status = status,
                PaymentMethod = PaymentMethod.Unknown,
                OrdersProducts = new List<OrderProduct>()
            };
        }

        private OrderProduct CreateOrderProduct(Product product, int quantity, decimal price)
        {
            return new OrderProduct
            {
                Product = product,
                ProductId = product.Id,
                Quantity = quantity,
                UnitPrice = price
            };
        }

        private void SetupUser(Guid userId, User? user)
        {
            mockUserRepository
            .Setup(r => r.GetByIdAsync(userId))
            .ReturnsAsync(user);
        }

        private void SetupCart(Guid userId, Cart? cart)
        {
            mockCartRepository
            .Setup(r => r.GetCartWithProductsAsync(userId))
            .ReturnsAsync(cart);
        }

        private void SetupOrder(long orderId, Order? order)
        {
            mockOrderRepository
            .Setup(r => r.GetByIdAsync(orderId))
            .ReturnsAsync(order);
        }

        private void SetupOrdersQueryable(List<Order> orders)
        {
            var mockQueryable = orders.AsQueryable().BuildMockDbSet();

            mockOrderRepository
                .Setup(r => r.GetAllAttached())
                .Returns(mockQueryable.Object);
        }

        private void SetupOrderCount(int count)
        {
            mockOrderRepository
                .Setup(r => r.GetCountAsync())
                .ReturnsAsync(count);
        }

        private void SetupPagedOrders(int page, int pageSize, List<Order> orders)
        {
            mockOrderRepository
                .Setup(r => r.GetOrdersPagedAsync(page, pageSize))
                .ReturnsAsync(orders);
        }

        private void SetupOrderDetails(Guid userId, long orderId, Order? order)
        {
            mockOrderRepository
            .Setup(r => r.GetOrderDetailsAsync(userId, orderId))
            .ReturnsAsync(order);
        }

        private void SetupOrderDetails(long orderId, Order? order)
        {
            mockOrderRepository
            .Setup(r => r.GetOrderDetailsAsync(orderId))
            .ReturnsAsync(order);
        }

        private void SetupOrdersCountByUser(Guid userId, int count)
        {
            mockOrderRepository
            .Setup(r => r.GetCountByUserAsync(userId))
            .ReturnsAsync(count);
        }

        private void SetupPagedOrders(Guid userId, int page, int pageSize, IReadOnlyList<Order> orders)
        {
            mockOrderRepository
            .Setup(r => r.GetPagedByUserAsync(userId, page, pageSize))
            .ReturnsAsync(orders);
        }

        private void SetupTransactionSuccess()
        {
            mockUnitOfWork.Setup(u => u.BeginTransactionAsync()).Returns(Task.CompletedTask);
            mockUnitOfWork.Setup(u => u.SaveChangesAsync()).Returns(Task.CompletedTask);
            mockUnitOfWork.Setup(u => u.CommitAsync()).Returns(Task.CompletedTask);
        }

        private void SetupRollback()
        {
            mockUnitOfWork.Setup(u => u.RollbackAsync()).Returns(Task.CompletedTask);
        }
    }
}