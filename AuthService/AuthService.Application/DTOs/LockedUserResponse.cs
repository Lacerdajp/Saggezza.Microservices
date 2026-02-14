namespace AuthService.Application.DTOs;

public class LockedUserResponse
{
    public Guid Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public DateTime? LockoutEnd { get; set; }
    public int FailedLoginAttempts { get; set; }
}
