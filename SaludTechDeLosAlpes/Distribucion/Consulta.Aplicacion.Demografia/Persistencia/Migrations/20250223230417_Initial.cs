using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Consulta.Aplicacion.Demografia.Persistencia.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("CREATE EXTENSION IF NOT EXISTS pg_trgm;");
            migrationBuilder.CreateTable(
                name: "ImagenesDemograficas",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ImagenId = table.Column<Guid>(type: "uuid", nullable: false),
                    GrupoEdad = table.Column<string>(type: "text", nullable: true),
                    Sexo = table.Column<string>(type: "text", nullable: true),
                    Etnicidad = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ImagenesDemograficas", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ImagenesDemograficas_Etnicidad",
                table: "ImagenesDemograficas",
                column: "Etnicidad")
                .Annotation("Npgsql:IndexMethod", "GIN")
                .Annotation("Npgsql:IndexOperators", new[] { "gin_trgm_ops" });

            migrationBuilder.CreateIndex(
                name: "IX_ImagenesDemograficas_GrupoEdad",
                table: "ImagenesDemograficas",
                column: "GrupoEdad")
                .Annotation("Npgsql:IndexMethod", "GIN")
                .Annotation("Npgsql:IndexOperators", new[] { "gin_trgm_ops" });

            migrationBuilder.CreateIndex(
                name: "IX_ImagenesDemograficas_ImagenId",
                table: "ImagenesDemograficas",
                column: "ImagenId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ImagenesDemograficas_Sexo",
                table: "ImagenesDemograficas",
                column: "Sexo")
                .Annotation("Npgsql:IndexMethod", "GIN")
                .Annotation("Npgsql:IndexOperators", new[] { "gin_trgm_ops" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ImagenesDemograficas");
        }
    }
}
