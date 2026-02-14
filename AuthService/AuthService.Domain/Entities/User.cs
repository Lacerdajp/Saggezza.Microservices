namespace AuthService.Domain.Entities;

using AuthService.Domain.Enums;
using SharedKernel.ValueObjects;

public class User
{
    public Guid Id { get; private set; }
    public string FullName { get; private set; }
    public Email Email { get; private set; }
    public string PasswordHash { get; private set; }
    public UserRole Role { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public int FailedLoginAttempts { get; private set; }
    public bool IsLocked { get; private set; }
    public DateTime? LockoutEnd { get; private set; }
    public bool IsActive { get; private set; }

    private User() { }

    public User(
        string fullName,
        string email,
        string passwordHash,
        UserRole role)
    {
        Id = Guid.NewGuid();
        FullName = fullName;
        Email = Email.Create(email);
        PasswordHash = passwordHash;
        Role = role;
        CreatedAt = DateTime.UtcNow;
        IsActive = true;
        FailedLoginAttempts = 0;
        IsLocked = false;
        LockoutEnd = null;
    }

    public void Deactivate()
    {
        if (!IsActive)
            throw new InvalidOperationException("User already inactive");

        IsActive = false;
    }

    public void Activate()
    {
        if (IsActive)
            throw new InvalidOperationException("User already active");

        IsActive = true;
    }

    public void RegisterFailedLogin()
    {
        FailedLoginAttempts++;

        if (FailedLoginAttempts >= 5)
        {
            IsLocked = true;
            LockoutEnd = DateTime.UtcNow.AddMinutes(15);
        }

    }

    public void CheckAndUnlockIfExpired()
    {
        if (IsLocked && LockoutEnd.HasValue && LockoutEnd < DateTime.UtcNow)
        {
            Unlock();
        }
    }

    public void ResetFailedLoginAttempts()
    {
        FailedLoginAttempts = 0;
    }

    public void Unlock()
    {
        IsLocked = false;
        FailedLoginAttempts = 0;
        LockoutEnd = null;
    }
}
