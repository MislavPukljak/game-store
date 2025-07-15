using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Business.DTO.Authorization;
using Business.Interfaces;
using Data.SQL.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Business.Services;

public class AuthorizationService : IAuthService
{
    private readonly UserManager<User> _userManager;
    private readonly IConfiguration _configuration;
    private readonly IUserServiceClient _userServiceClient;

    public AuthorizationService(UserManager<User> userManager, IConfiguration configuration, IUserServiceClient userServiceClient)
    {
        _userManager = userManager;
        _configuration = configuration;
        _userServiceClient = userServiceClient;
    }

    public async Task<AuthServiceResponseDto> LoginAsync(AuthDto authDto)
    {
        await _userServiceClient.PostAuthAsync(authDto);

        var user = await _userManager.FindByEmailAsync(authDto.Email);
        if (user is null)
        {
            return new AuthServiceResponseDto()
            {
                IsSucceed = false,
                Message = "Wrong email",
            };
        }

        var isPasswordCorrect = await _userManager.CheckPasswordAsync(user, authDto.Password);

        if (!isPasswordCorrect)
        {
            return new AuthServiceResponseDto()
            {
                IsSucceed = false,
                Message = "Incorrect password",
            };
        }

        var userRoles = await _userManager.GetRolesAsync(user);

#pragma warning disable CS8604 // Possible null reference argument.
        var authClaims = new List<Claim>
            {
                new(ClaimTypes.Name, user.UserName),
                new(ClaimTypes.NameIdentifier, user.Id),
                new("JWTID", Guid.NewGuid().ToString()),
                new("UserName", user.UserName),
                new("Email", user.Email),
            };
#pragma warning restore CS8604 // Possible null reference argument.

        foreach (var userRole in userRoles)
        {
            authClaims.Add(new Claim(ClaimTypes.Role, userRole));
        }

        var token = GenerateNewJsonWebToken(authClaims);

        return new AuthServiceResponseDto()
        {
            IsSucceed = true,
            Message = token,
        };
    }

    private string GenerateNewJsonWebToken(List<Claim> claims)
    {
        var authSecret = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetSection("JWT:Secret").Value!));

        var tokenObject = new JwtSecurityToken(
                issuer: _configuration["JWT:ValidIssuer"],
                audience: _configuration["JWT:ValidAudience"],
                expires: DateTime.Now.AddHours(1),
                claims: claims,
                signingCredentials: new SigningCredentials(authSecret, SecurityAlgorithms.HmacSha256));

        string token = new JwtSecurityTokenHandler().WriteToken(tokenObject);

        return token;
    }
}
