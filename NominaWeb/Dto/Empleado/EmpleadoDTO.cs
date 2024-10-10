using System;
using System.ComponentModel.DataAnnotations;

namespace NominaWeb.Dto.Empleado
{
    public class EmpleadoDTO
    {
        public int IDEmpleado { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }

        // Cambiar de 'Cargo' a 'IDCargo' para usar la FK
        public int IDCargo { get; set; }  // ID del cargo relacionado

        public DateTime FechaIngreso { get; set; }
        public decimal Salario { get; set; }
        public bool Estado { get; set; }
    }

    public class CreateEmpleadoDTO
    {
        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [StringLength(50, ErrorMessage = "El nombre no puede exceder los 50 caracteres.")]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "El apellido es obligatorio.")]
        [StringLength(50, ErrorMessage = "El apellido no puede exceder los 50 caracteres.")]
        public string Apellido { get; set; }

        [Required(ErrorMessage = "El ID del cargo es obligatorio.")]
        public int IDCargo { get; set; }  // ID del cargo relacionado

        [Required(ErrorMessage = "La fecha de ingreso es obligatoria.")]
        [DataType(DataType.Date, ErrorMessage = "Formato de fecha no válido.")]
        public DateTime FechaIngreso { get; set; }

        [Required(ErrorMessage = "El salario es obligatorio.")]
        [Range(0, double.MaxValue, ErrorMessage = "El salario debe ser un valor positivo.")]
        public decimal Salario { get; set; }

        public bool Estado { get; set; } = true; // Activo por defecto
    }

    public class UpdateEmpleadoDTO : CreateEmpleadoDTO
    {
        public int IDEmpleado { get; set; }
    }
}
