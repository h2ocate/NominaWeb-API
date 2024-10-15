using System; // Asegúrate de que esta línea está incluida.
using NominaWeb.Enum; // Asegúrate de que esta línea es correcta y está en el espacio de nombres adecuado.

namespace NominaWeb.Helpers.Usuario
{
    public static class RoleValidator
    {
        public static bool IsValidRole(int role)
        {
            return System.Enum.IsDefined(typeof(UserRole), role);
        }
    }
}
