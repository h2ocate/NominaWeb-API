using AutoMapper;
using Microsoft.EntityFrameworkCore;
using NominaWeb.Data;
using NominaWeb.Dto.Empleado;
using NominaWeb.Interfaces;
using NominaWeb.Models;

namespace NominaWeb.Services
{
    public class EmpleadoService : IEmpleadoService
    {
        private readonly NominaDbContext _context;
        private readonly IMapper _mapper;

        public EmpleadoService(NominaDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<object> GetEmpleadosPaginatedAsync(int pageNumber, int pageSize)
        {
            var query = _context.Empleados
                .Where(e => e.Estado == true); // Solo empleados activos

            var totalRecords = await query.CountAsync();

            var empleados = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var empleadosDTO = _mapper.Map<IEnumerable<EmpleadoDTO>>(empleados);

            return new
            {
                empleados = empleadosDTO,
                totalRecords = totalRecords
            };
        }


        public async Task<IEnumerable<EmpleadoDTO>> GetAllEmpleadosAsync()
        {
            var empleados = await _context.Empleados
                   .Where(e => e.Estado == true) // Solo empleados activos
                   .ToListAsync();
            return _mapper.Map<IEnumerable<EmpleadoDTO>>(empleados);
        }

        public async Task<EmpleadoDTO> GetEmpleadoByIdAsync(int id)
        {
            var empleado = await _context.Empleados.FindAsync(id);
            return empleado == null ? null : _mapper.Map<EmpleadoDTO>(empleado);
        }

        public async Task<EmpleadoDTO> CreateEmpleadoAsync(CreateEmpleadoDTO empleadoDto)
        {
            var empleado = _mapper.Map<Empleado>(empleadoDto);
            _context.Empleados.Add(empleado);
            await _context.SaveChangesAsync();
            return _mapper.Map<EmpleadoDTO>(empleado);
        }

        public async Task<EmpleadoDTO> UpdateEmpleadoAsync(UpdateEmpleadoDTO empleadoDto)
        {
            var empleado = await _context.Empleados.FindAsync(empleadoDto.IDEmpleado);
            if (empleado == null)
            {
                return null;
            }

            _mapper.Map(empleadoDto, empleado);
            await _context.SaveChangesAsync();
            return _mapper.Map<EmpleadoDTO>(empleado);
        }

        public async Task<bool> DeleteEmpleadoAsync(int id)
        {
            var empleado = await _context.Empleados.FindAsync(id);
            if (empleado == null)
            {
                return false;
            }

            empleado.Estado = false; // Baja lógica
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<Cargo>> GetCargosAsync()
        {
            // Obtiene todos los cargos de la base de datos
            return await _context.Cargos.ToListAsync();
        }
    }
}
