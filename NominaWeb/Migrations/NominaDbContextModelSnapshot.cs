﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using NominaWeb.Data;

#nullable disable

namespace NominaWeb.Migrations
{
    [DbContext(typeof(NominaDbContext))]
    partial class NominaDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.8")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("NominaWeb.Models.Cargo", b =>
                {
                    b.Property<int>("IDCargo")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("IDCargo"));

                    b.Property<string>("NombreCargo")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.HasKey("IDCargo");

                    b.ToTable("Cargos");
                });

            modelBuilder.Entity("NominaWeb.Models.Empleado", b =>
                {
                    b.Property<int>("IDEmpleado")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("IDEmpleado"));

                    b.Property<string>("Apellido")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<bool>("Estado")
                        .HasColumnType("bit");

                    b.Property<DateTime>("FechaIngreso")
                        .HasColumnType("datetime2");

                    b.Property<int>("IDCargo")
                        .HasColumnType("int");

                    b.Property<string>("Nombre")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<decimal>("Salario")
                        .HasColumnType("decimal(18,2)");

                    b.HasKey("IDEmpleado");

                    b.HasIndex("IDCargo");

                    b.ToTable("Empleados");
                });

            modelBuilder.Entity("NominaWeb.Models.NominaEmpleado", b =>
                {
                    b.Property<int>("IDNomina")
                        .HasColumnType("int");

                    b.Property<int>("IDEmpleado")
                        .HasColumnType("int");

                    b.Property<decimal>("Bonos")
                        .HasColumnType("decimal(18,2)");

                    b.Property<decimal>("Deducciones")
                        .HasColumnType("decimal(18,2)");

                    b.Property<int?>("EmpleadoIDEmpleado")
                        .HasColumnType("int");

                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<decimal>("SalarioBase")
                        .HasColumnType("decimal(18,2)");

                    b.Property<decimal>("TotalPago")
                        .HasColumnType("decimal(18,2)");

                    b.HasKey("IDNomina", "IDEmpleado");

                    b.HasIndex("EmpleadoIDEmpleado");

                    b.HasIndex("IDEmpleado");

                    b.ToTable("NominaEmpleados");
                });

            modelBuilder.Entity("NominaWeb.Models.Nominas", b =>
                {
                    b.Property<int>("IDNomina")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("IDNomina"));

                    b.Property<decimal>("Bonos")
                        .HasColumnType("decimal(18,2)");

                    b.Property<decimal>("Deducciones")
                        .HasColumnType("decimal(18,2)");

                    b.Property<DateTime>("FechaPago")
                        .HasColumnType("datetime2");

                    b.Property<decimal>("SalarioBase")
                        .HasColumnType("decimal(18,2)");

                    b.Property<decimal>("TotalPago")
                        .HasColumnType("decimal(18,2)");

                    b.HasKey("IDNomina");

                    b.ToTable("Nominas");
                });

            modelBuilder.Entity("NominaWeb.Models.Usuario.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<bool>("IsActive")
                        .HasColumnType("bit");

                    b.Property<string>("PasswordHash")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<int>("Role")
                        .HasColumnType("int");

                    b.Property<string>("Username")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.HasKey("Id");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("NominaWeb.Models.Empleado", b =>
                {
                    b.HasOne("NominaWeb.Models.Cargo", "Cargo")
                        .WithMany("Empleados")
                        .HasForeignKey("IDCargo")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Cargo");
                });

            modelBuilder.Entity("NominaWeb.Models.NominaEmpleado", b =>
                {
                    b.HasOne("NominaWeb.Models.Empleado", null)
                        .WithMany("NominaEmpleados")
                        .HasForeignKey("EmpleadoIDEmpleado");

                    b.HasOne("NominaWeb.Models.Empleado", "Empleado")
                        .WithMany()
                        .HasForeignKey("IDEmpleado")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("NominaWeb.Models.Nominas", "Nomina")
                        .WithMany("NominaEmpleados")
                        .HasForeignKey("IDNomina")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Empleado");

                    b.Navigation("Nomina");
                });

            modelBuilder.Entity("NominaWeb.Models.Cargo", b =>
                {
                    b.Navigation("Empleados");
                });

            modelBuilder.Entity("NominaWeb.Models.Empleado", b =>
                {
                    b.Navigation("NominaEmpleados");
                });

            modelBuilder.Entity("NominaWeb.Models.Nominas", b =>
                {
                    b.Navigation("NominaEmpleados");
                });
#pragma warning restore 612, 618
        }
    }
}
