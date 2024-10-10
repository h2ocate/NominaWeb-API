using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NominaWeb.Models
{
    public class Empleado
    {
        [Key] // Marca esta propiedad como la clave primaria.
        public int IDEmpleado { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [StringLength(50, ErrorMessage = "El nombre no puede exceder los 50 caracteres.")]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "El apellido es obligatorio.")]
        [StringLength(50, ErrorMessage = "El apellido no puede exceder los 50 caracteres.")]
        public string Apellido { get; set; }

        [Required(ErrorMessage = "El cargo es obligatorio.")]
        public int IDCargo { get; set; } // Clave foránea para Cargo

        // Propiedad de navegación para Cargo
        [ForeignKey("IDCargo")]
        public Cargo Cargo { get; set; } // Relación con la tabla Cargo

        [Required(ErrorMessage = "La fecha de ingreso es obligatoria.")]
        [DataType(DataType.Date, ErrorMessage = "Formato de fecha no válido.")]
        public DateTime FechaIngreso { get; set; }

        [Required(ErrorMessage = "El salario es obligatorio.")]
        [Range(0, double.MaxValue, ErrorMessage = "El salario debe ser un valor positivo.")]
        public decimal Salario { get; set; }

        [Required]
        public bool Estado { get; set; } // Estado para eliminación lógica (activo/inactivo).

        public virtual ICollection<NominaEmpleado> NominaEmpleados { get; set; } = new List<NominaEmpleado>();

    }

}
