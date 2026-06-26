using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LungDisease.Shared.Common_Result;
using LungDisease.Shared.DataTransferObjects.IdentityDTOs;
using LungDisease.Shared.DataTransferObjects.PasswordDTOS;

namespace LungDisease.Service_Abstraction
{
    public interface IAuthenticationService
    {
        Task<Result<UserDTO>> LoginAsync(LoginDTO loginDTO);
        Task<Result<UserDTO>> RegisterAsync(RegisterDTO registerDTO);

        Task<Result> SendResetCodeAsync(ForgotPasswordDTO request);
        Task<Result> VerifyResetCodeAsync(VerifyCodeDTO verifyCode);
        Task<Result> ResetPasswordAsync(ResetPasswordDTO resetPassword);
    }
}
