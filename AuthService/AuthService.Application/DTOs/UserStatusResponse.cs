namespace AuthService.Application.DTOs;

public class UserStatusResponse
{
    public Guid Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public bool IsLocked { get; set; }
    public DateTime CreatedAt { get; set; }
}
