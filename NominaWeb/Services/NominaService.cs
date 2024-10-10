using AutoMapper;
using Microsoft.EntityFrameworkCore;
using NominaWeb.Data;
using NominaWeb.Dto.Empleado;
using NominaWeb.Dto.Nomina;
using NominaWeb.Interfaces;
using NominaWeb.Models;

namespace NominaWeb.Services
{
    public class NominaService : INominaService
    {
        private readonly NominaDbContext _context;
        private readonly IMapper _mapper;

        public NominaService(NominaDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<NominaDto> AddNominaAsync(NominaCreateDto nominaDto)
        {
            try
            {
                // Primero crear la nómina con los datos correctos
                var nomina = new Nominas
                {
                    FechaPago = nominaDto.FechaPago,
                    Deducciones = nominaDto.DeduccionesExtras, // Asigna las deducciones extras
                    TotalPago = 0, // Inicializa el total en 0 para sumar luego
                    NominaEmpleados = new List<NominaEmpleado>() // Inicializa la lista vacía
                };

                // Inicializar totales
                decimal totalDeduccionesGrupo = 0;
                decimal totalBonosGrupo = 0;

                // Calcular el total a pagar, deducciones y otros detalles para cada empleado
                foreach (var empleadoBono in nominaDto.IDEmpleados)
                {
                    var empleado = await _context.Empleados.FindAsync(empleadoBono.IDEmpleado);
                    if (empleado == null)
                    {
                        throw new Exception($"Empleado con ID {empleadoBono.IDEmpleado} no encontrado.");
                    }

                    var salarioBase = empleado.Salario; // Toma el salario del empleado
                    decimal deducciones = CalculateDeductions(salarioBase, nominaDto.DeduccionesExtras);
                    totalDeduccionesGrupo += deducciones; // Acumula las deducciones

                    var totalPagoEmpleado = salarioBase + empleadoBono.Bono - deducciones;

                    // Crear la entrada en la tabla intermedia NominaEmpleado
                    var nominaEmpleado = new NominaEmpleado
                    {
                        IDEmpleado = empleadoBono.IDEmpleado,
                        IDNomina = nomina.IDNomina, // Esto se asignará automáticamente al guardar la nómina
                        SalarioBase = salarioBase,
                        Deducciones = deducciones,
                        Bonos = empleadoBono.Bono,
                        TotalPago = totalPagoEmpleado // Calcula el total a pagar por empleado
                    };

                    // Agregar la relación del empleado a la nómina
                    nomina.NominaEmpleados.Add(nominaEmpleado);

                    // Acumular el total a pagar de la nómina
                    nomina.TotalPago += totalPagoEmpleado;
                    totalBonosGrupo += empleadoBono.Bono; // Acumula los bonos
                }

                // Guardar la nómina en la base de datos
                await _context.Nominas.AddAsync(nomina);
                await _context.SaveChangesAsync(); // Aquí se generará el ID de la nómina y se guardarán los empleados

                // Crear el DTO final para devolver
                var nominaDtoResponse = new NominaDto
                {
                    IDNomina = nomina.IDNomina,
                    FechaPago = nomina.FechaPago,
                    TotalPagoGrupo = nomina.TotalPago,
                    TotalDeduccionesGrupo = totalDeduccionesGrupo,
                    EmpleadosDetalles = nomina.NominaEmpleados.Select(ne => new EmpleadoDetalleDto
                    {
                        IDEmpleado = ne.IDEmpleado,
                        Nombre = _context.Empleados.Find(ne.IDEmpleado)?.Nombre, // Asumiendo que tienes una propiedad Nombre en Empleado
                        SalarioBase = ne.SalarioBase,
                        Deducciones = ne.Deducciones,
                        Bonos = ne.Bonos,
                        TotalPago = ne.TotalPago
                    }).ToList()
                };

                return nominaDtoResponse; // Retorna el DTO completo con toda la información
            }
            catch (DbUpdateException ex)
            {
                // Manejar la excepción de actualización
                var innerException = ex.InnerException?.Message;
                throw new Exception($"Error al guardar la nómina: {innerException}");
            }
            catch (Exception ex)
            {
                // Manejar otras excepciones
                throw new Exception($"Ocurrió un error inesperado: {ex.Message}");
            }
        }



        public async Task<NominaDto> GetNominaByIdAsync(int id)
        {
            // Obtener la nómina incluyendo los empleados relacionados
            var nomina = await _context.Nominas
                .Include(n => n.NominaEmpleados) // Incluir la lista de empleados en la nómina
                .ThenInclude(ne => ne.Empleado) // Incluir los detalles del empleado
                .FirstOrDefaultAsync(n => n.IDNomina == id); // Buscar la nómina por ID

            if (nomina == null)
            {
                throw new Exception("Nómina no encontrada.");
            }

            // Mapear la nómina a DTO
            var nominaDto = _mapper.Map<NominaDto>(nomina);

            // Agregar detalles de los empleados al DTO
            nominaDto.EmpleadosDetalles = nomina.NominaEmpleados.Select(ne => new EmpleadoDetalleDto
            {
                IDEmpleado = ne.IDEmpleado,
                Nombre = ne.Empleado.Nombre, // Asumiendo que el Empleado tiene una propiedad Nombre
                SalarioBase = ne.SalarioBase,
                Deducciones = ne.Deducciones,
                Bonos = ne.Bonos,
                TotalPago = ne.TotalPago
            }).ToList();

            // Calcular el total de pagos y deducciones
            nominaDto.TotalPagoGrupo = nomina.NominaEmpleados.Sum(ne => ne.TotalPago);
            nominaDto.TotalDeduccionesGrupo = nomina.NominaEmpleados.Sum(ne => ne.Deducciones);

            return nominaDto;
        }



        public async Task<IEnumerable<NominaDto>> GetAllNominasAsync()
        {
            var nominas = await _context.Nominas.ToListAsync();
            return _mapper.Map<IEnumerable<NominaDto>>(nominas);
        }

        public async Task UpdateNominaAsync(int id, NominaCreateDto nominaDto)
        {
            var nomina = await _context.Nominas.Include(n => n.NominaEmpleados).FirstOrDefaultAsync(n => n.IDNomina == id);
            if (nomina == null) throw new Exception("Nómina no encontrada.");

            // Actualiza los campos según el DTO
            nomina.FechaPago = nominaDto.FechaPago;

            // Limpiar las entradas existentes de NominaEmpleados antes de actualizar
            _context.NominaEmpleados.RemoveRange(nomina.NominaEmpleados);

            // Recalcular los totales y agregar empleados
            decimal totalDeducciones = 0;
            decimal totalPago = 0;

            foreach (var empleadoBono in nominaDto.IDEmpleados)
            {
                var empleado = await _context.Empleados.FindAsync(empleadoBono.IDEmpleado);
                if (empleado == null)
                {
                    throw new Exception($"Empleado con ID {empleadoBono.IDEmpleado} no encontrado.");
                }

                var salarioBase = empleado.Salario; // Toma el salario del empleado
                decimal deducciones = CalculateDeductions(salarioBase, nominaDto.DeduccionesExtras);
                decimal totalEmpleadoPago = salarioBase + empleadoBono.Bono - deducciones;

                var nominaEmpleado = new NominaEmpleado
                {
                    Empleado = empleado,
                    SalarioBase = salarioBase,
                    Deducciones = deducciones,
                    Bonos = empleadoBono.Bono,
                    TotalPago = totalEmpleadoPago
                };

                nomina.NominaEmpleados.Add(nominaEmpleado);

                // Acumular los totales
                totalDeducciones += deducciones;
                totalPago += totalEmpleadoPago;
            }

            // Actualizar el total de la nómina
            nomina.Deducciones = totalDeducciones;
            nomina.TotalPago = totalPago;

            _context.Nominas.Update(nomina);
            await _context.SaveChangesAsync();
        }

        public async Task DeactivateNominaAsync(int id)
        {
            var nomina = await _context.Nominas.FindAsync(id);
            if (nomina == null) throw new Exception("Nómina no encontrada.");

            // Aquí puedes aplicar la lógica para "desactivar" la nómina
            // Por ejemplo, establecer un campo `Estado` si lo tienes en tu modelo
            _context.Nominas.Remove(nomina); // Cambiar por la lógica que desees para desactivar
            await _context.SaveChangesAsync();
        }

        private decimal CalculateDeductions(decimal salarioBase, decimal? extraDeduccion = 0)
        {
            // Calcular AFP: 2.87% hasta el salario máximo cotizable
            decimal afpMaximo = 102859.92m;
            decimal afp = salarioBase <= afpMaximo ? salarioBase * 0.0287m : afpMaximo * 0.0287m;

            // Calcular ARS: 3.04% hasta el salario máximo cotizable
            decimal arsMaximo = 102859.92m;
            decimal ars = salarioBase <= arsMaximo ? salarioBase * 0.0304m : arsMaximo * 0.0304m;

            // Calcular ISR de acuerdo a los tramos
            decimal isr = 0;
            if (salarioBase > 72030.00m)
            {
                isr = 6654.63m + ((salarioBase - 72030.00m) * 0.25m);
            }
            else if (salarioBase > 52028.00m)
            {
                isr = 2616.43m + ((salarioBase - 52028.00m) * 0.20m);
            }
            else if (salarioBase > 34685.00m)
            {
                isr = (salarioBase - 34685.00m) * 0.15m;
            }

            // Calcular deducción extra (si se especifica)
            decimal extra = extraDeduccion > 0 ? extraDeduccion.Value : 0; // Solo aplicará el valor proporcionado

            // Sumar todas las deducciones
            return afp + ars + isr + extra;
        }
    }
}
