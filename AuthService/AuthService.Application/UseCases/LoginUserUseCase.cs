using AuthService.Application.DTOs;
using AuthService.Application.Interfaces;
using AuthService.Domain.Interfaces;
using SharedKernel.ValueObjects;

namespace AuthService.Application.UseCases;

public class LoginUserUseCase
{
    private readonly IUserRepository _userRepository;
    private readonly ITokenService _tokenService;

    public LoginUserUseCase(
        IUserRepository userRepository,
        ITokenService tokenService)
    {
        _userRepository = userRepository;
        _tokenService = tokenService;
    }

    public async Task<AuthResponse> ExecuteAsync(LoginRequest request)
    {
        var normalizedEmail = Email.Create(request.Email).Value;

        var user = await _userRepository.GetByEmailAsync(normalizedEmail);
        if (user is null)
            throw new Exception("Invalid credentials");
        if (!user.IsActive)
            throw new UnauthorizedAccessException("User is inactive");

        user.CheckAndUnlockIfExpired();
        if (user.IsLocked)
            throw new UnauthorizedAccessException("User account is locked");
        var isPasswordValid = BCrypt.Net.BCrypt.Verify(
            request.Password,
            user.PasswordHash
        );

        if (!isPasswordValid)
        {
            user.RegisterFailedLogin();
            await _userRepository.UpdateAsync(user);
            throw new Exception("Invalid credentials");
        }
        user.ResetFailedLoginAttempts();
        await _userRepository.UpdateAsync(user);
        var token = _tokenService.GenerateToken(user.Id, user.Email.Value, user.Role.ToString());

        return new AuthResponse
        {
            Token = token
        };
    }
}
