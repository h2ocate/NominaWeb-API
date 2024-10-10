using NominaWeb.Dto.Empleado;
using System.ComponentModel.DataAnnotations;

namespace NominaWeb.Dto.Nomina
{
    public class NominaCreateDto
    {
        public int IDNomina { get; set; } // Clave primaria, debe ser Identity
        public DateTime FechaPago { get; set; } // Fecha de pago
        public decimal DeduccionesExtras { get; set; } // Deducciones adicionales, si las hay
        public List<EmpleadoBonosDto> IDEmpleados { get; set; } // Lista de empleados con sus bonos
    }

}
