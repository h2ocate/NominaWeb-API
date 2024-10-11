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
                // Ajustar la fecha de pago si cae en fin de semana
                DateTime fechaPago = AdjustPaymentDate(nominaDto.FechaPago);

                // Crear una nueva nómina
                var nomina = new Nominas
                {
                    FechaPago = fechaPago,
                    Deducciones = nominaDto.DeduccionesExtras,
                    TotalPago = 0,
                    Bonos = 0,
                    NominaEmpleados = new List<NominaEmpleado>()
                };

                // Inicializar totales
                decimal totalDeduccionesGrupo = 0;
                decimal totalBonosGrupo = 0;

                // Agregar empleados a la nómina al momento de crearla
                foreach (var empleadoBono in nominaDto.IDEmpleados)
                {
                    var empleado = await _context.Empleados.FindAsync(empleadoBono.IDEmpleado);
                    if (empleado == null)
                    {
                        throw new Exception($"Empleado con ID {empleadoBono.IDEmpleado} no encontrado.");
                    }

                    var salarioBase = empleado.Salario;
                    decimal deducciones = CalculateDeductions(salarioBase, nominaDto.DeduccionesExtras);
                    totalDeduccionesGrupo += deducciones;

                    var totalPagoEmpleado = salarioBase + empleadoBono.Bono - deducciones;

                    var nominaEmpleado = new NominaEmpleado
                    {
                        IDEmpleado = empleadoBono.IDEmpleado,
                        IDNomina = nomina.IDNomina,
                        SalarioBase = salarioBase,
                        Deducciones = deducciones,
                        Bonos = empleadoBono.Bono,
                        TotalPago = totalPagoEmpleado
                    };

                    nomina.NominaEmpleados.Add(nominaEmpleado);
                    nomina.TotalPago += totalPagoEmpleado;
                    totalBonosGrupo += empleadoBono.Bono;
                }

                nomina.Bonos = totalBonosGrupo;

                // Guardar la nómina en la base de datos
                await _context.Nominas.AddAsync(nomina);
                await _context.SaveChangesAsync();

                // Retornar el DTO con la información de la nómina creada
                return new NominaDto
                {
                    IDNomina = nomina.IDNomina,
                    FechaPago = nomina.FechaPago,
                    TotalPagoGrupo = nomina.TotalPago,
                    TotalDeduccionesGrupo = totalDeduccionesGrupo,
                    EmpleadosDetalles = nomina.NominaEmpleados.Select(ne => new EmpleadoDetalleDto
                    {
                        IDEmpleado = ne.IDEmpleado,
                        Nombre = _context.Empleados.Find(ne.IDEmpleado)?.Nombre,
                        SalarioBase = ne.SalarioBase,
                        Deducciones = ne.Deducciones,
                        Bonos = ne.Bonos,
                        TotalPago = ne.TotalPago
                    }).ToList()
                };
            }
            catch (DbUpdateException ex)
            {
                var innerException = ex.InnerException?.Message;
                throw new Exception($"Error al guardar la nómina: {innerException}");
            }
            catch (Exception ex)
            {
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



            public async Task<IEnumerable<NominaDto>> GetAllNominasAsync(int pageNumber, int pageSize)
            {
                // Obtener el total de nóminas para el cálculo de la paginación
                var totalNominas = await _context.Nominas.CountAsync();

                // Obtener las nóminas de la base de datos con paginación
                var nominas = await _context.Nominas
                                            .Include(n => n.NominaEmpleados)
                                            .ThenInclude(ne => ne.Empleado) // Incluir los detalles del empleado
                                            .OrderBy(n => n.IDNomina) // Asegurarse de tener un orden para la paginación
                                            .Skip((pageNumber - 1) * pageSize) // Saltar los registros de las páginas anteriores
                                            .Take(pageSize) // Tomar solo la cantidad de registros que se necesitan para la página actual
                                            .ToListAsync();

                // Mapear las nóminas a DTOs
                return nominas.Select(nomina => new NominaDto
                {
                    IDNomina = nomina.IDNomina,
                    FechaPago = nomina.FechaPago,
                    TotalPagoGrupo = nomina.TotalPago,
                    TotalDeduccionesGrupo = nomina.Deducciones,
                    TotalBonoGrupos = nomina.Bonos, // Retornar el total de bonos
                    EmpleadosDetalles = nomina.NominaEmpleados.Select(ne => new EmpleadoDetalleDto
                    {
                        IDEmpleado = ne.IDEmpleado,
                        Nombre = ne.Empleado?.Nombre + " " + ne.Empleado?.Apellido, // Concatenar el nombre y el apellido
                        SalarioBase = ne.SalarioBase,
                        AFP = CalculateAFP(ne.SalarioBase), // Calcular la deducción de AFP
                        ARS = CalculateARS(ne.SalarioBase), // Calcular la deducción de ARS
                        ISR = CalculateISR(ne.SalarioBase), // Calcular la deducción de ISR
                        Deducciones = ne.Deducciones,
                        Bonos = ne.Bonos,
                        TotalPago = ne.TotalPago
                    }).ToList()
                });
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

        private decimal CalculateAFP(decimal salarioBase)
        {
            decimal afpMaximo = 102859.92m;
            return salarioBase <= afpMaximo ? salarioBase * 0.0287m : afpMaximo * 0.0287m;
        }

        private decimal CalculateARS(decimal salarioBase)
        {
            decimal arsMaximo = 102859.92m;
            return salarioBase <= arsMaximo ? salarioBase * 0.0304m : arsMaximo * 0.0304m;
        }

        private decimal CalculateISR(decimal salarioBase)
        {
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
            return isr;
        }

        public async Task<NominaResumenDTO> GetNominaResumenAsync()
        {
            var nominas = await _context.Nominas.ToListAsync();

            var totalNominas = nominas.Count;
            var totalPagar = nominas.Sum(n => n.TotalPago);

            return new NominaResumenDTO
            {
                TotalNominas = totalNominas,
                TotalPagar = totalPagar
            };
        }


        public async Task<NominaDto> AddEmpleadoToNominaAsync(int idNomina, NominaCreateDto nominaCreateDto)
        {
            try
            {
                // Buscar la nómina por su ID
                var nomina = await _context.Nominas
                    .Include(n => n.NominaEmpleados)
                    .FirstOrDefaultAsync(n => n.IDNomina == idNomina);

                if (nomina == null)
                {
                    throw new Exception($"Nómina con ID {idNomina} no encontrada.");
                }

                // Actualizar la fecha de pago si se proporciona una nueva
                if (nominaCreateDto.FechaPago != default(DateTime))
                {
                    nomina.FechaPago = AdjustPaymentDate(nominaCreateDto.FechaPago);
                }

                foreach (var empleadoBonoDto in nominaCreateDto.IDEmpleados)
                {
                    // Verificar si el empleado ya está en la nómina
                    if (nomina.NominaEmpleados.Any(ne => ne.IDEmpleado == empleadoBonoDto.IDEmpleado))
                    {
                        throw new Exception($"El empleado con ID {empleadoBonoDto.IDEmpleado} ya está en la nómina.");
                    }

                    // Buscar el empleado en la base de datos
                    var empleado = await _context.Empleados.FindAsync(empleadoBonoDto.IDEmpleado);
                    if (empleado == null)
                    {
                        throw new Exception($"Empleado con ID {empleadoBonoDto.IDEmpleado} no encontrado.");
                    }

                    // Calcular salario y deducciones
                    var salarioBase = empleado.Salario;
                    nomina.Deducciones = nominaCreateDto.DeduccionesExtras;
                    decimal deducciones = CalculateDeductions(salarioBase, nomina.Deducciones);
                    var totalPagoEmpleado = salarioBase + empleadoBonoDto.Bono - deducciones;

                    // Crear la entrada en la tabla intermedia NominaEmpleado
                    var nominaEmpleado = new NominaEmpleado
                    {
                        IDEmpleado = empleadoBonoDto.IDEmpleado,
                        IDNomina = nomina.IDNomina,
                        SalarioBase = salarioBase,
                        Deducciones = deducciones,
                        Bonos = empleadoBonoDto.Bono,
                        TotalPago = totalPagoEmpleado
                    };

                    // Agregar el empleado a la nómina
                    nomina.NominaEmpleados.Add(nominaEmpleado);
                    nomina.TotalPago += totalPagoEmpleado;
                    nomina.Bonos += empleadoBonoDto.Bono;
                }

                // Guardar los cambios en la base de datos
                await _context.SaveChangesAsync();

                // Devolver el DTO actualizado con los detalles de la nómina
                return new NominaDto
                {
                    IDNomina = nomina.IDNomina,
                    FechaPago = nomina.FechaPago,
                    TotalPagoGrupo = nomina.TotalPago,
                    TotalDeduccionesGrupo = nomina.Deducciones,
                    EmpleadosDetalles = nomina.NominaEmpleados.Select(ne => new EmpleadoDetalleDto
                    {
                        IDEmpleado = ne.IDEmpleado,
                        Nombre = ne.Empleado?.Nombre + " " + ne.Empleado?.Apellido, // Concatenar el nombre y el apellido
                        SalarioBase = ne.SalarioBase,
                        Deducciones = ne.Deducciones,
                        AFP = CalculateAFP(ne.SalarioBase), // Calcular la deducción de AFP
                        ARS = CalculateARS(ne.SalarioBase), // Calcular la deducción de ARS
                        ISR = CalculateISR(ne.SalarioBase), // Calcular la deducción de ISR
                        Bonos = ne.Bonos,
                        TotalPago = ne.TotalPago
                    }).ToList()
                };
            }
            catch (DbUpdateException ex)
            {
                var innerException = ex.InnerException?.Message;
                throw new Exception($"Error al actualizar la nómina: {innerException}");
            }
            catch (Exception ex)
            {
                throw new Exception($"Ocurrió un error inesperado: {ex.Message}");
            }
        }



        private DateTime AdjustPaymentDate(DateTime fechaPago)
        {
            // Si la fecha de pago es sábado (DayOfWeek = 6), se ajusta al viernes
            if (fechaPago.DayOfWeek == DayOfWeek.Saturday)
            {
                return fechaPago.AddDays(-1);
            }
            // Si la fecha de pago es domingo (DayOfWeek = 0), se ajusta al viernes
            else if (fechaPago.DayOfWeek == DayOfWeek.Sunday)
            {
                return fechaPago.AddDays(-2);
            }
            return fechaPago; // Si no cae en fin de semana, se deja la misma fecha
        }
    }
}
