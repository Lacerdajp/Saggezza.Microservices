using AuthService.Application.DTOs;
using AuthService.Application.Interfaces;
using AuthService.Application.UseCases;
using AuthService.Domain.Entities;
using AuthService.Domain.Enums;
using AuthService.Domain.Interfaces;
using FluentAssertions;
using Moq;

namespace AuthService.Tests.UseCases;

public class LoginUserUseCaseTests
{
    [Fact]
    public async Task ExecuteAsync_WhenUserNotFound_ShouldThrowInvalidCredentials()
    {
        var repo = new Mock<IUserRepository>();
        repo.Setup(r => r.GetByEmailAsync(It.IsAny<string>()))
            .ReturnsAsync((User?)null);

        var tokenService = new Mock<ITokenService>();
        var sut = new LoginUserUseCase(repo.Object, tokenService.Object);

        var act = () => sut.ExecuteAsync(new LoginRequest { Email = "john@test.com", Password = "Aa1!aaaa" });

        await act.Should().ThrowAsync<Exception>().WithMessage("Invalid credentials");
        repo.Verify(r => r.UpdateAsync(It.IsAny<User>()), Times.Never);
        tokenService.Verify(t => t.GenerateToken(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task ExecuteAsync_WhenUserInactive_ShouldThrowUnauthorized()
    {
        var passwordHash = BCrypt.Net.BCrypt.HashPassword("Aa1!aaaa");
        var user = new User("John", "john@test.com", passwordHash, UserRole.User);
        user.Deactivate();

        var repo = new Mock<IUserRepository>();
        repo.Setup(r => r.GetByEmailAsync(It.IsAny<string>()))
            .ReturnsAsync(user);

        var tokenService = new Mock<ITokenService>();
        var sut = new LoginUserUseCase(repo.Object, tokenService.Object);

        var act = () => sut.ExecuteAsync(new LoginRequest { Email = "john@test.com", Password = "Aa1!aaaa" });

        await act.Should().ThrowAsync<UnauthorizedAccessException>().WithMessage("User is inactive");
        repo.Verify(r => r.UpdateAsync(It.IsAny<User>()), Times.Never);
        tokenService.Verify(t => t.GenerateToken(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task ExecuteAsync_WhenPasswordInvalid_ShouldIncrementFailedAttempts_UpdateUser_AndThrow()
    {
        var passwordHash = BCrypt.Net.BCrypt.HashPassword("Aa1!aaaa");
        var user = new User("John", "john@test.com", passwordHash, UserRole.User);

        var repo = new Mock<IUserRepository>();
        repo.Setup(r => r.GetByEmailAsync(It.IsAny<string>()))
            .ReturnsAsync(user);

        var tokenService = new Mock<ITokenService>();
        var sut = new LoginUserUseCase(repo.Object, tokenService.Object);

        var act = () => sut.ExecuteAsync(new LoginRequest { Email = "john@test.com", Password = "wrong" });

        await act.Should().ThrowAsync<Exception>().WithMessage("Invalid credentials");

        user.FailedLoginAttempts.Should().Be(1);
        repo.Verify(r => r.UpdateAsync(It.Is<User>(u => u.FailedLoginAttempts == 1)), Times.Once);
        tokenService.Verify(t => t.GenerateToken(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task ExecuteAsync_WhenPasswordValid_ShouldResetFailedAttempts_UpdateUser_AndReturnToken()
    {
        var passwordHash = BCrypt.Net.BCrypt.HashPassword("Aa1!aaaa");
        var user = new User("John", "john@test.com", passwordHash, UserRole.Admin);

        // simulate previous failures
        user.RegisterFailedLogin();
        user.RegisterFailedLogin();
        user.FailedLoginAttempts.Should().Be(2);

        var repo = new Mock<IUserRepository>();
        repo.Setup(r => r.GetByEmailAsync(It.IsAny<string>()))
            .ReturnsAsync(user);

        var tokenService = new Mock<ITokenService>();
        tokenService.Setup(t => t.GenerateToken(user.Id, user.Email.Value, user.Role.ToString()))
            .Returns("token123");

        var sut = new LoginUserUseCase(repo.Object, tokenService.Object);

        var response = await sut.ExecuteAsync(new LoginRequest { Email = "john@test.com", Password = "Aa1!aaaa" });

        response.Token.Should().Be("token123");
        user.FailedLoginAttempts.Should().Be(0);

        repo.Verify(r => r.UpdateAsync(It.Is<User>(u => u.FailedLoginAttempts == 0)), Times.Once);
        tokenService.Verify(t => t.GenerateToken(user.Id, user.Email.Value, user.Role.ToString()), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_WhenUserLocked_ShouldThrowUnauthorized()
    {
        var passwordHash = BCrypt.Net.BCrypt.HashPassword("Aa1!aaaa");
        var user = new User("John", "john@test.com", passwordHash, UserRole.User);

        // Lock account
        for (var i = 0; i < 5; i++) user.RegisterFailedLogin();
        user.IsLocked.Should().BeTrue();

        var repo = new Mock<IUserRepository>();
        repo.Setup(r => r.GetByEmailAsync(It.IsAny<string>()))
            .ReturnsAsync(user);

        var tokenService = new Mock<ITokenService>();
        var sut = new LoginUserUseCase(repo.Object, tokenService.Object);

        var act = () => sut.ExecuteAsync(new LoginRequest { Email = "john@test.com", Password = "Aa1!aaaa" });

        await act.Should().ThrowAsync<UnauthorizedAccessException>().WithMessage("User account is locked");
        tokenService.Verify(t => t.GenerateToken(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
    }
}
