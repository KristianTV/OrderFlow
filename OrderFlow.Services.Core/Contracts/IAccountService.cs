using Microsoft.AspNetCore.Identity;
using OrderFlow.Data.Models;
using OrderFlow.Data.Repository.Contracts;
using OrderFlow.ViewModels.User;

namespace OrderFlow.Services.Core.Contracts
{
    public interface IAccountService : IRepository
    {
        IQueryable<ApplicationUser> GetAll();
        Task<IdentityResult> RegisterAsync(RegisterViewModel model);
        Task<ApplicationUser?> GetCurrentUserWithProfilesAsync(string? userId);
        Task<IdentityResult> UpdateUserAndProfileAsync(string? userId, EditProfileViewModel model);
        Task<IdentityResult> DeleteAsync(string? userId);
    }
}
