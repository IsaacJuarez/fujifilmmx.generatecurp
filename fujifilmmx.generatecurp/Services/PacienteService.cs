using fujifilmmx.generatecurp.Models;
using CURP;
using CURP.Enums;
using Microsoft.EntityFrameworkCore;
using fujifilmmx.generatecurp.Data;
using System.Text.Json;

namespace fujifilmmx.generatecurp.Services
{
    public class PacienteService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<PacienteService> _logger;

        public PacienteService(ApplicationDbContext context, ILogger<PacienteService> logger)
        {
            _context = context;
            _logger = logger;
        }
        public string AddOrSelectPerson(Paciente paciente)
        {
            // Validar los datos del paciente
            if (string.IsNullOrWhiteSpace(paciente.Nombre) ||
                string.IsNullOrWhiteSpace(paciente.ApellidoPaterno) ||
                string.IsNullOrWhiteSpace(paciente.ApellidoMaterno) ||
                string.IsNullOrWhiteSpace(paciente.Genero) )
            {
                _logger.LogWarning("Datos del paciente no válidos.");
                throw new ArgumentException("Datos del paciente no válidos.");
            }

            var curp = GetCurp(paciente);
            _logger.LogInformation($"CURP generada: {curp}");

            var existingCURP = _context.Pacientes.FirstOrDefault(p =>
                p.CURP == curp);

            if (existingCURP != null)
            {
                // Validamos si los datos de del paciente existingCurp son iguales a los del modelo paciente
                if (ArePacienteDataEqual(existingCURP, paciente))
                {
                    _logger.LogInformation($"los datos de de la persona existingCurp son iguales a los del modelo persona: {JsonSerializer.Serialize(existingCURP)}");
                    return FormatCurp(existingCURP.CURP, existingCURP.intVeces);
                }
                else
                {
                    // validamos si el paciente ya existe en base de datos por los datos del paciente y no por curp
                    var existingPaciente = _context.Pacientes.FirstOrDefault(p =>
                        p.Nombre == paciente.Nombre &&
                        p.ApellidoPaterno == paciente.ApellidoPaterno &&
                        p.ApellidoMaterno == paciente.ApellidoMaterno &&
                        p.Genero == paciente.Genero &&
                        p.EstadoNacimiento == paciente.EstadoNacimiento &&
                        p.FechaNacimiento == paciente.FechaNacimiento);

                    if (existingPaciente != null)
                    {
                        _logger.LogInformation($"la persona ya existe en base de datos por los datos demograficos y no por curp: {JsonSerializer.Serialize(existingPaciente)}");
                        return FormatCurp(existingPaciente.CURP, existingPaciente.intVeces);
                    }
                    else
                    {
                        // Contar el número de registros con el mismo CURP
                        var curpCount = _context.Pacientes.Count(p => p.CURP == curp);
                        curpCount += 1;

                        // Insertar un nuevo registro con los datos actualizados
                        insertPatient(paciente, curp, curpCount);

                        // si existe mas de una vez, regresamos la curp asignada con el contador
                        return FormatCurp(existingCURP.CURP, curpCount);
                    }
                }
            }
            else
            {
                insertPatient(paciente, curp, 1);
                return FormatCurp(curp, 1);
            }
        }


        private bool insertPatient(Paciente paciente, string curp ,int veces)
        {
            _logger.LogInformation($"insertPatient( {JsonSerializer.Serialize(paciente)},{curp},{veces}");
            // si no existe la curp, la creamos
            paciente.CURP = curp;
            paciente.datInserta = DateTime.Now;
            paciente.bitActivo = true;
            paciente.intVeces = veces;

            try
            {
                _context.Pacientes.Add(paciente);
                _context.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogInformation($"insertPatientEx({ex.Message})");
                return false;
            }
        }

        private bool ArePacienteDataEqual(Paciente existingPaciente, Paciente newPaciente)
        {
            return existingPaciente.Nombre == newPaciente.Nombre &&
                   existingPaciente.ApellidoPaterno == newPaciente.ApellidoPaterno &&
                   existingPaciente.ApellidoMaterno == newPaciente.ApellidoMaterno &&
                   existingPaciente.Genero == newPaciente.Genero &&
                   existingPaciente.EstadoNacimiento == newPaciente.EstadoNacimiento &&
                   existingPaciente.FechaNacimiento == newPaciente.FechaNacimiento;
        }

        private string FormatCurp(string curp, int count)
        {
            _logger.LogInformation($"FormatCurp({curp},{count})");
            var curpArregada = count == 1 ? curp : $"{curp}_{count}";
            _logger.LogInformation($"FormatCurpReturn({curpArregada})");
            return curpArregada;
        }

        public string GetCurp(Paciente paciente)
        {
            var sexo = paciente.Genero == "M" ? Sexo.Hombre : Sexo.Mujer;

            var estado = Estado.Extranjero;

            switch (paciente.EstadoNacimiento)
            {
                case 01:
                    estado = Estado.Aguascalientes;
                    break;
                case 02:
                    estado = Estado.Baja_California;
                    break;
                case 03:
                    estado = Estado.Baja_California_Sur;
                    break;
                case 04:
                    estado = Estado.Campeche;
                    break;
                case 05:
                    estado = Estado.Coahuila;
                    break;
                case 06:
                    estado = Estado.Colima;
                    break;
                case 07:
                    estado = Estado.Chiapas;
                    break;
                case 08:
                    estado = Estado.Chihuahua;
                    break;
                case 09:
                    estado = Estado.Distrito_Federal;
                    break;
                case 10:
                    estado = Estado.Durango;
                    break;
                case 11:
                    estado = Estado.Guanajuato;
                    break;
                case 12:
                    estado = Estado.Guerrero;
                    break;
                case 13:
                    estado = Estado.Hidalgo;
                    break;
                case 14:
                    estado = Estado.Jalisco;
                    break;
                case 15:
                    estado = Estado.Mexico;
                    break;
                case 16:
                    estado = Estado.Michoacan;
                    break;
                case 17:
                    estado = Estado.Morelos;
                    break;
                case 18:
                    estado = Estado.Nayarit;
                    break;
                case 19:
                    estado = Estado.Nuevo_Leon;
                    break;
                case 20:
                    estado = Estado.Oaxaca;
                    break;
                case 21:
                    estado = Estado.Puebla;
                    break;
                case 22:
                    estado = Estado.Queretaro;
                    break;
                case 23:
                    estado = Estado.Quintana_Roo;
                    break;
                case 24:
                    estado = Estado.San_Luis_Potosi;
                    break;
                case 25:
                    estado = Estado.Sinaloa;
                    break;
                case 26:
                    estado = Estado.Sonora;
                    break;
                case 27:
                    estado = Estado.Tabasco;
                    break;
                case 28:
                    estado = Estado.Tamaulipas;
                    break;
                case 29:
                    estado = Estado.Tlaxcala;
                    break;
                case 30:
                    estado = Estado.Veracruz;
                    break;
                case 31:
                    estado = Estado.Yucatan;
                    break;
                case 32:
                    estado = Estado.Zacatecas;
                    break;
                default:
                    estado = Estado.Extranjero;
                    break;
            }

            return Curp.Generar(paciente.Nombre, paciente.ApellidoPaterno, paciente.ApellidoMaterno, sexo, paciente.FechaNacimiento.ToDateTime(TimeOnly.MinValue), estado);
        }
    }
}
