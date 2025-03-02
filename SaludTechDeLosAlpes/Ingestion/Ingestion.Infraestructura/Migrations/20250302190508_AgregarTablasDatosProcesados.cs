using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ingestion.Infraestructura.Migrations
{
    /// <inheritdoc />
    public partial class AgregarTablasDatosProcesados : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ImagenAnonimizadas_Imagenes_ImagenId",
                table: "ImagenAnonimizadas");

            migrationBuilder.DropForeignKey(
                name: "FK_MetadatosGenerados_Imagenes_ImagenId",
                table: "MetadatosGenerados");

            migrationBuilder.AlterColumn<string>(
                name: "Value",
                table: "MetadatosGenerados",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Key",
                table: "MetadatosGenerados",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "ImagenId",
                table: "MetadatosGenerados",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ImagenProcesadaPath",
                table: "ImagenAnonimizadas",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "ImagenId",
                table: "ImagenAnonimizadas",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ImagenEntityId",
                table: "ImagenAnonimizadas",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ImagenAnonimizadas_ImagenEntityId",
                table: "ImagenAnonimizadas",
                column: "ImagenEntityId");

            migrationBuilder.AddForeignKey(
                name: "FK_ImagenAnonimizadas_Imagenes_ImagenEntityId",
                table: "ImagenAnonimizadas",
                column: "ImagenEntityId",
                principalTable: "Imagenes",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ImagenAnonimizadas_Imagenes_ImagenId",
                table: "ImagenAnonimizadas",
                column: "ImagenId",
                principalTable: "Imagenes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MetadatosGenerados_Imagenes_ImagenId",
                table: "MetadatosGenerados",
                column: "ImagenId",
                principalTable: "Imagenes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ImagenAnonimizadas_Imagenes_ImagenEntityId",
                table: "ImagenAnonimizadas");

            migrationBuilder.DropForeignKey(
                name: "FK_ImagenAnonimizadas_Imagenes_ImagenId",
                table: "ImagenAnonimizadas");

            migrationBuilder.DropForeignKey(
                name: "FK_MetadatosGenerados_Imagenes_ImagenId",
                table: "MetadatosGenerados");

            migrationBuilder.DropIndex(
                name: "IX_ImagenAnonimizadas_ImagenEntityId",
                table: "ImagenAnonimizadas");

            migrationBuilder.DropColumn(
                name: "ImagenEntityId",
                table: "ImagenAnonimizadas");

            migrationBuilder.AlterColumn<string>(
                name: "Value",
                table: "MetadatosGenerados",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "Key",
                table: "MetadatosGenerados",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<Guid>(
                name: "ImagenId",
                table: "MetadatosGenerados",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AlterColumn<string>(
                name: "ImagenProcesadaPath",
                table: "ImagenAnonimizadas",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<Guid>(
                name: "ImagenId",
                table: "ImagenAnonimizadas",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddForeignKey(
                name: "FK_ImagenAnonimizadas_Imagenes_ImagenId",
                table: "ImagenAnonimizadas",
                column: "ImagenId",
                principalTable: "Imagenes",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_MetadatosGenerados_Imagenes_ImagenId",
                table: "MetadatosGenerados",
                column: "ImagenId",
                principalTable: "Imagenes",
                principalColumn: "Id");
        }
    }
}
