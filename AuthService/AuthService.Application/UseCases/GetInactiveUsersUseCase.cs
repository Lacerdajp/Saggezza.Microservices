using AuthService.Application.DTOs;
using AuthService.Domain.Interfaces;

namespace AuthService.Application.UseCases;

public class GetInactiveUsersUseCase
{
    private readonly IUserRepository _repository;

    public GetInactiveUsersUseCase(IUserRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<UserStatusResponse>> ExecuteAsync()
    {
        var users = await _repository.GetInactiveUsersAsync();

        return users.Select(u => new UserStatusResponse
        {
            Id = u.Id,
            FullName = u.FullName,
            Email = u.Email,
            IsActive = u.IsActive,
            IsLocked = u.IsLocked,
            CreatedAt = u.CreatedAt
        }).ToList();
    }
}
