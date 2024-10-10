using System.ComponentModel.DataAnnotations;

namespace NominaWeb.Models
{
    public class Cargo
    {
        [Key] // Marca esta propiedad como la clave primaria.
        public int IDCargo { get; set; }

        [Required(ErrorMessage = "El nombre del cargo es obligatorio.")]
        [StringLength(100, ErrorMessage = "El nombre del cargo no puede exceder los 100 caracteres.")]
        public string NombreCargo { get; set; }

        // Relación con Empleado
        public ICollection<Empleado> Empleados { get; set; } // Para navegar a los empleados relacionados
    }
}
