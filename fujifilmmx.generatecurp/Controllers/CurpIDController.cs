using fujifilmmx.generatecurp.DTOs;
using fujifilmmx.generatecurp.Models;
using fujifilmmx.generatecurp.Services;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using System.Text.Json;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace fujifilmmx.generatecurp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CurpIDController : ControllerBase
    {
        private readonly PacienteService _pacienteService;
        private readonly ILogger<CurpIDController> _logger;
        public CurpIDController(PacienteService pacienteService, ILogger<CurpIDController> logger)
        {
            _pacienteService = pacienteService;
            _logger = logger;
        }

        // POST api/<CurpIDController>
        [HttpPost]
        public ActionResult<ResponseModel> Post([FromBody] PacienteDto pacienteDto)
        {
            _logger.LogInformation($"Procesando solicitud DTO para crear o seleccionar persona. {JsonSerializer.Serialize(pacienteDto)}");

            var response = new ResponseModel();

            try
            {
                var paciente = new Paciente
                {
                    idHIS = pacienteDto.idHIS,
                    Nombre = pacienteDto.Nombre,
                    ApellidoPaterno = pacienteDto.ApellidoPaterno,
                    ApellidoMaterno = pacienteDto.ApellidoMaterno,
                    Genero = pacienteDto.Genero,
                    EstadoNacimiento = int.Parse(pacienteDto.EstadoNacimiento),
                    FechaNacimiento = DateOnly.ParseExact(pacienteDto.FechaNacimiento, "yyyyMMdd", CultureInfo.InvariantCulture),
                    origen = pacienteDto.origen,
                };
                response.CURP = _pacienteService.AddOrSelectPerson(paciente);
                response.hasError = false;
                response.ErrorMessage = string.Empty;
                // Serializar el objeto Paciente a JSON y escribirlo en el log
                var pacienteJson = JsonSerializer.Serialize(paciente);
                var curp = _pacienteService.AddOrSelectPerson(paciente);
            }
            catch (ArgumentException ex)
            {
                _logger.LogError(ex, "Error al procesar la solicitud.");
                response.hasError = true;
                response.ErrorMessage = ex.Message;
                return BadRequest(response);
            }
            return Ok(response);
        }
    }
}
