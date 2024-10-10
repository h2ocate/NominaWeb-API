using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NominaWeb.Models
{
    public class Nominas
    {
        [Key]
        public int IDNomina { get; set; }

        [Required]
        public DateTime FechaPago { get; set; }

        [Required]
        [Range(0, double.MaxValue)]
        public decimal SalarioBase { get; set; }

        [Required]
        [Range(0, double.MaxValue)]
        public decimal Deducciones { get; set; }

        [Range(0, double.MaxValue)]
        public decimal Bonos { get; set; }

        [Required]
        [Range(0, double.MaxValue)]
        public decimal TotalPago { get; set; }

        public virtual ICollection<NominaEmpleado> NominaEmpleados { get; set; } = new List<NominaEmpleado>();

    }
}
