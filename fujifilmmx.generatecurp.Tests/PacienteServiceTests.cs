using fujifilmmx.generatecurp.Models;
using fujifilmmx.generatecurp.Services;
using fujifilmmx.generatecurp.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;
using System;
using fujifilmmx.generatecurp.DTOs;
using fujifilmmx.generatecurp.Controllers;

namespace fujifilmmx.generatecurp.Tests
{
    public class PacienteServiceTests : IDisposable
    {
        private readonly PacienteService _pacienteService;
        private readonly ApplicationDbContext _context;
        private readonly CurpIDController _curpIDController;

        public PacienteServiceTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            _context = new ApplicationDbContext(options);
            _pacienteService = new PacienteService(_context, new NullLogger<PacienteService>());
            _curpIDController = new CurpIDController(_pacienteService, new NullLogger<CurpIDController>());
        }
        
        [Fact]
        public void Database_ShouldBeEmpty_AtStartOfEachTest()
        {
            // Assert
            Assert.Empty(_context.Pacientes);
        }

        [Fact]
        public void AddOrSelectPersonControler_ShouldReturnCurp_WhenPacienteIsValid()
        {
            // Arrange
            var paciente = new PacienteDto
            {
                idHIS = "123",
                Nombre = "Juan",
                ApellidoPaterno = "Perez",
                ApellidoMaterno = "Lopez",
                Genero = "M",
                EstadoNacimiento = "01",
                FechaNacimiento = "19000101",
                origen = "TestLocation",
            };

            // Act
            var result = _curpIDController.Post(paciente);

            // Assert
            Assert.NotNull(result);
            Assert.Single(_context.Pacientes);
            Assert.Equal("PELJ000101HASRPN00", _context.Pacientes.First().CURP);
        }

        [Fact]
        public void AddOrSelectPerson_ShouldReturnCurp_WhenPacienteIsValid()
        {
            // Arrange
            var paciente = new Paciente
            {
                idHIS = "123",
                Nombre = "Juan",
                ApellidoPaterno = "Perez",
                ApellidoMaterno = "Lopez",
                Genero = "M",
                EstadoNacimiento = 1,
                FechaNacimiento = new DateOnly(1990, 1, 1),
                origen = "TestLocation",
            };

            // Act
            var result = _pacienteService.AddOrSelectPerson(paciente);

            // Assert
            Assert.NotNull(result);
            Assert.Single(_context.Pacientes);
        }

        [Fact]
        public void AddOrSelectPerson_ShouldThrowArgumentException_WhenPacienteIsInvalid()
        {
            // Arrange
            var paciente = new Paciente
            {
                idHIS = "123",
                Nombre = "",
                ApellidoPaterno = "Perez",
                ApellidoMaterno = "Lopez",
                Genero = "M",
                EstadoNacimiento = 1,
                FechaNacimiento = new DateOnly(1990, 1, 1),
                origen = "TestLocation",
            };

            // Act & Assert
            Assert.Throws<ArgumentException>(() => _pacienteService.AddOrSelectPerson(paciente));
        }

        [Fact]
        public void AddOrSelectPerson_ShouldReturnExistingCurp_WhenPacienteDataMatches()
        {
            // Arrange
            var existingPaciente = new Paciente
            {
                idHIS = "123",
                CURP = "PELJ900101HASRPN04",
                Nombre = "Juan",
                ApellidoPaterno = "Perez",
                ApellidoMaterno = "Lopez",
                Genero = "M",
                EstadoNacimiento = 1,
                FechaNacimiento = new DateOnly(1990, 1, 1),
                intVeces = 1,
                origen = "TestLocation",
            };

            _context.Pacientes.Add(existingPaciente);
            _context.SaveChanges();

            var paciente = new Paciente
            {
                idHIS = "123",
                Nombre = "Juan",
                ApellidoPaterno = "Perez",
                ApellidoMaterno = "Lopez",
                Genero = "M",
                EstadoNacimiento = 1,
                FechaNacimiento = new DateOnly(1990, 1, 1),
                origen = "TestLocation",
            };

            // Act
            var result = _pacienteService.AddOrSelectPerson(paciente);

            // Assert
            Assert.Equal("PELJ900101HASRPN04", result);
        }

        [Fact]
        public void AddOrSelectPerson_ShouldIncrementIntVeces_WhenCurpExistsButDataDiffers()
        {
            // Arrange
            var existingPaciente = new Paciente
            {
                idHIS = "123",
                CURP = "PELJ900101HASRPN04",
                Nombre = "Juan",
                ApellidoPaterno = "Perez",
                ApellidoMaterno = "Lopez",
                Genero = "M",
                EstadoNacimiento = 1,
                FechaNacimiento = new DateOnly(1990, 1, 1),
                intVeces = 1,
                origen = "TestLocation",
            };

            _context.Pacientes.Add(existingPaciente);
            _context.SaveChanges();

            var paciente = new Paciente
            {
                idHIS = "123",
                Nombre = "Juan",
                ApellidoPaterno = "Perezz",
                ApellidoMaterno = "Lopez",
                Genero = "M",
                EstadoNacimiento = 1,
                FechaNacimiento = new DateOnly(1990, 1, 1),
                origen = "TestLocation",
            };

            // Act
            var result = _pacienteService.AddOrSelectPerson(paciente);

            // Assert
            Assert.Equal("PELJ900101HASRPN04_2", result);
            Assert.Equal(2, _context.Pacientes.Count());
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

    }
}