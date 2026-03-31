using Microsoft.AspNetCore.Identity;
using MockQueryable.Moq;
using Moq;
using NUnit.Framework;
using TechStore.Data.Models;
using TechStore.Data.Repository.Interfaces;
using TechStore.Web.ViewModels.User;

namespace TechStore.Services.Core.Tests
{
    [TestFixture]
    public class UserServiceTests
    {
        private Mock<IUserRepository> mockUserRepository = null!;
        private Mock<UserManager<User>> mockUserManager = null!;
        private Mock<RoleManager<IdentityRole<Guid>>> mockRoleManager = null!;
        private UserService userService = null!;

        [SetUp]
        public void SetUp()
        {
            mockUserRepository = new Mock<IUserRepository>();

            mockUserManager = CreateUserManagerMock();
            mockRoleManager = CreateRoleManagerMock();

            userService = new UserService(
                mockUserRepository.Object,
                mockUserManager.Object,
                mockRoleManager.Object);
        }

        [Test]
        public void PassAlways()
        {
            // Test that will always pass to show that the SetUp is working
            Assert.Pass();
        }

        [Test]
        public async Task AssignRoleAsync_ShouldReturnFalse_WhenUserDoesNotExist()
        {
            var userId = Guid.NewGuid();

            SetupFindUser(userId, null);

            var result = await userService.AssignRoleAsync(userId, "Admin");

            Assert.That(result, Is.False);
        }

        [Test]
        public async Task AssignRoleAsync_ShouldReturnTrue_WhenUserAlreadyHasRole()
        {
            var userId = Guid.NewGuid();
            var user = CreateUser(userId, "test@abv.bg");

            SetupFindUser(userId, user);
            SetupUserRoles(user, new List<string> { "Admin" });

            var result = await userService.AssignRoleAsync(userId, "Admin");

            Assert.That(result, Is.True);

            mockUserManager.Verify(m => m.RemoveFromRolesAsync(It.IsAny<User>(), It.IsAny<IEnumerable<string>>()), Times.Never);
        }

        [Test]
        public async Task AssignRoleAsync_ShouldReturnFalse_WhenRemoveRolesFails()
        {
            var userId = Guid.NewGuid();
            var user = CreateUser(userId, "test@abv.bg");

            SetupFindUser(userId, user);
            SetupUserRoles(user, new List<string> { "User" });
            SetupRemoveRoles(user, FailedResult());

            var result = await userService.AssignRoleAsync(userId, "Admin");

            Assert.That(result, Is.False);
        }

        [Test]
        public async Task AssignRoleAsync_ShouldReturnFalse_WhenAddRoleFails()
        {
            var userId = Guid.NewGuid();
            var user = CreateUser(userId, "test@abv.bg");

            SetupFindUser(userId, user);
            SetupUserRoles(user, new List<string> { "User" });
            SetupRemoveRoles(user, SuccessResult());
            SetupAddRole(user, "Admin", FailedResult());

            var result = await userService.AssignRoleAsync(userId, "Admin");

            Assert.That(result, Is.False);
        }

        [Test]
        public async Task AssignRoleAsync_ShouldReturnTrue_WhenRoleAssignedSuccessfully()
        {
            var userId = Guid.NewGuid();
            var user = CreateUser(userId, "test@abv.bg");

            SetupFindUser(userId, user);
            SetupUserRoles(user, new List<string> { "User" });
            SetupRemoveRoles(user, SuccessResult());
            SetupAddRole(user, "Admin", SuccessResult());

            var result = await userService.AssignRoleAsync(userId, "Admin");

            Assert.That(result, Is.True);

            mockUserManager.Verify(m => m.RemoveFromRolesAsync(user, It.IsAny<IEnumerable<string>>()), Times.Once);
            mockUserManager.Verify(m => m.AddToRoleAsync(user, "Admin"), Times.Once);
        }

        [Test]
        public async Task GetPagedAsync_ShouldReturnEmptyResult_WhenNoUsers()
        {
            SetupCount(0);

            var result = await userService.GetPagedAsync(1, 10, Guid.NewGuid());

            Assert.That(result.Users, Is.Empty);
            Assert.That(result.CurrentPage, Is.EqualTo(1));
            Assert.That(result.TotalPages, Is.EqualTo(0));
        }

