using Microsoft.AspNetCore.Mvc;
using NominaWeb.Dto.Usuario;
using NominaWeb.Interfaces;

namespace NominaWeb.Controllers.Usuario
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserRegistrationDTO userDTO)
        {
            try
            {
                var token = await _authService.RegisterUser(userDTO);
                return Ok(new { Token = token });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserLoginDTO userDTO)
        {
            try
            {
                var token = await _authService.Login(userDTO);
                return Ok(new { Token = token });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
