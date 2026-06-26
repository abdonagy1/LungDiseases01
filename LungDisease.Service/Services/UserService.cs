using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LungDisease.Domain.IdentityModule;
using LungDisease.Service_Abstraction;
using LungDisease.Shared.Common_Result;
using LungDisease.Shared.DataTransferObjects.IdentityDTOs;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace LungDisease.Service.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public UserService(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }
        public async Task<Result<UserProfileDto>> GetCurrentUserAsync(string userId)
        {
           var User =await _userManager.FindByIdAsync(userId);

            if (User is null)
               return Error.NotFound($"{User}.not found");
            var UserDto = new UserProfileDto()
            {
                Email = User.Email!,
                DisplayName = User.DisplayName,
            };
            return Result< UserProfileDto>.Ok(UserDto);
        }

        public async Task<Result> UpdateProfileAsync(string userId, UpdateProfileDto updateProfile)
        {
            var User=await _userManager.FindByIdAsync(userId);

            if (User is null)
                return Result.Fail(Error.NotFound($"{userId} Not Found"));

            //if (!string.IsNullOrWhiteSpace(updateProfile.UserName)) 
            //    {
            //        User.UserName = updateProfile.UserName;
            //    }
            
            if (!string.IsNullOrWhiteSpace(updateProfile.DisplayName)) 
                {
                    var existingDisplayName = await _userManager.Users.FirstOrDefaultAsync(x => x.DisplayName == updateProfile.DisplayName);

                    if (existingDisplayName is not null && existingDisplayName.Id != userId)
                        return Result.Fail(Error.Failure("DisplayName is already taken"));

                    User.DisplayName = updateProfile.DisplayName;

                }
            
            if (!string.IsNullOrWhiteSpace(updateProfile.Email))
            {
                var existingUser = await _userManager.FindByEmailAsync(updateProfile.Email);

                if (existingUser is not null && existingUser.Id != userId)
                    return Result.Fail(Error.Failure("Email already exists"));

                User.Email = updateProfile.Email;
                User.NormalizedEmail = updateProfile.Email.ToUpper();
            }

            var result = await _userManager.UpdateAsync(User);
            if (!result.Succeeded)
            {
                return Result.Fail(Error.Failure(
                    string.Join(", ", result.Errors.Select(e => e.Description))));
            }

            return Result.Ok();

        }
    }
}
