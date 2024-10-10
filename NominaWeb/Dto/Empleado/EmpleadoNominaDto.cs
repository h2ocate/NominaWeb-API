namespace NominaWeb.Dto.Empleado
{
    public class EmpleadoNominaDto
    {
        public int Id { get; set; }
        public int IDEmpleado { get; set; }
        public decimal SalarioBase { get; set; }
        public decimal Deducciones { get; set; }
        public decimal Bonos { get; set; }
        public decimal TotalPago { get; set; }
    }
}
