using NominaWeb.Enum;

namespace NominaWeb.Dto.Usuario
{
    public class UserRegistrationDTO
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; } // Contraseña sin encriptar
        public int Role { get; set; } // Cambia esto a int para usar con el Enum

    }

}
