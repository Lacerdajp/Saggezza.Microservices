using AuthService.Domain.Interfaces;

namespace AuthService.Application.UseCases;

public class DeactivateUserUseCase
{
    private readonly IUserRepository _repository;

    public DeactivateUserUseCase(IUserRepository repository)
    {
        _repository = repository;
    }

    public async Task ExecuteAsync(Guid userId)
    {
        var user = await _repository.GetByIdAsync(userId);

        if (user is null)
            throw new Exception("User not found");

        user.Deactivate();

        await _repository.UpdateAsync(user);
    }
}
