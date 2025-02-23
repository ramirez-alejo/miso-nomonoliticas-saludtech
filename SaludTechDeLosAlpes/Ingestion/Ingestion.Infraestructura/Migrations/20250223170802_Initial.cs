using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ingestion.Infraestructura.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AtributosImagen",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Resolucion = table.Column<string>(type: "text", nullable: false),
                    Contraste = table.Column<string>(type: "text", nullable: false),
                    Es3D = table.Column<bool>(type: "boolean", nullable: false),
                    FaseEscaner = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AtributosImagen", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ContextosProcesales",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Etapa = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContextosProcesales", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Demografias",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    GrupoEdad = table.Column<string>(type: "text", nullable: false),
                    Sexo = table.Column<string>(type: "text", nullable: false),
                    Etnicidad = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Demografias", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EntornosClinicos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TipoAmbiente = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EntornosClinicos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Historiales",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Fumador = table.Column<bool>(type: "boolean", nullable: false),
                    Diabetico = table.Column<bool>(type: "boolean", nullable: false),
                    CondicionesPrevias = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Historiales", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Modalidades",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Nombre = table.Column<string>(type: "text", nullable: false),
                    Descripcion = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Modalidades", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Patologias",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Descripcion = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Patologias", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RegionesAnatomicas",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Nombre = table.Column<string>(type: "text", nullable: false),
                    Descripcion = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RegionesAnatomicas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Sintomas",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Descripcion = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sintomas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Metadatos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    EntornoClinicoId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Metadatos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Metadatos_EntornosClinicos_EntornoClinicoId",
                        column: x => x.EntornoClinicoId,
                        principalTable: "EntornosClinicos",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Pacientes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    DemografiaId = table.Column<Guid>(type: "uuid", nullable: false),
                    HistorialId = table.Column<Guid>(type: "uuid", nullable: false),
                    TokenAnonimo = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pacientes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Pacientes_Demografias_DemografiaId",
                        column: x => x.DemografiaId,
                        principalTable: "Demografias",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Pacientes_Historiales_HistorialId",
                        column: x => x.HistorialId,
                        principalTable: "Historiales",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TiposImagen",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ModalidadId = table.Column<Guid>(type: "uuid", nullable: false),
                    RegionAnatomicaId = table.Column<Guid>(type: "uuid", nullable: false),
                    PatologiaId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TiposImagen", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TiposImagen_Modalidades_ModalidadId",
                        column: x => x.ModalidadId,
                        principalTable: "Modalidades",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TiposImagen_Patologias_PatologiaId",
                        column: x => x.PatologiaId,
                        principalTable: "Patologias",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_TiposImagen_RegionesAnatomicas_RegionAnatomicaId",
                        column: x => x.RegionAnatomicaId,
                        principalTable: "RegionesAnatomicas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MetadatosSintomas",
                columns: table => new
                {
                    MetadatosId = table.Column<Guid>(type: "uuid", nullable: false),
                    SintomasId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MetadatosSintomas", x => new { x.MetadatosId, x.SintomasId });
                    table.ForeignKey(
                        name: "FK_MetadatosSintomas_Metadatos_MetadatosId",
                        column: x => x.MetadatosId,
                        principalTable: "Metadatos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MetadatosSintomas_Sintomas_SintomasId",
                        column: x => x.SintomasId,
                        principalTable: "Sintomas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Imagenes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Version = table.Column<string>(type: "text", nullable: false),
                    TipoImagenId = table.Column<Guid>(type: "uuid", nullable: false),
                    AtributosImagenId = table.Column<Guid>(type: "uuid", nullable: false),
                    ContextoProcesalId = table.Column<Guid>(type: "uuid", nullable: true),
                    MetadatosId = table.Column<Guid>(type: "uuid", nullable: true),
                    PacienteId = table.Column<Guid>(type: "uuid", nullable: true),
                    EntornoClinicoId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Imagenes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Imagenes_AtributosImagen_AtributosImagenId",
                        column: x => x.AtributosImagenId,
                        principalTable: "AtributosImagen",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Imagenes_ContextosProcesales_ContextoProcesalId",
                        column: x => x.ContextoProcesalId,
                        principalTable: "ContextosProcesales",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Imagenes_EntornosClinicos_EntornoClinicoId",
                        column: x => x.EntornoClinicoId,
                        principalTable: "EntornosClinicos",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Imagenes_Metadatos_MetadatosId",
                        column: x => x.MetadatosId,
                        principalTable: "Metadatos",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Imagenes_Pacientes_PacienteId",
                        column: x => x.PacienteId,
                        principalTable: "Pacientes",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Imagenes_TiposImagen_TipoImagenId",
                        column: x => x.TipoImagenId,
                        principalTable: "TiposImagen",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Imagenes_AtributosImagenId",
                table: "Imagenes",
                column: "AtributosImagenId");

            migrationBuilder.CreateIndex(
                name: "IX_Imagenes_ContextoProcesalId",
                table: "Imagenes",
                column: "ContextoProcesalId");

            migrationBuilder.CreateIndex(
                name: "IX_Imagenes_EntornoClinicoId",
                table: "Imagenes",
                column: "EntornoClinicoId");

            migrationBuilder.CreateIndex(
                name: "IX_Imagenes_MetadatosId",
                table: "Imagenes",
                column: "MetadatosId");

            migrationBuilder.CreateIndex(
                name: "IX_Imagenes_PacienteId",
                table: "Imagenes",
                column: "PacienteId");

            migrationBuilder.CreateIndex(
                name: "IX_Imagenes_TipoImagenId",
                table: "Imagenes",
                column: "TipoImagenId");

            migrationBuilder.CreateIndex(
                name: "IX_Metadatos_EntornoClinicoId",
                table: "Metadatos",
                column: "EntornoClinicoId");

            migrationBuilder.CreateIndex(
                name: "IX_MetadatosSintomas_SintomasId",
                table: "MetadatosSintomas",
                column: "SintomasId");

            migrationBuilder.CreateIndex(
                name: "IX_Pacientes_DemografiaId",
                table: "Pacientes",
                column: "DemografiaId");

            migrationBuilder.CreateIndex(
                name: "IX_Pacientes_HistorialId",
                table: "Pacientes",
                column: "HistorialId");

            migrationBuilder.CreateIndex(
                name: "IX_TiposImagen_ModalidadId",
                table: "TiposImagen",
                column: "ModalidadId");

            migrationBuilder.CreateIndex(
                name: "IX_TiposImagen_PatologiaId",
                table: "TiposImagen",
                column: "PatologiaId");

            migrationBuilder.CreateIndex(
                name: "IX_TiposImagen_RegionAnatomicaId",
                table: "TiposImagen",
                column: "RegionAnatomicaId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Imagenes");

            migrationBuilder.DropTable(
                name: "MetadatosSintomas");

            migrationBuilder.DropTable(
                name: "AtributosImagen");

            migrationBuilder.DropTable(
                name: "ContextosProcesales");

            migrationBuilder.DropTable(
                name: "Pacientes");

            migrationBuilder.DropTable(
                name: "TiposImagen");

            migrationBuilder.DropTable(
                name: "Metadatos");

            migrationBuilder.DropTable(
                name: "Sintomas");

            migrationBuilder.DropTable(
                name: "Demografias");

            migrationBuilder.DropTable(
                name: "Historiales");

            migrationBuilder.DropTable(
                name: "Modalidades");

            migrationBuilder.DropTable(
                name: "Patologias");

            migrationBuilder.DropTable(
                name: "RegionesAnatomicas");

            migrationBuilder.DropTable(
                name: "EntornosClinicos");
        }
    }
}
