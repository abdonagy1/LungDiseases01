using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LungDisease.Service_Abstraction;
using LungDisease.Shared.DataTransferObjects.IdentityDTOs;
using LungDisease.Shared.DataTransferObjects.PasswordDTOS;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers
{
    public class AuthenticationController:ApiBaseController
    {
        private readonly IAuthenticationService _authenticationService;

        public AuthenticationController(IAuthenticationService authenticationService)
        {
            _authenticationService = authenticationService;
        }

        // Login
        // POST: baseUrl/api/Authentication/Login

        [HttpPost("Login")]
        public async Task<ActionResult<UserDTO>> Login(LoginDTO loginDTO)
        {
            var Result = await _authenticationService.LoginAsync(loginDTO);
            return HandleResult(Result);
        }

        // Register
        // POST: BaseUrl/api/Authentication/Register
        [HttpPost("Register")]
        public async Task<ActionResult<UserDTO>> Register(RegisterDTO registerDTO)
        {
            var Result = await _authenticationService.RegisterAsync(registerDTO);
            return HandleResult(Result);
        }

        // POST: baseUrl/api/Authentication/SendCode
        [HttpPost("SendCode")]
        public async Task<IActionResult> SendCode(ForgotPasswordDTO email)
        {
            var result = await _authenticationService.SendResetCodeAsync(email);
            return HandleResult(result);
        }


        // POST: baseUrl/api/Authentication/VerifyCode
        [HttpPost("VerifyCode")]
        public async Task<IActionResult> VerifyPassword(VerifyCodeDTO dto)
        {
            var result = await _authenticationService.VerifyResetCodeAsync(dto);
            if (result.IsFailure)
            {
                return HandleResult(result);
            }
            return Ok(new
            {
                verified = true
            });
        }


        // POST: baseUrl/api/Authentication/ResetPassword
        [HttpPost("ResetPassword")]
        public async Task<IActionResult> ResetPassword(ResetPasswordDTO dto)
        {
            var result = await _authenticationService.ResetPasswordAsync(dto);

            return HandleResult(result);

        }


    }
}
