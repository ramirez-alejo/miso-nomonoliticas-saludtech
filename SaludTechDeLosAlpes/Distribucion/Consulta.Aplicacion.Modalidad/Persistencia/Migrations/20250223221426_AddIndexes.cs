using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Consulta.Aplicacion.Modalidad.Persistencia.Migrations
{
    /// <inheritdoc />
    public partial class AddIndexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            
            migrationBuilder.Sql("CREATE EXTENSION IF NOT EXISTS pg_trgm;");
            migrationBuilder.AlterColumn<string>(
                name: "RegionDescripcion",
                table: "ImagenesModalidad",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "RegionAnatomica",
                table: "ImagenesModalidad",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "Nombre",
                table: "ImagenesModalidad",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "Descripcion",
                table: "ImagenesModalidad",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.CreateIndex(
                name: "IX_ImagenesModalidad_Descripcion",
                table: "ImagenesModalidad",
                column: "Descripcion")
                .Annotation("Npgsql:IndexMethod", "GIN")
                .Annotation("Npgsql:IndexOperators", new[] { "gin_trgm_ops" });

            migrationBuilder.CreateIndex(
                name: "IX_ImagenesModalidad_Nombre",
                table: "ImagenesModalidad",
                column: "Nombre")
                .Annotation("Npgsql:IndexMethod", "GIN")
                .Annotation("Npgsql:IndexOperators", new[] { "gin_trgm_ops" });

            migrationBuilder.CreateIndex(
                name: "IX_ImagenesModalidad_RegionAnatomica",
                table: "ImagenesModalidad",
                column: "RegionAnatomica")
                .Annotation("Npgsql:IndexMethod", "GIN")
                .Annotation("Npgsql:IndexOperators", new[] { "gin_trgm_ops" });

            migrationBuilder.CreateIndex(
                name: "IX_ImagenesModalidad_RegionDescripcion",
                table: "ImagenesModalidad",
                column: "RegionDescripcion")
                .Annotation("Npgsql:IndexMethod", "GIN")
                .Annotation("Npgsql:IndexOperators", new[] { "gin_trgm_ops" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ImagenesModalidad_Descripcion",
                table: "ImagenesModalidad");

            migrationBuilder.DropIndex(
                name: "IX_ImagenesModalidad_Nombre",
                table: "ImagenesModalidad");

            migrationBuilder.DropIndex(
                name: "IX_ImagenesModalidad_RegionAnatomica",
                table: "ImagenesModalidad");

            migrationBuilder.DropIndex(
                name: "IX_ImagenesModalidad_RegionDescripcion",
                table: "ImagenesModalidad");

            migrationBuilder.AlterColumn<string>(
                name: "RegionDescripcion",
                table: "ImagenesModalidad",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "RegionAnatomica",
                table: "ImagenesModalidad",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Nombre",
                table: "ImagenesModalidad",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Descripcion",
                table: "ImagenesModalidad",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);
        }
    }
}
