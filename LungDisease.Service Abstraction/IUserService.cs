using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LungDisease.Shared.Common_Result;
using LungDisease.Shared.DataTransferObjects.IdentityDTOs;

namespace LungDisease.Service_Abstraction
{
    public interface IUserService
    {
        Task<Result<UserProfileDto>> GetCurrentUserAsync(string userId);
        Task<Result> UpdateProfileAsync(string userId, UpdateProfileDto updateProfile);
    }
}
