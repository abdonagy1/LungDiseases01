using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using LungDisease.Domain.IdentityModule;
using LungDisease.Service_Abstraction;
using LungDisease.Shared.Common_Result;
using LungDisease.Shared.DataTransferObjects.IdentityDTOs;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace LungDisease.Service.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _configuration;

        public AuthenticationService(UserManager<ApplicationUser> userManager,IConfiguration configuration)
        {
            _userManager = userManager;
            _configuration = configuration;
        }
        public async Task<Result<UserDTO>> LoginAsync(LoginDTO loginDTO)
        {
            var User = await _userManager.FindByEmailAsync(loginDTO.Email);
            if (User is null)
                return Error.InvalidCredentials("User.InvalidCredentials");

            var IsPasswordValid = await _userManager.CheckPasswordAsync(User, loginDTO.Password);

            if (!IsPasswordValid)
                Error.InvalidCredentials("User.InvalidCredentials");
            var Token = await CreateTokenAsync(User);
            return new UserDTO(User.Email!, User.DisplayName, Token);

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
            { 
                var Token = await CreateTokenAsync(User);
            return new UserDTO(User.Email!, User.DisplayName, Token); 
            }

            return IdentityResult.Errors.Select(E => Error.Validation(E.Code, E.Description)).ToList();


        }

        private async Task<string> CreateTokenAsync(ApplicationUser user)
        {
            var Claims = new List<Claim>()
            {
                new Claim ( JwtRegisteredClaimNames.Email , user. Email!),
                new Claim( JwtRegisteredClaimNames.Name,user.UserName!),
                new Claim(JwtRegisteredClaimNames.Sub, user.Id)
            };


            var SecretKey = _configuration["JWTOptions:SecretKey"];
            var Key = new SymmetricSecurityKey(key: Encoding.UTF8.GetBytes(SecretKey));
            var Cred = new SigningCredentials(key: Key, algorithm: SecurityAlgorithms.HmacSha256);

            var Token = new JwtSecurityToken(
                issuer: _configuration[key: "JWTOptions:Issuer"],
                audience: _configuration[key: "JWTOptions:Audience"],
                expires: DateTime.UtcNow.AddHours(1),
                claims: Claims,
                signingCredentials: Cred);

            return new JwtSecurityTokenHandler().WriteToken(Token);

        }


    }
}
