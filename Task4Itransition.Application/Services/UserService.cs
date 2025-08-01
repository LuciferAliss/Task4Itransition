using System;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Task4Itransition.Application.Abstracts.Services;
using Task4Itransition.Application.DTO.User;
using Task4Itransition.Application.Exceptions;
using Task4Itransition.Domain.Abstracts.Repository;
using Task4Itransition.Domain.Entities;

namespace Task4Itransition.Application.Services;

public class UserService(UserManager<User> userManager, IUserRepository userRepository, IMapper mapper) : IUserService
{
    private readonly UserManager<User> _userManager = userManager;
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IMapper _mapper = mapper;

    public async Task<UserDto> GetUserAsync(string id)
    {
        var user = await _userManager.FindByIdAsync(id.ToString()) ?? throw new UserNotFoundException("User not found.");
        await CheckBlockUserAsync(id);

        return _mapper.Map<UserDto>(user);
    }

    public async Task BlockUserAsync(Guid id, string currentUserId)
    {
        var user = await _userManager.FindByIdAsync(id.ToString()) ?? throw new UserNotFoundException("User not found.");
        await CheckBlockUserAsync(currentUserId);

        var result = await _userManager.SetLockoutEndDateAsync(user, DateTime.UtcNow.AddYears(1000));

        if (!result.Succeeded)
        {
            throw new UserBlockFailedException("Failed to block the user.");
        }
    }

    public async Task UnblockUserAsync(Guid id, string currentUserId)
    {
        var user = await _userManager.FindByIdAsync(id.ToString()) ?? throw new UserNotFoundException("User not found.");
        await CheckBlockUserAsync(currentUserId);

        var result = await _userManager.SetLockoutEndDateAsync(user, DateTimeOffset.UtcNow);

        if (!result.Succeeded)
        {
            throw new UserUnblockFailedException("Failed to unblock the user.");
        }
    }

    public async Task DeleteUserAsync(Guid id, string currentUserId)
    {
        var user = await _userManager.FindByIdAsync(id.ToString()) ?? throw new UserNotFoundException("User not found.");
        await CheckBlockUserAsync(currentUserId);

        await _userManager.DeleteAsync(await _userManager.FindByIdAsync(id.ToString()) ?? throw new UserNotFoundException("User not found."));
    }

    public async Task<IEnumerable<UserDto>> GetAllUsersAsync(string currentUserId)
    {
        await CheckBlockUserAsync(currentUserId);

        return _mapper.Map<IEnumerable<UserDto>>(await _userManager.Users.ToListAsync() ?? throw new UserNotFoundException("Users not found."));
    }

    public async Task CheckBlockUserAsync(string id)
    {
        var user = await _userManager.FindByIdAsync(id) ?? throw new UserNotFoundException("User not found.");
        if (await _userManager.IsLockedOutAsync(user) && user.LockoutEnd > DateTime.UtcNow)
        {
            throw new UserBlockException("User is blocked.");
        }
    }
}
