using AuthService.Application.DTOs;
using AuthService.Domain.Interfaces;

namespace AuthService.Application.UseCases;

public class GetLockedUsersUseCase
{
    private readonly IUserRepository _repository;

    public GetLockedUsersUseCase(IUserRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<LockedUserResponse>> ExecuteAsync()
    {
        var users = await _repository.GetLockedUsersAsync();

        return users.Select(u => new LockedUserResponse
        {
            Id = u.Id,
            FullName = u.FullName,
            Email = u.Email,
            LockoutEnd = u.LockoutEnd,
            FailedLoginAttempts = u.FailedLoginAttempts
        }).ToList();
    }
}
