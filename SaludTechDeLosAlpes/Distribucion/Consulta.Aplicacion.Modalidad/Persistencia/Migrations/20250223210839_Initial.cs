using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Consulta.Aplicacion.Modalidad.Persistencia.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ImagenesModalidad",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ImagenId = table.Column<Guid>(type: "uuid", nullable: false),
                    Nombre = table.Column<string>(type: "text", nullable: false),
                    Descripcion = table.Column<string>(type: "text", nullable: false),
                    RegionAnatomica = table.Column<string>(type: "text", nullable: false),
                    RegionDescripcion = table.Column<string>(type: "text", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ImagenesModalidad", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ImagenesModalidad_ImagenId",
                table: "ImagenesModalidad",
                column: "ImagenId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ImagenesModalidad");
        }
    }
}
