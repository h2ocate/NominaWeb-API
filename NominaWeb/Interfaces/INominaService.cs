using NominaWeb.Dto.Nomina;

namespace NominaWeb.Interfaces
{
    public interface INominaService
    {
        Task<NominaDto> AddNominaAsync(NominaCreateDto nominaDto);
        Task<NominaDto> GetNominaByIdAsync(int id);
        Task<IEnumerable<NominaDto>> GetAllNominasAsync();
        Task UpdateNominaAsync(int id, NominaCreateDto nominaDto);
        Task DeactivateNominaAsync(int id);
    }
}
