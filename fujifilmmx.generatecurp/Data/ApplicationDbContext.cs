using Microsoft.EntityFrameworkCore;
using fujifilmmx.generatecurp.Models;
using System.Collections.Generic;

namespace fujifilmmx.generatecurp.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Paciente> Pacientes { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuraciones del modelo aquí
            // Asegúrate de no realizar consultas a la base de datos aquí
        }
    }
}