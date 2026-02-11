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
    public class AuthenticationService : IAuthenticationService
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public AuthenticationService(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }
        public async Task<Result<UserDTO>> LoginAsync(LoginDTO loginDTO)
        {
            var User = await _userManager.FindByEmailAsync(loginDTO.Email);
            if (User is null)
                return Error.InvalidCredentials("User.InvalidCredentials");

            var IsPasswordValid = await _userManager.CheckPasswordAsync(User, loginDTO.Password);

            if (!IsPasswordValid)
                Error.InvalidCredentials("User.InvalidCredentials");
            return new UserDTO(User.Email!, User.DisplayName, "Token");

        }

        public async Task<Result<UserDTO>> RegisterAsync(RegisterDTO registerDTO)
        {
            var User = new ApplicationUser()
            {
                Email = registerDTO.Email,
                DisplayName = registerDTO.DisplayName,
                UserName = registerDTO.UserName,
            };
            var IdentityResult = await _userManager.CreateAsync(User, registerDTO.Password);

            if (IdentityResult.Succeeded)
                return new UserDTO(User.Email!, User.DisplayName, "Token");

            return IdentityResult.Errors.Select(E => Error.Validation(E.Code, E.Description)).ToList();


        }
    }
}
