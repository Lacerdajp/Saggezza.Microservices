using AuthService.Application.DTOs;
using AuthService.Application.Security;
using AuthService.Domain.Entities;
using AuthService.Domain.Enums;
using AuthService.Domain.Interfaces;

namespace AuthService.Application.UseCases;

public class RegisterUserUseCase
{
    private readonly IUserRepository _userRepository;

    public RegisterUserUseCase(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task ExecuteAsync(RegisterUserRequest request)
    {
        var existingUser = await _userRepository.GetByEmailAsync(request.Email);

        if (existingUser is not null)
            throw new Exception("User already exists");

        PasswordPolicy.EnsureStrong(request.Password);

        var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

        var role = ParseRoleOrDefault(request.Role);

        var user = new User(
            request.FullName,
            request.Email,
            passwordHash,
            role
        );

        await _userRepository.AddAsync(user);
    }

    private static UserRole ParseRoleOrDefault(string? role)
    {
        if (string.IsNullOrWhiteSpace(role))
            return UserRole.User;

        return Enum.TryParse<UserRole>(role.Trim(), ignoreCase: true, out var parsed)
            ? parsed
            : UserRole.User;
    }
}
