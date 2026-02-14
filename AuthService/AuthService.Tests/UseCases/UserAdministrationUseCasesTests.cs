using AuthService.Application.UseCases;
using AuthService.Domain.Entities;
using AuthService.Domain.Enums;
using AuthService.Domain.Interfaces;
using FluentAssertions;
using Moq;

namespace AuthService.Tests.UseCases;

public class UserAdministrationUseCasesTests
{
    [Fact]
    public async Task UnlockUserUseCase_WhenUserNotFound_ShouldThrow()
    {
        var repo = new Mock<IUserRepository>();
        repo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((User?)null);

        var sut = new UnlockUserUseCase(repo.Object);

        var act = () => sut.ExecuteAsync(Guid.NewGuid());

        await act.Should().ThrowAsync<Exception>().WithMessage("User not found");
        repo.Verify(r => r.UpdateAsync(It.IsAny<User>()), Times.Never);
    }

    [Fact]
    public async Task UnlockUserUseCase_WhenUserExists_ShouldUnlockAndUpdate()
    {
        var user = new User("John", "john@test.com", "hash", UserRole.User);
        for (var i = 0; i < 5; i++) user.RegisterFailedLogin();
        user.IsLocked.Should().BeTrue();

        var repo = new Mock<IUserRepository>();
        repo.Setup(r => r.GetByIdAsync(user.Id)).ReturnsAsync(user);

        var sut = new UnlockUserUseCase(repo.Object);

        await sut.ExecuteAsync(user.Id);

        user.IsLocked.Should().BeFalse();
        user.FailedLoginAttempts.Should().Be(0);
        user.LockoutEnd.Should().BeNull();

        repo.Verify(r => r.UpdateAsync(It.Is<User>(u => u.Id == user.Id && !u.IsLocked)), Times.Once);
    }

    [Fact]
    public async Task DeactivateUserUseCase_WhenUserNotFound_ShouldThrow()
    {
        var repo = new Mock<IUserRepository>();
        repo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((User?)null);

        var sut = new DeactivateUserUseCase(repo.Object);

        var act = () => sut.ExecuteAsync(Guid.NewGuid());

        await act.Should().ThrowAsync<Exception>().WithMessage("User not found");
    }

    [Fact]
    public async Task DeactivateUserUseCase_WhenUserExists_ShouldDeactivateAndUpdate()
    {
        var user = new User("John", "john@test.com", "hash", UserRole.User);
        user.IsActive.Should().BeTrue();

        var repo = new Mock<IUserRepository>();
        repo.Setup(r => r.GetByIdAsync(user.Id)).ReturnsAsync(user);

        var sut = new DeactivateUserUseCase(repo.Object);

        await sut.ExecuteAsync(user.Id);

        user.IsActive.Should().BeFalse();
        repo.Verify(r => r.UpdateAsync(It.Is<User>(u => u.Id == user.Id && !u.IsActive)), Times.Once);
    }

    [Fact]
    public async Task ActivateUserUseCase_WhenUserNotFound_ShouldThrow()
    {
        var repo = new Mock<IUserRepository>();
        repo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((User?)null);

        var sut = new ActivateUserUseCase(repo.Object);

        var act = () => sut.ExecuteAsync(Guid.NewGuid());

        await act.Should().ThrowAsync<Exception>().WithMessage("User not found");
    }

    [Fact]
    public async Task ActivateUserUseCase_WhenUserExists_ShouldActivateAndUpdate()
    {
        var user = new User("John", "john@test.com", "hash", UserRole.User);
        user.Deactivate();
        user.IsActive.Should().BeFalse();

        var repo = new Mock<IUserRepository>();
        repo.Setup(r => r.GetByIdAsync(user.Id)).ReturnsAsync(user);

        var sut = new ActivateUserUseCase(repo.Object);

        await sut.ExecuteAsync(user.Id);

        user.IsActive.Should().BeTrue();
        repo.Verify(r => r.UpdateAsync(It.Is<User>(u => u.Id == user.Id && u.IsActive)), Times.Once);
    }
}
