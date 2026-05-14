using OrderFlow.Data.Models;
using OrderFlow.ViewModels.User;

namespace OrderFlow.Services.Core.Extensions
{
    public static class UserProfileExtentions
    {
        public static ProfileViewModel MapProfileViewModel(this ApplicationUser user)
        {
            return new ProfileViewModel
            {
                UserName = user.UserName ?? string.Empty,
                UserEmail = user.Email ?? string.Empty,
                UserPhone = user.PhoneNumber ?? string.Empty,
                AccountType = user.AccountType,
                FirstName = user.PersonalProfile?.FirstName ?? string.Empty,
                LastName = user.PersonalProfile?.LastName ?? string.Empty,
                PersonalNumber = user.PersonalProfile?.PersonalNumber ?? string.Empty,
                Address = user.PersonalProfile?.Adress ?? string.Empty,
                DateOfBirth = user.PersonalProfile?.DateOfBirth,
                CompanyName = user.CompanyProfile?.CompanyName ?? string.Empty,
                VATNumber = user.CompanyProfile?.VATNumber ?? string.Empty,
                CompanyAddress = user.CompanyProfile?.CompanyAdress ?? string.Empty,
                ContactPersonName = user.CompanyProfile?.ContactPersonName ?? string.Empty,
                ContactPhone = user.CompanyProfile?.ContactPhone ?? string.Empty
            };
        }

        public static EditProfileViewModel MapEditProfileViewModel(this ApplicationUser user)
        {
            return new EditProfileViewModel
            {
                UserName = user.UserName ?? string.Empty,
                UserEmail = user.Email ?? string.Empty,
                UserPhone = user.PhoneNumber ?? string.Empty,
                AccountType = user.AccountType,
                FirstName = user.PersonalProfile?.FirstName ?? string.Empty,
                LastName = user.PersonalProfile?.LastName ?? string.Empty,
                PersonalNumber = user.PersonalProfile?.PersonalNumber ?? string.Empty,
                Address = user.PersonalProfile?.Adress ?? string.Empty,
                DateOfBirth = user.PersonalProfile?.DateOfBirth,
                CompanyName = user.CompanyProfile?.CompanyName ?? string.Empty,
                VATNumber = user.CompanyProfile?.VATNumber ?? string.Empty,
                CompanyAddress = user.CompanyProfile?.CompanyAdress ?? string.Empty,
                ContactPersonName = user.CompanyProfile?.ContactPersonName ?? string.Empty,
                ContactPhone = user.CompanyProfile?.ContactPhone ?? string.Empty
            };
        }
    }
}
