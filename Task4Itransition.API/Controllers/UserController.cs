using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Task4Itransition.Application.Abstracts.Services;
using Task4Itransition.Application.DTO.User;

namespace Task4Itransition.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UserController(IUserService userService) : ControllerBase
    {
        private readonly IUserService _userService = userService;

        [HttpGet("me")]
        public async Task<IResult> GetUser()
        {
            return Results.Ok(await _userService.GetUserAsync(User.FindFirstValue(ClaimTypes.NameIdentifier)!));
        }

        [HttpGet("get-all-users")]
        public async Task<IResult> GetAllUsers()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            return Results.Ok(await _userService.GetAllUsersAsync(userId!));
        }

        [HttpDelete("delete-user")]
        public async Task<IResult> DeleteUser(UserDeleteRequest request)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            await _userService.DeleteUserAsync(request.Id, userId!);

            return Results.Ok();
        }

        [HttpPut("block-user")]
        public async Task<IResult> BlockUser(UserBlockRequest request)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            await _userService.BlockUserAsync(request.Id, userId!);

            return Results.Ok();
        }

        [HttpPut("unblock-user")]
        public async Task<IResult> UnblockUser(UserUnblockRequest request)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            await _userService.UnblockUserAsync(request.Id, userId!);

            return Results.Ok();
        }
    }
}
