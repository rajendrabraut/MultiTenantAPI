using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MultiTenantAPI.Application.Auth;

namespace MultiTenantAPI.API.Controllers;

[ApiController]
[Route("api/auth")]
public sealed class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly IValidator<LoginRequest> _validator;

    public AuthController(IAuthService authService, IValidator<LoginRequest> validator)
    {
        _authService = authService;
        _validator = validator;
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginRequest request, CancellationToken cancellationToken)
    {
        var validation = await _validator.ValidateAsync(request, cancellationToken);
        if (!validation.IsValid)
        {
            return ValidationProblem(validation.ToDictionary());
        }

        var response = await _authService.LoginAsync(request, cancellationToken);
        return Ok(response);
    }
}
