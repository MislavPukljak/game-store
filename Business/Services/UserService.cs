using Business.DTO.Authorization;
using Business.Exceptions;
using Business.Interfaces;
using Data.SQL.Entities;
using Data.SQL.Enums;
using Data.SQL.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace Business.Services;

public class UserService : IUserService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly UserManager<User> _userManager;
    private readonly ILogger<UserService> _logger;
    private readonly IUserServiceClient _userServiceClient;

    public UserService(IUnitOfWork unitOfWork, UserManager<User> userManager, ILogger<UserService> logger, IUserServiceClient userServiceClient)
    {
        _unitOfWork = unitOfWork;
        _userManager = userManager;
        _logger = logger;
        _userServiceClient = userServiceClient;
    }

    public async Task<List<UserDto>> GetAllAsync()
    {
        var users = await _userServiceClient.GetUsersAsync();

        return users;
    }

    public async Task DeleteUserSqlAsync(string email)
    {
        await _userServiceClient.DeleteUserAsync(email);
        var user = await _userManager.FindByEmailAsync(email);

        if (user is null)
        {
            _logger.LogError($"User with email {email} not found");

            throw new UserException($"User with email {email} not found");
        }

        await _userManager.DeleteAsync(user);

        await _unitOfWork.SaveAsync();
    }

    public async Task UpdateUserSqlAsync(string originalEmail, UserChangeDto userChangeDto)
    {
        await _userServiceClient.PutUserAsync(originalEmail, userChangeDto);
        var user = await _userManager.FindByEmailAsync(originalEmail);

        if (user is null)
        {
            _logger.LogError($"User with email {userChangeDto.Email} not found");

            throw new UserException($"User with email {userChangeDto.Email} not found");
        }

        user.UserName = userChangeDto.FirstName;
        user.LastName = userChangeDto.LastName;
        user.Name = userChangeDto.FirstName;
        user.Email = userChangeDto.Email;
        user.PasswordHash = _userManager.PasswordHasher.HashPassword(user, userChangeDto.Password);

        await _userManager.UpdateAsync(user);

        await _unitOfWork.SaveAsync();
    }

    public async Task<AuthServiceResponseDto> RegisterAsync(UserChangeDto userDto)
    {
        await _userServiceClient.PostUserAsync(userDto);
        var userExists = await _userManager.FindByNameAsync(userDto.FirstName);
        if (userExists != null)
        {
            return new AuthServiceResponseDto()
            {
                IsSucceed = false,
                Message = "Invalid Credentials!",
            };
        }

        var user = new User()
        {
            UserName = userDto.FirstName,
            LastName = userDto.LastName,
            Email = userDto.Email,
            SecurityStamp = Guid.NewGuid().ToString(),
            Name = userDto.FirstName,
            Roles = new List<string> { UsersRoles.Roles.User.ToString() },
        };

        var createdUserResult = await _userManager.CreateAsync(user, userDto.Password);

        if (!createdUserResult.Succeeded)
        {
            return new AuthServiceResponseDto()
            {
                IsSucceed = false,
                Message = "User creation failed! Please check user details and try again.",
            };
        }

        await _userManager.AddToRoleAsync(user, UsersRoles.Roles.User.ToString());

        return new AuthServiceResponseDto()
        {
            IsSucceed = true,
            Message = "User created successfully!",
        };
    }

    public async Task<AuthServiceResponseDto> ManageRoles(UpdatePermissionDto updatePermissionDto, UsersRoles.Roles role)
    {
        var user = await _userManager.FindByEmailAsync(updatePermissionDto.Email);

        if (user is null)
        {
            return new AuthServiceResponseDto()
            {
                IsSucceed = false,
                Message = "Invalid email",
            };
        }

        await UpdateUserRoleAsync(user, role);

        user.Roles.Add(role.ToString());

        await _userManager.UpdateAsync(user);

        return new AuthServiceResponseDto()
        {
            IsSucceed = true,
            Message = "User role updated successfully!",
        };
    }

    private async Task UpdateUserRoleAsync(User user, UsersRoles.Roles role)
    {
        switch (role)
        {
            case UsersRoles.Roles.Administrator:
                await _userManager.AddToRoleAsync(user, UsersRoles.Roles.Administrator.ToString());
                break;
            case UsersRoles.Roles.Manager:
                await _userManager.AddToRoleAsync(user, UsersRoles.Roles.Manager.ToString());
                break;
            case UsersRoles.Roles.Moderator:
                await _userManager.AddToRoleAsync(user, UsersRoles.Roles.Moderator.ToString());
                break;
            case UsersRoles.Roles.Publisher:
                await _userManager.AddToRoleAsync(user, UsersRoles.Roles.Publisher.ToString());
                break;
            case UsersRoles.Roles.User:
                await _userManager.AddToRoleAsync(user, UsersRoles.Roles.User.ToString());
                break;
            default:
                await _userManager.AddToRoleAsync(user, UsersRoles.Roles.User.ToString());
                break;
        }

        await _userManager.AddToRoleAsync(user, role.ToString());
    }
}
