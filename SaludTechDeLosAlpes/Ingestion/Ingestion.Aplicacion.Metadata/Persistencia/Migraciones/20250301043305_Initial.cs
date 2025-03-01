using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ingestion.Aplicacion.Metadata.Persistencia.Migraciones
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Imagenes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Version = table.Column<string>(type: "text", nullable: true),
                    VersionServicio = table.Column<string>(type: "text", nullable: true),
                    ImagenId = table.Column<Guid>(type: "uuid", nullable: false),
                    NombreModalidad = table.Column<string>(type: "text", nullable: true),
                    DescripcionModalidad = table.Column<string>(type: "text", nullable: true),
                    RegionAnatomica = table.Column<string>(type: "text", nullable: true),
                    DescripcionRegionAnatomica = table.Column<string>(type: "text", nullable: true),
                    DescripcionPatologia = table.Column<string>(type: "text", nullable: true),
                    Resolucion = table.Column<string>(type: "text", nullable: true),
                    Contraste = table.Column<string>(type: "text", nullable: true),
                    Es3D = table.Column<bool>(type: "boolean", nullable: false),
                    FaseEscaner = table.Column<string>(type: "text", nullable: true),
                    EtapaContextoProcesal = table.Column<string>(type: "text", nullable: true),
                    GrupoEdad = table.Column<string>(type: "text", nullable: true),
                    Sexo = table.Column<string>(type: "text", nullable: true),
                    Etnicidad = table.Column<string>(type: "text", nullable: true),
                    Fumador = table.Column<bool>(type: "boolean", nullable: false),
                    Diabetico = table.Column<bool>(type: "boolean", nullable: false),
                    CondicionesPrevias = table.Column<string>(type: "text", nullable: true),
                    TipoAmbiente = table.Column<string>(type: "text", nullable: true),
                    Sintomas = table.Column<string>(type: "text", nullable: true),
                    Timestamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Imagenes", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Imagenes_ImagenId",
                table: "Imagenes",
                column: "ImagenId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Imagenes");
        }
    }
}
