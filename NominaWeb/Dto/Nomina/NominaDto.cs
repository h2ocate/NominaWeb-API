using NominaWeb.Dto.Empleado;

namespace NominaWeb.Dto.Nomina
{
    public class NominaDto
    {
        public int IDNomina { get; set; }
        public DateTime FechaPago { get; set; }
        public decimal TotalPagoGrupo { get; set; }
        public decimal TotalDeduccionesGrupo { get; set; }
        public List<EmpleadoDetalleDto> EmpleadosDetalles { get; set; }

    }

    public class EmpleadoDetalleDto
    {
        public int IDEmpleado { get; set; }
        public string Nombre { get; set; }
        public decimal SalarioBase { get; set; }
        public decimal Deducciones { get; set; }
        public decimal Bonos { get; set; }
        public decimal TotalPago { get; set; }
    }
}
