using dotnetapp.Exceptions;
using dotnetapp.Models;
using dotnetapp.Services;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class AuthenticationController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly ILogger<AuthenticationController> _logger;

    public AuthenticationController(IAuthService authService, ILogger<AuthenticationController> logger)
    {
        _authService = authService;
        _logger = logger;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] User model)
    {
        try
        {
            var (code, msg) = await _authService.Registration(model, model.UserRole);
            if (code == 1)
            {
                return Ok(msg);
            }
            return BadRequest(msg);
        }
        catch (LoanException ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginModel model)
    {
        try
        {
            var (code, message) = await _authService.Login(model);
            if (code == 1)
            {
                return Ok(new { token = message });
            }
            return BadRequest(message);
        }
        catch (LoanException ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    [HttpGet("{userId}")]
    public async Task<IActionResult> GetUserById(int userId)
    {
        var user = await _authService.GetUserByIdAsync(userId);
        if (user == null)
        {
            return NotFound("User not found");
        }

        return Ok(user);
    }
}