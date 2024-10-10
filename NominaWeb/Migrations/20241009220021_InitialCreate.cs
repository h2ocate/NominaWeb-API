using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NominaWeb.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Cargos",
                columns: table => new
                {
                    IDCargo = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NombreCargo = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cargos", x => x.IDCargo);
                });

            migrationBuilder.CreateTable(
                name: "Nominas",
                columns: table => new
                {
                    IDNomina = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FechaPago = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SalarioBase = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Deducciones = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Bonos = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TotalPago = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Nominas", x => x.IDNomina);
                });

            migrationBuilder.CreateTable(
                name: "Empleados",
                columns: table => new
                {
                    IDEmpleado = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Apellido = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    IDCargo = table.Column<int>(type: "int", nullable: false),
                    FechaIngreso = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Salario = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Estado = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Empleados", x => x.IDEmpleado);
                    table.ForeignKey(
                        name: "FK_Empleados_Cargos_IDCargo",
                        column: x => x.IDCargo,
                        principalTable: "Cargos",
                        principalColumn: "IDCargo",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "NominaEmpleados",
                columns: table => new
                {
                    IDNomina = table.Column<int>(type: "int", nullable: false),
                    IDEmpleado = table.Column<int>(type: "int", nullable: false),
                    Id = table.Column<int>(type: "int", nullable: false),
                    SalarioBase = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Deducciones = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Bonos = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TotalPago = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    EmpleadoIDEmpleado = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NominaEmpleados", x => new { x.IDNomina, x.IDEmpleado });
                    table.ForeignKey(
                        name: "FK_NominaEmpleados_Empleados_EmpleadoIDEmpleado",
                        column: x => x.EmpleadoIDEmpleado,
                        principalTable: "Empleados",
                        principalColumn: "IDEmpleado");
                    table.ForeignKey(
                        name: "FK_NominaEmpleados_Empleados_IDEmpleado",
                        column: x => x.IDEmpleado,
                        principalTable: "Empleados",
                        principalColumn: "IDEmpleado",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_NominaEmpleados_Nominas_IDNomina",
                        column: x => x.IDNomina,
                        principalTable: "Nominas",
                        principalColumn: "IDNomina",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Empleados_IDCargo",
                table: "Empleados",
                column: "IDCargo");

            migrationBuilder.CreateIndex(
                name: "IX_NominaEmpleados_EmpleadoIDEmpleado",
                table: "NominaEmpleados",
                column: "EmpleadoIDEmpleado");

            migrationBuilder.CreateIndex(
                name: "IX_NominaEmpleados_IDEmpleado",
                table: "NominaEmpleados",
                column: "IDEmpleado");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "NominaEmpleados");

            migrationBuilder.DropTable(
                name: "Empleados");

            migrationBuilder.DropTable(
                name: "Nominas");

            migrationBuilder.DropTable(
                name: "Cargos");
        }
    }
}
