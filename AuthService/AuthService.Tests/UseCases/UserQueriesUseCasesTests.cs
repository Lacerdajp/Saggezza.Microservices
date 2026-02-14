using AuthService.Application.UseCases;
using AuthService.Domain.Entities;
using AuthService.Domain.Enums;
using AuthService.Domain.Interfaces;
using FluentAssertions;
using Moq;

namespace AuthService.Tests.UseCases;

public class UserQueriesUseCasesTests
{
    [Fact]
    public async Task GetLockedUsersUseCase_ShouldMapUsersCorrectly()
    {
        var users = new List<User>
        {
            new("Locked 1", "l1@test.com", "hash", UserRole.User),
            new("Locked 2", "l2@test.com", "hash", UserRole.Admin)
        };
        users[0].RegisterFailedLogin();
        users[0].RegisterFailedLogin();
        users[0].RegisterFailedLogin();
        users[0].RegisterFailedLogin();
        users[0].RegisterFailedLogin();

        users[1].RegisterFailedLogin();
        users[1].RegisterFailedLogin();
        users[1].RegisterFailedLogin();
        users[1].RegisterFailedLogin();
        users[1].RegisterFailedLogin();

        var repo = new Mock<IUserRepository>();
        repo.Setup(r => r.GetLockedUsersAsync()).ReturnsAsync(users);

        var sut = new GetLockedUsersUseCase(repo.Object);

        var result = await sut.ExecuteAsync();

        result.Should().HaveCount(2);
        result[0].Id.Should().Be(users[0].Id);
        result[0].FullName.Should().Be(users[0].FullName);
        result[0].Email.Should().Be(users[0].Email);
        result[0].LockoutEnd.Should().Be(users[0].LockoutEnd);
        result[0].FailedLoginAttempts.Should().Be(users[0].FailedLoginAttempts);
    }

    [Fact]
    public async Task GetActiveUsersUseCase_ShouldMapUsersCorrectly()
    {
        var user1 = new User("A", "a@test.com", "hash", UserRole.User);
        var user2 = new User("B", "b@test.com", "hash", UserRole.Admin);
        user2.Deactivate();

        var repo = new Mock<IUserRepository>();
        repo.Setup(r => r.GetActiveUsersAsync()).ReturnsAsync(new List<User> { user1 });

        var sut = new GetActiveUsersUseCase(repo.Object);

        var result = await sut.ExecuteAsync();

        result.Should().ContainSingle();
        result[0].Id.Should().Be(user1.Id);
        result[0].Email.Should().Be(user1.Email);
        result[0].IsActive.Should().BeTrue();
    }

    [Fact]
    public async Task GetInactiveUsersUseCase_ShouldMapUsersCorrectly()
    {
        var user1 = new User("A", "a@test.com", "hash", UserRole.User);
        user1.Deactivate();

        var repo = new Mock<IUserRepository>();
        repo.Setup(r => r.GetInactiveUsersAsync()).ReturnsAsync(new List<User> { user1 });

        var sut = new GetInactiveUsersUseCase(repo.Object);

        var result = await sut.ExecuteAsync();

        result.Should().ContainSingle();
        result[0].Id.Should().Be(user1.Id);
        result[0].Email.Should().Be(user1.Email);
        result[0].IsActive.Should().BeFalse();
    }

    [Fact]
    public async Task GetUserStatusUseCase_WhenUserNotFound_ShouldThrow()
    {
        var repo = new Mock<IUserRepository>();
        repo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((User?)null);

        var sut = new GetUserStatusUseCase(repo.Object);

        var act = () => sut.ExecuteAsync(Guid.NewGuid());

        await act.Should().ThrowAsync<Exception>().WithMessage("User not found");
    }

    [Fact]
    public async Task GetUserStatusUseCase_WhenUserFound_ShouldMap()
    {
        var user = new User("A", "a@test.com", "hash", UserRole.User);

        var repo = new Mock<IUserRepository>();
        repo.Setup(r => r.GetByIdAsync(user.Id)).ReturnsAsync(user);

        var sut = new GetUserStatusUseCase(repo.Object);

        var result = await sut.ExecuteAsync(user.Id);

        result.Id.Should().Be(user.Id);
        result.FullName.Should().Be(user.FullName);
        result.Email.Should().Be(user.Email);
        result.IsActive.Should().Be(user.IsActive);
        result.IsLocked.Should().Be(user.IsLocked);
        result.CreatedAt.Should().Be(user.CreatedAt);
    }
}
