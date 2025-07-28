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

        [HttpGet("get-all-users")]
        public async Task<IResult> GetAllUsers()
        {
            string? refreshToken = HttpContext.Request.Cookies["REFRESH_TOKEN"];

            return Results.Ok(await _userService.GetAllUsersAsync(refreshToken!));
        }

        [HttpDelete("delete-user")]
        public async Task<IResult> DeleteUser(UserDeleteRequest request)
        {
            string? refreshToken = HttpContext.Request.Cookies["REFRESH_TOKEN"];

            await _userService.DeleteUserAsync(request.Id, refreshToken!);

            return Results.Ok();
        }

        [HttpPut("block-user")]
        public async Task<IResult> BlockUser(UserBlockRequest request)
        {
            string? refreshToken = HttpContext.Request.Cookies["REFRESH_TOKEN"];

            await _userService.BlockUserAsync(request.Id, refreshToken!);

            return Results.Ok();
        }

        [HttpPut("unblock-user")]
        public async Task<IResult> UnblockUser(UserUnblockRequest request)
        {
            string? refreshToken = HttpContext.Request.Cookies["REFRESH_TOKEN"];

            await _userService.UnblockUserAsync(request.Id, refreshToken!);

            return Results.Ok();
        }
    }
}
