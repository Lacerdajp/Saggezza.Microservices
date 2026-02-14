using AuthService.Application.DTOs;
using AuthService.Application.UseCases;
using Microsoft.AspNetCore.Mvc;

namespace AuthService.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly RegisterUserUseCase _registerUseCase;
    private readonly LoginUserUseCase _loginUseCase;

    public AuthController(
        RegisterUserUseCase registerUseCase,
        LoginUserUseCase loginUseCase)
    {
        _registerUseCase = registerUseCase;
        _loginUseCase = loginUseCase;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterUserRequest request)
    {
        await _registerUseCase.ExecuteAsync(request);
        return Ok("User created successfully");
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        var response = await _loginUseCase.ExecuteAsync(request);
        return Ok(response);
    }
}
