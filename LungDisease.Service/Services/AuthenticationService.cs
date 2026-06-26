using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Inventra.Service_Abstraction;

using LungDisease.Domain.IdentityModule;
using LungDisease.Service_Abstraction;
using LungDisease.Shared.Common_Result;
using LungDisease.Shared.DataTransferObjects.IdentityDTOs;
using LungDisease.Shared.DataTransferObjects.PasswordDTOS;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Org.BouncyCastle.Asn1.Ocsp;

namespace LungDisease.Service.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _configuration;
        private readonly IMemoryCache _memoryCache;
        private readonly IEmailService _emailService;

        public AuthenticationService(UserManager<ApplicationUser> userManager,IConfiguration configuration
           , IMemoryCache memoryCache, IEmailService emailService)
        {
            _userManager = userManager;
            _configuration = configuration;
            _memoryCache = memoryCache;
            _emailService = emailService;
        }
        public async Task<Result<UserDTO>> LoginAsync(LoginDTO loginDTO)
        {
            var user = await _userManager.FindByEmailAsync(loginDTO.Email);

            if (user is null)
                return Error.InvalidCredentials("Invalid email or password");

            var isPasswordValid = await _userManager.CheckPasswordAsync(user, loginDTO.Password);

            if (!isPasswordValid)
                return Error.InvalidCredentials("Invalid email or password");

            var token = await CreateTokenAsync(user);

            return new UserDTO(user.Email!, user.DisplayName, token);
        }

        public async Task<Result<UserDTO>> RegisterAsync(RegisterDTO registerDTO)
        {
            var existingUser = await _userManager.FindByEmailAsync(registerDTO.Email);

            if (existingUser is not null)
                return Error.Failure("Email already exists");

            var user = new ApplicationUser
            {
                Email = registerDTO.Email,
                DisplayName = registerDTO.DisplayName,
                UserName = registerDTO.Email,
            };

            var identityResult = await _userManager.CreateAsync(user, registerDTO.Password);

            if (!identityResult.Succeeded)
            {
                var errors = identityResult.Errors
                    .Select(e => Error.Validation(e.Code, e.Description))
                    .ToList();

                return Result<UserDTO>.Fail(errors);
            }

            var token = await CreateTokenAsync(user);

            return new UserDTO(user.Email!, user.DisplayName, token);
        }
        public async Task<Result> SendResetCodeAsync(ForgotPasswordDTO request)
        {
            var email = request.Email?.Trim().ToLower();

            if (string.IsNullOrWhiteSpace(email))
                return Result.Fail(Error.InvalidCredentials("Email.Invalid", "Email is required"));

            if (_memoryCache.TryGetValue($"otp_lock_{email}", out _))
                return Result.Fail(Error.InvalidCredentials("OTP.TooManyRequests", "Try again later"));

            var user = await _userManager.FindByEmailAsync(email);

            if (user is null)
                return Result.Fail(Error.InvalidCredentials("User.NotFound", "Email not found"));

            var code = RandomNumberGenerator.GetInt32(100000, 999999).ToString();

            _memoryCache.Set($"otp_{email}", code, new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(2)
            });

            _memoryCache.Set($"otp_lock_{email}", true, TimeSpan.FromSeconds(30));

            var emailResult = await _emailService.SendAsync(
                email,
                "Reset Password Code",
                $"LungDiseases\n\nYour password reset code is: {code}\n\nThis code will expire in 2 minutes."
            );

            if (emailResult.IsFailure)
                return emailResult;

            return Result.Ok();
        }

        public Task<Result> VerifyResetCodeAsync(VerifyCodeDTO verifyCode)
        {
            if (verifyCode.Code.Length != 6 || !verifyCode.Code.All(char.IsDigit))
            {
                return Task.FromResult(
                   Result.Fail(Error.Validation("Code.InvalidFormat", "Code must be 6 digits")
                ));
            }

            if (!_memoryCache.TryGetValue($"otp_{verifyCode.Email}", out string storedCode))
            {
                return Task.FromResult(
                   Result.Fail(Error.Validation("Code.Expired", "Code expired")
                ));
            }

            if (storedCode != verifyCode.Code)
            {
                return Task.FromResult(
                   Result.Fail(Error.InvalidCredentials("Code.Invalid", "Wrong code")
                ));
            }

            // ✔ mark as verified
            _memoryCache.Set($"verified_{verifyCode.Email}", true, TimeSpan.FromMinutes(10));

            return Task.FromResult(Result.Ok());
        }

        public async Task<Result> ResetPasswordAsync(ResetPasswordDTO resetPassword)
        {
            // ✔ لازم يكون verified
            if (!_memoryCache.TryGetValue($"verified_{resetPassword.Email}", out bool isVerified) || !isVerified)
            {
                return Result.Fail(Error.InvalidCredentials("Code.NotVerified", "Code not verified"));
            }

            var user = await _userManager.FindByEmailAsync(resetPassword.Email);

            if (user is null)
                return Result.Fail(Error.InvalidCredentials("User.NotFound", "User not found"));

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);

            var result = await _userManager.ResetPasswordAsync(
                user,
                token,
                resetPassword.NewPassword
            );

            if (result.Succeeded)
            {

                _memoryCache.Remove($"otp_{resetPassword.Email}");
                _memoryCache.Remove($"verified_{resetPassword.Email}");

                return Result.Ok();
            }

            return Result.Fail(result.Errors
                .Select(e => Error.Validation(e.Code, e.Description))
                .ToList());
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
