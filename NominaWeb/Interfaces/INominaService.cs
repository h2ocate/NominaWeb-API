using NominaWeb.Dto.Nomina;
using System.Threading.Tasks;

namespace NominaWeb.Interfaces
{
    public interface INominaService
    {
        Task<NominaDto> AddNominaAsync(NominaCreateDto nominaDto);
        Task<NominaDto> GetNominaByIdAsync(int id);
        Task<IEnumerable<NominaDto>> GetAllNominasAsync(int pageNumber, int pageSize);
        Task UpdateNominaAsync(int id, NominaCreateDto nominaDto);
        Task DeactivateNominaAsync(int id);
    }
}
