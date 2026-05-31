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
                Id = User.Id,
                UserName = User.UserName!,
                Email = User.Email!
            };
            return Result< UserProfileDto>.Ok(UserDto);
        }
    }
}
