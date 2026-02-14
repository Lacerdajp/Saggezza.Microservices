using AuthService.Application.DTOs;
using AuthService.Application.UseCases;
using AuthService.Domain.Entities;
using AuthService.Domain.Enums;
using AuthService.Domain.Interfaces;
using FluentAssertions;
using Moq;

namespace AuthService.Tests.UseCases;

public class RegisterUserUseCaseTests
{
    [Fact]
    public async Task ExecuteAsync_WhenUserAlreadyExists_ShouldThrow()
    {
        var repo = new Mock<IUserRepository>();
        repo.Setup(r => r.GetByEmailAsync(It.IsAny<string>()))
            .ReturnsAsync(new User("Existing", "existing@test.com", "hash", UserRole.User));

        var sut = new RegisterUserUseCase(repo.Object);

        var act = () => sut.ExecuteAsync(new RegisterUserRequest
        {
            FullName = "John Doe",
            Email = "existing@test.com",
            Password = "Aa1!aaaa",
            Role = "User"
        });

        await act.Should().ThrowAsync<Exception>()
            .WithMessage("User already exists");

        repo.Verify(r => r.AddAsync(It.IsAny<User>()), Times.Never);
    }

    [Fact]
    public async Task ExecuteAsync_WhenPasswordIsWeak_ShouldThrow()
    {
        var repo = new Mock<IUserRepository>();
        repo.Setup(r => r.GetByEmailAsync(It.IsAny<string>()))
            .ReturnsAsync((User?)null);

        var sut = new RegisterUserUseCase(repo.Object);

        var act = () => sut.ExecuteAsync(new RegisterUserRequest
        {
            FullName = "John Doe",
            Email = "john@test.com",
            Password = "weak",
            Role = "User"
        });

        await act.Should().ThrowAsync<ArgumentException>();
        repo.Verify(r => r.AddAsync(It.IsAny<User>()), Times.Never);
    }

    [Fact]
    public async Task ExecuteAsync_WhenValidRequest_ShouldAddUser_WithDefaultRoleIfNullOrInvalid()
    {
        var repo = new Mock<IUserRepository>();
        repo.Setup(r => r.GetByEmailAsync(It.IsAny<string>()))
            .ReturnsAsync((User?)null);

        User? captured = null;
        repo.Setup(r => r.AddAsync(It.IsAny<User>()))
            .Callback<User>(u => captured = u)
            .Returns(Task.CompletedTask);

        var sut = new RegisterUserUseCase(repo.Object);

        await sut.ExecuteAsync(new RegisterUserRequest
        {
            FullName = "John Doe",
            Email = "john@test.com",
            Password = "Aa1!aaaa",
            Role = "NotARole"
        });

        captured.Should().NotBeNull();
        captured!.FullName.Should().Be("John Doe");
        captured.Email.Value.Should().Be("john@test.com");
        captured.Role.Should().Be(UserRole.User);
        captured.PasswordHash.Should().NotBeNullOrWhiteSpace();
        captured.PasswordHash.Should().NotBe("Aa1!aaaa");

        repo.Verify(r => r.AddAsync(It.IsAny<User>()), Times.Once);
    }
}
