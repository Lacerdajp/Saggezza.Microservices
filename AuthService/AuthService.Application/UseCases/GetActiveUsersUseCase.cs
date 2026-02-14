using AuthService.Application.DTOs;
using AuthService.Domain.Interfaces;

namespace AuthService.Application.UseCases;

public class GetActiveUsersUseCase
{
    private readonly IUserRepository _repository;

    public GetActiveUsersUseCase(IUserRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<UserStatusResponse>> ExecuteAsync()
    {
        var users = await _repository.GetActiveUsersAsync();

        return users.Select(u => new UserStatusResponse
        {
            Id = u.Id,
            FullName = u.FullName,
            Email = u.Email,
            IsActive = u.IsActive,
            CreatedAt = u.CreatedAt,
            IsLocked = u.IsLocked
        }).ToList();
    }
}
