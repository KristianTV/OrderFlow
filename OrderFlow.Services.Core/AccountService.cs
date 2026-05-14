using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OrderFlow.Data;
using OrderFlow.Data.Models;
using OrderFlow.Data.Models.Enums;
using OrderFlow.Data.Repository;
using OrderFlow.Services.Core.Contracts;
using OrderFlow.ViewModels.User;

namespace OrderFlow.Services.Core
{
    public class AccountService : BaseRepository, IAccountService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        public AccountService(OrderFlowDbContext _context,
                              UserManager<ApplicationUser> userManager) : base(_context)
        {
            _userManager = userManager;
        }

        public IQueryable<ApplicationUser> GetAll()
        {
            return this.All<ApplicationUser>();
        }

        private IQueryable<T> GetProfile<T>() where T : class
        {
            return this.All<T>();
        }

        public async Task<IdentityResult> RegisterAsync(RegisterViewModel model)
        {
            ApplicationUser user = new ApplicationUser
            {
                Email = model.Email,
                UserName = model.UserName,
                EmailConfirmed = true,
                AccountType = model.AccountType
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, "User");
                var profileCreated = await CreateProfileAsync(user.Id, model);

                if (!profileCreated)
                {
                    await _userManager.DeleteAsync(user);
                    return IdentityResult.Failed(new IdentityError { Description = "Failed to create user profile." });
                }

                return result;
            }

            return result;
        }

        private async Task<bool> CreateProfileAsync(Guid userId, RegisterViewModel model)
        {
            if (model.AccountType == AccountType.Personal)
            {
                await this.AddAsync(new PersonalProfile
                {
                    UserId = userId,
                    FirstName = model.FirstName!,
                    LastName = model.LastName!,
                    PersonalNumber = model.PersonalNumber!,
                    Adress = model.Address!,
                    DateOfBirth = model.DateOfBirth
                });
            }
            else
            {
                await this.AddAsync(new CompanyProfile
                {
                    UserId = userId,
                    CompanyName = model.CompanyName!,
                    VATNumber = model.VATNumber!,
                    CompanyAdress = model.CompanyAddress!,
                    ContactPersonName = model.ContactPersonName!,
                    ContactPhone = model.ContactPhone!
                });
            }

            var result = await this.SaveChangesAsync();

            return result > 0;
        }

        public async Task<ApplicationUser?> GetCurrentUserWithProfilesAsync(string? userId)
        {
            if (string.IsNullOrWhiteSpace(userId) || !Guid.TryParse(userId, out var userGuid))
            {
                return null;
            }

            return await this.GetAll()
                             .AsNoTracking()
                             .Include(u => u.PersonalProfile)
                             .Include(u => u.CompanyProfile)
                             .FirstOrDefaultAsync(u => u.Id == userGuid);
        }

        public async Task<IdentityResult> UpdateUserAndProfileAsync(string? userId, EditProfileViewModel model)
        {
            ApplicationUser user = await this.GetAll().FirstOrDefaultAsync(u => u.Id.ToString() == userId) ?? throw new InvalidOperationException("User not found.");

            user.UserName = model.UserName;
            user.Email = model.UserEmail;
            user.PhoneNumber = model.UserPhone;

            if (user.AccountType == AccountType.Personal)
            {
                PersonalProfile personalProfile = await this.GetProfile<PersonalProfile>().FirstOrDefaultAsync(p => p.UserId == user.Id) ?? new PersonalProfile { UserId = user.Id };

                personalProfile.FirstName = model.FirstName!;
                personalProfile.LastName = model.LastName!;
                personalProfile.PersonalNumber = model.PersonalNumber!;
                personalProfile.Adress = model.Address!;
                personalProfile.DateOfBirth = model.DateOfBirth;
            }
            else
            {
                CompanyProfile companyProfile = await this.GetProfile<CompanyProfile>().FirstOrDefaultAsync(c => c.UserId == user.Id) ?? new CompanyProfile { UserId = user.Id };

                companyProfile.CompanyName = model.CompanyName!;
                companyProfile.VATNumber = model.VATNumber!;
                companyProfile.CompanyAdress = model.CompanyAddress!;
                companyProfile.ContactPersonName = model.ContactPersonName!;
                companyProfile.ContactPhone = model.ContactPhone!;
            }

            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded)
            {
                await this.SaveChangesAsync();
            }

            return result;
        }

        public async Task<IdentityResult> DeleteAsync(string? userId)
        {
            ApplicationUser user = await this.GetAll().FirstOrDefaultAsync(u => u.Id.ToString() == userId) ?? throw new InvalidOperationException("User not found.");

            if (user.AccountType == AccountType.Personal)
            {
                PersonalProfile personalProfile = await this.GetProfile<PersonalProfile>().FirstOrDefaultAsync(p => p.UserId == user.Id) ?? throw new InvalidOperationException("User profile not found.");

                this.Delete(personalProfile);
            }
            else
            {
                CompanyProfile companyProfile = await this.GetProfile<CompanyProfile>().FirstOrDefaultAsync(c => c.UserId == user.Id) ?? throw new InvalidOperationException("User profile not found."); ;

                this.Delete(companyProfile);
            }

            var result = await _userManager.DeleteAsync(user);

            if (result.Succeeded)
            {
                var saveResult = await this.SaveChangesAsync();
                if (saveResult <= 0)
                {
                    return IdentityResult.Failed(new IdentityError { Description = "Failed to delete user profile." });
                }
            }
            return result;
        }
    }
}
