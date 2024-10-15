using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using NominaWeb.Data;
using NominaWeb.Dto.Usuario;
using NominaWeb.Enum;
using NominaWeb.Helpers.Usuario;
using NominaWeb.Interfaces;
using NominaWeb.Models.Usuario;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace NominaWeb.Services.Usuario
{
    public class AuthService : IAuthService
    {
        private readonly NominaDbContext _context;
        private readonly IConfiguration _config;

        public AuthService(NominaDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        public async Task<string> RegisterUser(UserRegistrationDTO userDTO)
        {
            if (await _context.Users.AnyAsync(u => u.Username == userDTO.Username))
                throw new Exception("Existe ya este nombre de usuario");

            if (!RoleValidator.IsValidRole(userDTO.Role))
                throw new Exception("El rol especificado no encontrado");

            var user = new User
            {
                Username = userDTO.Username,
                Email = userDTO.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(userDTO.Password), // Encriptación de contraseña
                Role = (UserRole)userDTO.Role // Asignación del rol
            };

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            return GenerateJwtToken(user); // Retorna token JWT al registrar
        }


        public async Task<string> Login(UserLoginDTO userDTO)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == userDTO.Username);

            if (user == null || !BCrypt.Net.BCrypt.Verify(userDTO.Password, user.PasswordHash))
                throw new Exception("Invalid username or password");

            if (!user.IsActive)
                throw new Exception("User is inactive");

            return GenerateJwtToken(user); // Retorna el token JWT
        }

        public string GenerateJwtToken(User user)
        {
            var claims = new[]
            {
            new Claim(JwtRegisteredClaimNames.Sub, user.Username),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role.ToString()), // Rol basado en enum
            new Claim("IsActive", user.IsActive.ToString())
        };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                _config["Jwt:Issuer"],
                _config["Jwt:Issuer"],
                claims,
                expires: DateTime.Now.AddMinutes(30), // Expiración del token
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
