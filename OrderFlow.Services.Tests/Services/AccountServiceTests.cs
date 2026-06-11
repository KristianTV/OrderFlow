using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Moq;
using OrderFlow.Data;
using OrderFlow.Data.Models;
using OrderFlow.Data.Models.Enums;
using OrderFlow.Services.Core;
using OrderFlow.ViewModels.User;

namespace OrderFlow.Tests.Services
{
    [TestFixture]
    public class AccountServiceTests
    {
        private OrderFlowDbContext _context = null!;
        private Mock<UserManager<ApplicationUser>> _userManagerMock = null!;
        private AccountService _accountService = null!;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<OrderFlowDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            _context = new OrderFlowDbContext(options);
            _userManagerMock = CreateUserManagerMock();
            _accountService = new AccountService(_context, _userManagerMock.Object);
        }

        [TearDown]
        public void TearDown()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [Test]
        public async Task GetAll_ReturnsUsers()
        {
            await _context.Users.AddAsync(new ApplicationUser { Id = Guid.NewGuid(), UserName = "User" });
            await _context.SaveChangesAsync();

            var result = _accountService.GetAll().ToList();

            Assert.That(result, Has.Count.EqualTo(1));
            Assert.That(result[0].UserName, Is.EqualTo("User"));
        }

        [Test]
        public async Task RegisterAsync_CreatesPersonalProfileAndAddsUserRole_WhenIdentityCreateSucceeds()
        {
            _userManagerMock
                .Setup(manager => manager.CreateAsync(It.IsAny<ApplicationUser>(), "Password123!"))
                .Callback<ApplicationUser, string>((user, _) =>
                {
                    user.Id = Guid.NewGuid();
                    _context.Users.Add(user);
                    _context.SaveChanges();
                })
                .ReturnsAsync(IdentityResult.Success);
            _userManagerMock
                .Setup(manager => manager.AddToRoleAsync(It.IsAny<ApplicationUser>(), "User"))
                .ReturnsAsync(IdentityResult.Success);

            var result = await _accountService.RegisterAsync(new RegisterViewModel
            {
                UserName = "newuser",
                Email = "new@test.com",
                Password = "Password123!",
                AccountType = AccountType.Personal,
                FirstName = "Ivan",
                LastName = "Ivanov",
                PersonalNumber = "1234567890",
                Address = "Sofia",
                DateOfBirth = new DateTime(1990, 1, 1)
            });

            var profile = await _context.PersonalProfiles.SingleAsync();
            Assert.That(result.Succeeded, Is.True);
            Assert.That(profile.FirstName, Is.EqualTo("Ivan"));
            _userManagerMock.Verify(manager => manager.AddToRoleAsync(It.IsAny<ApplicationUser>(), "User"), Times.Once);
        }

        [Test]
        public async Task RegisterAsync_CreatesCompanyProfile_WhenAccountTypeIsCompany()
        {
            _userManagerMock
                .Setup(manager => manager.CreateAsync(It.IsAny<ApplicationUser>(), "Password123!"))
                .Callback<ApplicationUser, string>((user, _) =>
                {
                    user.Id = Guid.NewGuid();
                    _context.Users.Add(user);
                    _context.SaveChanges();
                })
                .ReturnsAsync(IdentityResult.Success);
            _userManagerMock
                .Setup(manager => manager.AddToRoleAsync(It.IsAny<ApplicationUser>(), "User"))
                .ReturnsAsync(IdentityResult.Success);

            var result = await _accountService.RegisterAsync(new RegisterViewModel
            {
                UserName = "companyuser",
                Email = "company@test.com",
                Password = "Password123!",
                AccountType = AccountType.Company,
                CompanyName = "Acme",
                VATNumber = "BG123",
                CompanyAddress = "Plovdiv",
                ContactPersonName = "Maria",
                ContactPhone = "0888123456"
            });

            var profile = await _context.CompanyProfiles.SingleAsync();
            Assert.That(result.Succeeded, Is.True);
            Assert.That(profile.CompanyName, Is.EqualTo("Acme"));
        }

        [Test]
        public async Task RegisterAsync_ReturnsIdentityFailure_WhenCreateFails()
        {
            _userManagerMock
                .Setup(manager => manager.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Failed(new IdentityError { Description = "Duplicate user" }));

            var result = await _accountService.RegisterAsync(new RegisterViewModel
            {
                UserName = "baduser",
                Email = "bad@test.com",
                Password = "Password123!",
                AccountType = AccountType.Personal
            });

            Assert.That(result.Succeeded, Is.False);
            Assert.That(_context.Users, Is.Empty);
            Assert.That(_context.PersonalProfiles, Is.Empty);
        }

