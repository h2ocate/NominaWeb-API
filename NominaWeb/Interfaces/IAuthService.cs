using NominaWeb.Dto.Usuario;
using NominaWeb.Models.Usuario;

namespace NominaWeb.Interfaces
{
    public interface IAuthService
    {
        Task<string> RegisterUser(UserRegistrationDTO userDTO);
        Task<string> Login(UserLoginDTO userDTO);
        string GenerateJwtToken(User user);
    }

}