        [Test]
        public async Task GetPagedAsync_ShouldReturnUsersMappedCorrectly()
        {
            var currentUserId = Guid.NewGuid();
            var user1 = CreateUser(Guid.NewGuid(), "b@abv.bg");
            var user2 = CreateUser(Guid.NewGuid(), "a@abv.bg");

            var users = new List<User> { user1, user2 };

            var mockQueryable = MockHelper.CreateMockQueryable(users);

            SetupCount(2);
            SetupUsers(mockQueryable);
            SetupUserRoles(user1, new List<string> { "Admin" });
            SetupUserRoles(user2, new List<string> { "User" });
            SetupAllRoles("Admin", "User");

            var result = await userService.GetPagedAsync(1, 10, currentUserId);

            Assert.That(result.Users.Count, Is.EqualTo(2));
            Assert.That(result.Users.First().Email, Is.EqualTo("a@abv.bg"));
            Assert.That(result.AllRoles.Count, Is.EqualTo(2));
        }

        [Test]
        public async Task GetPagedAsync_ShouldExcludeCurrentUser()
        {
            var currentUserId = Guid.NewGuid();

            var user1 = CreateUser(currentUserId, "me@abv.bg");
            var user2 = CreateUser(Guid.NewGuid(), "other@abv.bg");

            var users = new List<User> { user1, user2 };

            var mockQueryable = MockHelper.CreateMockQueryable(users);

            SetupCount(2);
            SetupUsers(mockQueryable);
            SetupUserRoles(user2, new List<string>());
            SetupAllRoles();

            var result = await userService.GetPagedAsync(1, 10, currentUserId);

            Assert.That(result.Users.Count, Is.EqualTo(1));
            Assert.That(result.Users.First().Email, Is.EqualTo("other@abv.bg"));
        }

        [Test]
        public async Task GetPagedAsync_ShouldClampPage_WhenPageExceedsTotalPages()
        {
            var user = CreateUser(Guid.NewGuid(), "test@abv.bg");

            var mockQueryable = MockHelper.CreateMockQueryable(new List<User> { user });

            SetupCount(1);
            SetupUsers(mockQueryable);
            SetupUserRoles(user, new List<string>());
            SetupAllRoles();

            var result = await userService.GetPagedAsync(10, 10, Guid.NewGuid());

            Assert.That(result.CurrentPage, Is.EqualTo(1));
        }

        [Test]
        public async Task GetPagedAsync_ShouldNormalizePaging_WhenInvalidInput()
        {
            SetupCount(0);

            var result = await userService.GetPagedAsync(0, 0, Guid.NewGuid());

            Assert.That(result.CurrentPage, Is.EqualTo(1));
        }

        [Test]
        public async Task GetPagedAsync_ShouldReturnAllRoles()
        {
            var user = CreateUser(Guid.NewGuid(), "test@abv.bg");

            var mockQueryable = MockHelper.CreateMockQueryable(new List<User> { user });

            SetupCount(1);
            SetupUsers(mockQueryable);
            SetupUserRoles(user, new List<string>());
            SetupAllRoles("Admin", "User");

            var result = await userService.GetPagedAsync(1, 10, Guid.NewGuid());

            Assert.That(result.AllRoles, Does.Contain("Admin"));
            Assert.That(result.AllRoles, Does.Contain("User"));
        }

        [Test]
        public async Task GetDeliveryDetailsAsync_ShouldReturnNull_WhenUserDoesNotExist()
        {
            var userId = Guid.NewGuid();

            SetupFindUser(userId, null);

            var result = await userService.GetDeliveryDetailsAsync(userId);

            Assert.That(result, Is.Null);
        }

        [Test]
        public async Task GetDeliveryDetailsAsync_ShouldReturnCorrectModel_WhenUserExists()
        {
            var userId = Guid.NewGuid();

            var user = CreateUser(userId, "test@abv.bg");

            SetupFindUser(userId, user);

            var result = await userService.GetDeliveryDetailsAsync(userId);

            Assert.That(result, Is.Not.Null);
            Assert.That(result!.FullName, Is.EqualTo(user.FullName));
            Assert.That(result.Address, Is.EqualTo(user.Address));
            Assert.That(result.PhoneNumber, Is.EqualTo(user.PhoneNumber));
        }

        [Test]
        public async Task GetDeliveryDetailsAsync_ShouldReturnEmptyStrings_WhenUserPropertiesAreNull()
        {
            var userId = Guid.NewGuid();

            var user = new User()
            {
                FullName = null,
                Address = null,
                PhoneNumber = null,
            };

            SetupFindUser(userId, user);

            var result = await userService.GetDeliveryDetailsAsync(userId);

            Assert.That(result, Is.Not.Null);
            Assert.That(result!.FullName, Is.EqualTo(string.Empty));
            Assert.That(result.Address, Is.EqualTo(string.Empty));
            Assert.That(result.PhoneNumber, Is.EqualTo(string.Empty));
        }