        [Test]
        public async Task GetCurrentUserWithProfilesAsync_ReturnsUserWithProfilesOrNull()
        {
            var user = new ApplicationUser { Id = Guid.NewGuid(), UserName = "ProfileUser", AccountType = AccountType.Personal };
            await _context.Users.AddAsync(user);
            await _context.PersonalProfiles.AddAsync(new PersonalProfile
            {
                UserId = user.Id,
                FirstName = "Test",
                LastName = "User",
                PersonalNumber = "123",
                Adress = "Sofia"
            });
            await _context.SaveChangesAsync();

            var result = await _accountService.GetCurrentUserWithProfilesAsync(user.Id.ToString());
            var invalid = await _accountService.GetCurrentUserWithProfilesAsync("not-a-guid");

            Assert.That(result, Is.Not.Null);
            Assert.That(result!.PersonalProfile, Is.Not.Null);
            Assert.That(invalid, Is.Null);
        }

        [Test]
        public async Task UpdateUserAndProfileAsync_UpdatesPersonalProfileAndUser()
        {
            var user = new ApplicationUser { Id = Guid.NewGuid(), UserName = "Old", Email = "old@test.com", AccountType = AccountType.Personal };
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
            _userManagerMock.Setup(manager => manager.UpdateAsync(user)).ReturnsAsync(IdentityResult.Success);

            var result = await _accountService.UpdateUserAndProfileAsync(user.Id.ToString(), new EditProfileViewModel
            {
                UserName = "New",
                UserEmail = "new@test.com",
                UserPhone = "0888123456",
                FirstName = "New",
                LastName = "Name",
                PersonalNumber = "1234567890",
                Address = "Varna"
            });

            var profile = await _context.PersonalProfiles.SingleAsync(p => p.UserId == user.Id);
            Assert.That(result.Succeeded, Is.True);
            Assert.That(user.UserName, Is.EqualTo("New"));
            Assert.That(profile.Adress, Is.EqualTo("Varna"));
        }

        [Test]
        public async Task UpdateUserAndProfileAsync_UpdatesCompanyProfile()
        {
            var user = new ApplicationUser { Id = Guid.NewGuid(), UserName = "Company", Email = "company@test.com", AccountType = AccountType.Company };
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
            _userManagerMock.Setup(manager => manager.UpdateAsync(user)).ReturnsAsync(IdentityResult.Success);

            var result = await _accountService.UpdateUserAndProfileAsync(user.Id.ToString(), new EditProfileViewModel
            {
                UserName = "CompanyNew",
                UserEmail = "company-new@test.com",
                CompanyName = "Acme",
                VATNumber = "BG999",
                CompanyAddress = "Burgas",
                ContactPersonName = "Nikol",
                ContactPhone = "0888999999"
            });

            var profile = await _context.CompanyProfiles.SingleAsync(p => p.UserId == user.Id);
            Assert.That(result.Succeeded, Is.True);
            Assert.That(profile.CompanyAdress, Is.EqualTo("Burgas"));
        }

        [Test]
        public void UpdateUserAndProfileAsync_Throws_WhenUserDoesNotExist()
        {
            Assert.ThrowsAsync<InvalidOperationException>(() =>
                _accountService.UpdateUserAndProfileAsync(Guid.NewGuid().ToString(), new EditProfileViewModel()));
        }

        [Test]
        public async Task DeleteAsync_AnonymizesPersonalUserAndDeletesProfile()
        {
            var user = new ApplicationUser { Id = Guid.NewGuid(), UserName = "DeleteMe", Email = "delete@test.com", AccountType = AccountType.Personal };
            await _context.Users.AddAsync(user);
            await _context.PersonalProfiles.AddAsync(new PersonalProfile
            {
                UserId = user.Id,
                FirstName = "Delete",
                LastName = "Me",
                PersonalNumber = "123",
                Adress = "Sofia"
            });
            await _context.SaveChangesAsync();
            _userManagerMock.Setup(manager => manager.SetLockoutEnabledAsync(user, true)).ReturnsAsync(IdentityResult.Success);
            _userManagerMock.Setup(manager => manager.UpdateAsync(user)).ReturnsAsync(IdentityResult.Success);

            var result = await _accountService.DeleteAsync(user.Id.ToString());

            Assert.That(result.Succeeded, Is.True);
            Assert.That(user.UserName, Does.StartWith("Deleted_"));
            Assert.That(user.Email, Does.EndWith("@orderflow.com"));
            Assert.That(await _context.PersonalProfiles.AnyAsync(p => p.UserId == user.Id), Is.False);
        }

        [Test]
        public void DeleteAsync_Throws_WhenUserDoesNotExist()
        {
            Assert.ThrowsAsync<InvalidOperationException>(() => _accountService.DeleteAsync(Guid.NewGuid().ToString()));
        }

        private static Mock<UserManager<ApplicationUser>> CreateUserManagerMock()
        {
            var store = new Mock<IUserStore<ApplicationUser>>();
            return new Mock<UserManager<ApplicationUser>>(
                store.Object,
                null!,
                null!,
                Array.Empty<IUserValidator<ApplicationUser>>(),
                Array.Empty<IPasswordValidator<ApplicationUser>>(),
                null!,
                null!,
                null!,
                null!);
        }
    }
}
