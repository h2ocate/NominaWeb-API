
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NominaWeb.Models
{
    public class NominaEmpleado
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] // Ensures Id is auto-incremented
        public int Id { get; set; }
        public int IDNomina { get; set; }
        public Nominas Nomina { get; set; }

        public int IDEmpleado { get; set; }
        public Empleado Empleado { get; set; }

        public decimal SalarioBase { get; set; }
        public decimal Deducciones { get; set; }
        public decimal Bonos { get; set; }
        public decimal TotalPago { get; set; }
    }

}
