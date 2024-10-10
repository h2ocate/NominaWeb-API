using Microsoft.AspNetCore.Mvc;
using NominaWeb.Dto.Empleado;
using NominaWeb.Interfaces;
using NominaWeb.Models;

namespace NominaWeb.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmpleadosController : ControllerBase
    {
        private readonly IEmpleadoService _empleadoService;

        public EmpleadosController(IEmpleadoService empleadoService)
        {
            _empleadoService = empleadoService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<EmpleadoDTO>>> GetAll()
        {
            var empleados = await _empleadoService.GetAllEmpleadosAsync();
            return Ok(empleados);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<EmpleadoDTO>> GetById(int id)
        {
            var empleado = await _empleadoService.GetEmpleadoByIdAsync(id);
            if (empleado == null)
            {
                return NotFound();
            }

            return Ok(empleado);
        }

        [HttpPost("GuardarEmpleado")]
        public async Task<ActionResult<EmpleadoDTO>> Create([FromBody] CreateEmpleadoDTO createEmpleadoDto)
            {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var empleado = await _empleadoService.CreateEmpleadoAsync(createEmpleadoDto);
            return CreatedAtAction(nameof(GetById), new { id = empleado.IDEmpleado }, empleado);
        }
        [HttpPut("Actualizar/{Id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateEmpleadoDTO updateEmpleadoDto)
        {
            try
            {
                // Verifica si el ID del empleado en el cuerpo de la solicitud coincide con el ID de la URL
                if (id != updateEmpleadoDto.IDEmpleado)
                {
                    return BadRequest(new { mensaje = "El ID del empleado no coincide." });
                }

                // Intenta actualizar el empleado
                var updatedEmpleado = await _empleadoService.UpdateEmpleadoAsync(updateEmpleadoDto);

                // Verifica si la actualización fue exitosa
                if (updatedEmpleado == null)
                {
                    return NotFound(new { mensaje = "Empleado no encontrado." });
                }

                // Devuelve un código de éxito
                return NoContent();
            }
            catch (Exception ex)
            {

                return StatusCode(500, new { mensaje = "Error interno del servidor", detalle = ex.Message });
            }
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _empleadoService.DeleteEmpleadoAsync(id);
            if (!result)
            {
                return NotFound();
            }

            return NoContent();
        }
        [HttpGet("GetCargos")]
        public async Task<ActionResult<IEnumerable<Cargo>>> GetCargos()
        {
            var cargos = await _empleadoService.GetCargosAsync(); // Llama al servicio para obtener cargos
            return Ok(cargos); // Devuelve la lista de cargos
        }
    }
}
