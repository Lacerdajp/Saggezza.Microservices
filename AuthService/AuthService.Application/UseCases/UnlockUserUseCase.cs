using AuthService.Domain.Interfaces;

namespace AuthService.Application.UseCases;

public class UnlockUserUseCase
{
    private readonly IUserRepository _repository;

    public UnlockUserUseCase(IUserRepository repository)
    {
        _repository = repository;
    }

    public async Task ExecuteAsync(Guid useId)
    {
        var user = await _repository.GetByIdAsync(useId);

        if (user is null)
            throw new Exception("User not found");

        user.Unlock();

        await _repository.UpdateAsync(user);
    }
}
