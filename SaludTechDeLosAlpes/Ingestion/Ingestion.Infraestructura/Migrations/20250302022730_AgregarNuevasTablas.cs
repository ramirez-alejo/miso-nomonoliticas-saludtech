using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ingestion.Infraestructura.Migrations
{
    /// <inheritdoc />
    public partial class AgregarNuevasTablas : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ImagenEntityId",
                table: "Metadatos",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ImagenId",
                table: "Metadatos",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "ImagenAnonimizadas",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ImagenProcesadaPath = table.Column<string>(type: "text", nullable: true),
                    ImagenId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ImagenAnonimizadas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ImagenAnonimizadas_Imagenes_ImagenId",
                        column: x => x.ImagenId,
                        principalTable: "Imagenes",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "MetadatosGenerados",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Key = table.Column<string>(type: "text", nullable: true),
                    Value = table.Column<string>(type: "text", nullable: true),
                    ImagenId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MetadatosGenerados", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MetadatosGenerados_Imagenes_ImagenId",
                        column: x => x.ImagenId,
                        principalTable: "Imagenes",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Metadatos_ImagenEntityId",
                table: "Metadatos",
                column: "ImagenEntityId");

            migrationBuilder.CreateIndex(
                name: "IX_ImagenAnonimizadas_ImagenId",
                table: "ImagenAnonimizadas",
                column: "ImagenId");

            migrationBuilder.CreateIndex(
                name: "IX_MetadatosGenerados_ImagenId",
                table: "MetadatosGenerados",
                column: "ImagenId");

            migrationBuilder.AddForeignKey(
                name: "FK_Metadatos_Imagenes_ImagenEntityId",
                table: "Metadatos",
                column: "ImagenEntityId",
                principalTable: "Imagenes",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Metadatos_Imagenes_ImagenEntityId",
                table: "Metadatos");

            migrationBuilder.DropTable(
                name: "ImagenAnonimizadas");

            migrationBuilder.DropTable(
                name: "MetadatosGenerados");

            migrationBuilder.DropIndex(
                name: "IX_Metadatos_ImagenEntityId",
                table: "Metadatos");

            migrationBuilder.DropColumn(
                name: "ImagenEntityId",
                table: "Metadatos");

            migrationBuilder.DropColumn(
                name: "ImagenId",
                table: "Metadatos");
        }
    }
}
