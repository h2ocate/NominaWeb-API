using Microsoft.AspNetCore.Mvc;
using NominaWeb.Dto.Nomina;
using NominaWeb.Interfaces;

namespace NominaWeb.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NominaController : ControllerBase
    {
        private readonly INominaService _nominaService;

        public NominaController(INominaService nominaService)
        {
            _nominaService = nominaService;
        }

        [HttpPost("GuardarNomina")]
        public async Task<IActionResult> CreateNomina([FromBody] NominaCreateDto nominaDto)
        {
            // Verifica que la lista de empleados no esté vacía
            if (nominaDto.IDEmpleados == null || !nominaDto.IDEmpleados.Any())
            {
                return BadRequest("La lista de empleados no puede estar vacía.");
            }

            try
            {
                // Llama al servicio para agregar la nómina
                var nomina = await _nominaService.AddNominaAsync(nominaDto);

                // Devuelve una respuesta 201 Created con la información de la nómina
                return CreatedAtAction(nameof(GetNominaById), new { id = nomina.IDNomina }, nomina);
            }
            catch (Exception ex)
            {
                // Maneja errores y devuelve un mensaje adecuado
                return BadRequest($"Error al crear la nómina: {ex.Message}");
            }
        }



        [HttpGet("{id}")]
        public async Task<IActionResult> GetNominaById(int id)
        {
            var nomina = await _nominaService.GetNominaByIdAsync(id);
            return Ok(nomina);
        }

        [HttpGet]
        public async Task<IActionResult> GetNominas(int pageNumber = 1, int pageSize = 2)
        {
            var nominas = await _nominaService.GetAllNominasAsync(pageNumber, pageSize);
            return Ok(nominas);
        }

        [HttpPut("Actualizar/{id}")]
        public async Task<IActionResult> UpdateNomina(int id, [FromBody] NominaCreateDto nominaDto)
        {
            await _nominaService.UpdateNominaAsync(id, nominaDto);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeactivateNomina(int id)
        {
            await _nominaService.DeactivateNominaAsync(id);
            return NoContent();
        }

        [HttpGet("resumen")]
        public async Task<ActionResult<NominaResumenDTO>> GetNominaResumen()
        {
            try
            {
                var resumen = await _nominaService.GetNominaResumenAsync();
                return Ok(resumen);
            }
            catch (Exception ex)
            {
                // Manejo de errores
                return StatusCode(500, "Error al obtener el resumen de nóminas");
            }
        }
    }
}
