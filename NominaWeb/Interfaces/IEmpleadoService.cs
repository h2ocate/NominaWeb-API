using NominaWeb.Dto.Empleado;
using NominaWeb.Models;

namespace NominaWeb.Interfaces
{
    public interface IEmpleadoService
    {
        Task<IEnumerable<EmpleadoDTO>> GetAllEmpleadosAsync();
        Task<EmpleadoDTO> GetEmpleadoByIdAsync(int id);
        Task<EmpleadoDTO> CreateEmpleadoAsync(CreateEmpleadoDTO empleadoDto);
        Task<EmpleadoDTO> UpdateEmpleadoAsync(UpdateEmpleadoDTO empleadoDto);
        Task<bool> DeleteEmpleadoAsync(int id);
        Task<IEnumerable<Cargo>> GetCargosAsync();
    }
}
