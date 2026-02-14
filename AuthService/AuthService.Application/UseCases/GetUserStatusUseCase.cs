using AuthService.Application.DTOs;
using AuthService.Domain.Interfaces;

public class GetUserStatusUseCase
{
    private readonly IUserRepository _repository;

    public GetUserStatusUseCase(IUserRepository repository)
    {
        _repository = repository;
    }

    public async Task<UserStatusResponse> ExecuteAsync(Guid userId)
    {
        var user = await _repository.GetByIdAsync(userId);

        if (user is null)
            throw new Exception("User not found");

        return new UserStatusResponse
        {
            Id = user.Id,
            FullName = user.FullName,
            Email = user.Email,
            CreatedAt = user.CreatedAt,
            IsActive = user.IsActive,
            IsLocked = user.IsLocked
        };
    }
}