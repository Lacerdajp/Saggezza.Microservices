using AuthService.Domain.Entities;
using AuthService.Domain.Enums;
using FluentAssertions;

namespace AuthService.Tests.Domain;

public class UserTests
{
    [Fact]
    public void RegisterFailedLogin_WhenCalled5Times_ShouldLockUser_AndSetLockoutEnd()
    {
        var user = new User("John", "john@test.com", "hash", UserRole.User);

        for (var i = 0; i < 5; i++)
            user.RegisterFailedLogin();

        user.IsLocked.Should().BeTrue();
        user.FailedLoginAttempts.Should().Be(5);
        user.LockoutEnd.Should().NotBeNull();
    }

    [Fact]
    public void CheckAndUnlockIfExpired_WhenLockoutExpired_ShouldUnlock()
    {
        var user = new User("John", "john@test.com", "hash", UserRole.User);
        for (var i = 0; i < 5; i++) user.RegisterFailedLogin();
        user.IsLocked.Should().BeTrue();
        typeof(User).GetProperty(nameof(User.LockoutEnd))!
            .SetValue(user, DateTime.UtcNow.AddMinutes(-1));

        user.CheckAndUnlockIfExpired();

        user.IsLocked.Should().BeFalse();
        user.LockoutEnd.Should().BeNull();
        user.FailedLoginAttempts.Should().Be(0);
    }

    [Fact]
    public void Activate_WhenAlreadyActive_ShouldThrow()
    {
        var user = new User("John", "john@test.com", "hash", UserRole.User);

        var act = () => user.Activate();

        act.Should().Throw<InvalidOperationException>().WithMessage("User already active");
    }

    [Fact]
    public void Deactivate_WhenAlreadyInactive_ShouldThrow()
    {
        var user = new User("John", "john@test.com", "hash", UserRole.User);
        user.Deactivate();

        var act = () => user.Deactivate();

        act.Should().Throw<InvalidOperationException>().WithMessage("User already inactive");
    }
}
