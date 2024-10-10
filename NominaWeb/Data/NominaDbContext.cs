using Microsoft.EntityFrameworkCore;
using NominaWeb.Models;

namespace NominaWeb.Data
{
    public class NominaDbContext : DbContext
    {
        public NominaDbContext(DbContextOptions<NominaDbContext> options)
            : base(options) { }

        public DbSet<Empleado> Empleados { get; set; }
        public DbSet<Cargo> Cargos { get; set; }
        public DbSet<Nominas> Nominas { get; set; }
    public DbSet<NominaEmpleado> NominaEmpleados { get; set; } // Aquí está


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Empleado>()
                .Property(e => e.Salario)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<NominaEmpleado>()
                .Property(n => n.SalarioBase)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<NominaEmpleado>()
                .Property(n => n.Deducciones)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<NominaEmpleado>()
                .Property(n => n.Bonos)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<NominaEmpleado>()
                .Property(n => n.TotalPago)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Nominas>()
                .Property(n => n.Deducciones)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Nominas>()
                .Property(n => n.TotalPago)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Empleado>()
                .HasOne(e => e.Cargo)
                .WithMany(c => c.Empleados)
                .HasForeignKey(e => e.IDCargo);

            // Definición de clave primaria compuesta
            modelBuilder.Entity<NominaEmpleado>()
                .HasKey(ne => new { ne.IDNomina, ne.IDEmpleado });

            // Configuración de claves foráneas
            modelBuilder.Entity<NominaEmpleado>()
                .HasOne(ne => ne.Nomina)
                .WithMany(n => n.NominaEmpleados)
                .HasForeignKey(ne => ne.IDNomina)
                .OnDelete(DeleteBehavior.Restrict); // Cambiar de Cascade a Restrict

            modelBuilder.Entity<NominaEmpleado>()
                .HasOne(ne => ne.Empleado)
                .WithMany() // Si tienes una colección en Empleado, colócala aquí
                .HasForeignKey(ne => ne.IDEmpleado)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