        [Test]
        public async Task UpdateDeliveryDetailsAsync_ShouldReturnFalse_WhenUserDoesNotExist()
        {
            var userId = Guid.NewGuid();

            SetupFindUser(userId, null);

            var model = new DeliveryDetailsViewModel
            {
                FullName = "Test",
                Address = "Address",
                PhoneNumber = "123"
            };

            var result = await userService.UpdateDeliveryDetailsAsync(userId, model);

            Assert.That(result, Is.False);

            mockUserManager.Verify(m => m.UpdateAsync(It.IsAny<User>()), Times.Never);
        }

        [Test]
        public async Task UpdateDeliveryDetailsAsync_ShouldReturnFalse_WhenUpdateFails()
        {
            var userId = Guid.NewGuid();

            var user = CreateUser(userId, "test@abv.bg");

            SetupFindUser(userId, user);

            mockUserManager
                .Setup(m => m.UpdateAsync(user))
                .ReturnsAsync(IdentityResult.Failed());

            var model = new DeliveryDetailsViewModel
            {
                FullName = "Test",
                Address = "Address",
                PhoneNumber = "123"
            };

            var result = await userService.UpdateDeliveryDetailsAsync(userId, model);

            Assert.That(result, Is.False);
        }

        [Test]
        public async Task UpdateDeliveryDetailsAsync_ShouldUpdateUserAndReturnTrue_WhenValid()
        {
            var userId = Guid.NewGuid();

            var user = CreateUser(userId, "test@abv.bg");

            SetupFindUser(userId, user);

            mockUserManager
                .Setup(m => m.UpdateAsync(user))
                .ReturnsAsync(IdentityResult.Success);

            var model = new DeliveryDetailsViewModel
            {
                FullName = "  Some Name  ",
                Address = "  Some Address  ",
                PhoneNumber = "123456"
            };

            var result = await userService.UpdateDeliveryDetailsAsync(userId, model);

            Assert.That(result, Is.True);

            Assert.That(user.FullName, Is.EqualTo("Some Name")); // Trimmed
            Assert.That(user.Address, Is.EqualTo("Some Address")); // Trimmed
            Assert.That(user.PhoneNumber, Is.EqualTo(model.PhoneNumber));

            mockUserManager.Verify(m => m.UpdateAsync(user), Times.Once);
        }

        private Mock<UserManager<User>> CreateUserManagerMock()
        {
            var store = new Mock<IUserStore<User>>();

            return new Mock<UserManager<User>>(
                store.Object,
                null!, null!, null!, null!,
                null!, null!, null!, null!);
        }

        private Mock<RoleManager<IdentityRole<Guid>>> CreateRoleManagerMock()
        {
            var store = new Mock<IRoleStore<IdentityRole<Guid>>>();

            return new Mock<RoleManager<IdentityRole<Guid>>>(
                store.Object,
                null!, null!, null!, null!);
        }

        private User CreateUser(Guid userId, string email)
        {
            return new User
            {
                Id = userId,
                Email = email,
                FullName = "Test Name",
                Address = "Test Address",
                PhoneNumber = "000"
            };
        }

        private void SetupCount(int count)
        {
            mockUserRepository
                .Setup(r => r.GetCountAsync())
                .ReturnsAsync(count);
        }

        private void SetupUsers(IQueryable<User> mockQueryable)
        {
            mockUserRepository
                .Setup(r => r.GetAllAttached())
                .Returns(mockQueryable);
        }

        private void SetupFindUser(Guid userId, User? user)
        {
            mockUserManager
                .Setup(m => m.FindByIdAsync(userId.ToString()))
                .ReturnsAsync(user);
        }

        private void SetupUserRoles(User user, IList<string> roles)
        {
            mockUserManager
                .Setup(m => m.GetRolesAsync(user))
                .ReturnsAsync(roles);
        }

        private void SetupAllRoles(params string[] roles)
        {
            var roleEntities = roles
                .Select(r => new IdentityRole<Guid> { Name = r })
                .ToList();

            var mockDbSet = roleEntities.AsQueryable().BuildMockDbSet();

            mockRoleManager
                .Setup(r => r.Roles)
                .Returns(mockDbSet.Object);
        }

        private IdentityResult SuccessResult() => IdentityResult.Success;

        private IdentityResult FailedResult()
            => IdentityResult.Failed(new IdentityError());

        private void SetupRemoveRoles(User user, IdentityResult result)
        {
            mockUserManager
                .Setup(m => m.RemoveFromRolesAsync(user, It.IsAny<IEnumerable<string>>()))
                .ReturnsAsync(result);
        }

        private void SetupAddRole(User user, string role, IdentityResult result)
        {
            mockUserManager
                .Setup(m => m.AddToRoleAsync(user, role))
                .ReturnsAsync(result);
        }
    }
}