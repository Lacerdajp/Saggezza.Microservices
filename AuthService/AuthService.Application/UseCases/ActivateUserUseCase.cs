using AuthService.Domain.Interfaces;

namespace AuthService.Application.UseCases;

public class ActivateUserUseCase
{
    private readonly IUserRepository _repository;

    public ActivateUserUseCase(IUserRepository repository)
    {
        _repository = repository;
    }

    public async Task ExecuteAsync(Guid userId)
    {
        var user = await _repository.GetByIdAsync(userId);

        if (user is null)
            throw new Exception("User not found");

        user.Activate();

        await _repository.UpdateAsync(user);
    }
}
