using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using LungDisease.Service_Abstraction;
using LungDisease.Shared.DataTransferObjects.IdentityDTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace Presentation.Controllers
{
    [Authorize]
    public class UserController : ApiBaseController
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }
        [HttpGet("me")]
        public async Task<ActionResult<UserProfileDto>> GetCurrentUser()
        {

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
                return Unauthorized("Invalid token");

            var result = await _userService.GetCurrentUserAsync(userId);

            return HandleResult(result);
        }
        [HttpPut("profileUpdate")]
        public async Task<IActionResult> UpdateProfile(UpdateProfileDto model)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var result = await _userService.UpdateProfileAsync(userId!, model);

            if (!result.IsSuccess)
                return BadRequest(result.IsFailure);

            return Ok("Profile updated successfully");
        }
    }
}
