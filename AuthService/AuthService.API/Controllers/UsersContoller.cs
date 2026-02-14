using AuthService.Application.UseCases;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AuthService.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly UnlockUserUseCase _unlockUserUseCase;
    private readonly GetLockedUsersUseCase _getLockedUsersUseCase;
    private readonly DeactivateUserUseCase _deactivateUserUseCase;
    private readonly ActivateUserUseCase _activateUserUseCase;
    private readonly GetActiveUsersUseCase _getActiveUsersUseCase;
    private readonly GetInactiveUsersUseCase _getInactiveUsersUseCase;
    private readonly GetUserStatusUseCase _getUserStatusUseCase;


    public UsersController(UnlockUserUseCase unlockUserUseCase, GetLockedUsersUseCase getLockedUsersUseCase,DeactivateUserUseCase deactivateUserUseCase,ActivateUserUseCase activateUserUseCase,GetActiveUsersUseCase getActiveUsersUseCase, GetInactiveUsersUseCase getInactiveUsersUseCase,GetUserStatusUseCase getUserStatusUseCase)
    {
        _unlockUserUseCase = unlockUserUseCase;
        _getLockedUsersUseCase = getLockedUsersUseCase;
        _deactivateUserUseCase = deactivateUserUseCase;
        _activateUserUseCase = activateUserUseCase;
        _getActiveUsersUseCase = getActiveUsersUseCase;
        _getInactiveUsersUseCase = getInactiveUsersUseCase;
        _getUserStatusUseCase = getUserStatusUseCase;
    }

    [HttpPut("{userId}/unlock")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Unlock(string userId)
    {
        await _unlockUserUseCase.ExecuteAsync(Guid.Parse(userId));
        return Ok("User unlocked successfully");
    }
    [HttpGet("locked")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetLockedUsers()
    {
        var users = await _getLockedUsersUseCase.ExecuteAsync();
        return Ok(users);
    }
    [HttpGet("active")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetActiveUsers()
    {
        var users = await _getActiveUsersUseCase.ExecuteAsync();
        return Ok(users);
    }
    [HttpGet("inactive")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetInactiveUsers()
    {
        var users = await _getInactiveUsersUseCase.ExecuteAsync();
        return Ok(users);
    }
    [HttpPut("{userId}/deactivate")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Deactivate(string userId)
    {
        await _deactivateUserUseCase.ExecuteAsync(Guid.Parse(userId));
        return Ok("User deactivated successfully");
    }
    [HttpPut("{userId}/activate")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Activate(string userId)
    {
        await _activateUserUseCase.ExecuteAsync(Guid.Parse(userId));
        return Ok("User activated successfully");
    }
    [HttpGet("{id}/status")]
    public async Task<IActionResult> GetStatus(Guid id)
    {
        var loggedUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (loggedUserId is null)
            return Unauthorized();

        var isAdmin = User.IsInRole("Admin");

        if (!isAdmin && loggedUserId != id.ToString())
            return Forbid();

        var result = await _getUserStatusUseCase.ExecuteAsync(id);

        return Ok(result);
    }


}
