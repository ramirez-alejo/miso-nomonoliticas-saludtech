using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ingestion.Aplicacion.Anonimizacion.Persistencia.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AnonimizacionImagenes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Version = table.Column<string>(type: "text", nullable: false),
                    ImagenId = table.Column<Guid>(type: "uuid", nullable: false),
                    DetalleAnonimizacion = table.Column<string>(type: "text", nullable: false),
                    NombreModalidad = table.Column<string>(type: "text", nullable: true),
                    DescripcionModalidad = table.Column<string>(type: "text", nullable: true),
                    RegionAnatomica = table.Column<string>(type: "text", nullable: true),
                    DescripcionRegionAnatomica = table.Column<string>(type: "text", nullable: true),
                    Resolucion = table.Column<string>(type: "text", nullable: true),
                    Contraste = table.Column<string>(type: "text", nullable: true),
                    Es3D = table.Column<bool>(type: "boolean", nullable: false),
                    FaseEscaner = table.Column<string>(type: "text", nullable: true),
                    UbicacionImagen = table.Column<string>(type: "text", nullable: true),
                    UbicacionImagenProcesada = table.Column<string>(type: "text", nullable: true),
                    Timestamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AnonimizacionImagenes", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AnonimizacionImagenes");
        }
    }
}
