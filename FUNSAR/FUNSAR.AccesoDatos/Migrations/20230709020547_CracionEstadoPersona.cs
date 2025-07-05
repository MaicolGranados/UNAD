using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FUNSAR.Data.Migrations
{
    public partial class CracionEstadoPersona : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EstadoPersona",
                table: "Voluntario");

            migrationBuilder.DropColumn(
                name: "EstadoPersona",
                table: "CertificadoTemp");

            migrationBuilder.DropColumn(
                name: "EstadoPersona",
                table: "Certificado");

            migrationBuilder.AddColumn<int>(
                name: "EstadoId",
                table: "Voluntario",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "EstadoId",
                table: "CertificadoTemp",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "EstadoId",
                table: "Certificado",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "EstadoPersona",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    estadoPersona = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EstadoPersona", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Voluntario_EstadoId",
                table: "Voluntario",
                column: "EstadoId");

            migrationBuilder.CreateIndex(
                name: "IX_CertificadoTemp_EstadoId",
                table: "CertificadoTemp",
                column: "EstadoId");

            migrationBuilder.CreateIndex(
                name: "IX_Certificado_EstadoId",
                table: "Certificado",
                column: "EstadoId");

            migrationBuilder.AddForeignKey(
                name: "FK_Certificado_EstadoPersona_EstadoId",
                table: "Certificado",
                column: "EstadoId",
                principalTable: "EstadoPersona",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CertificadoTemp_EstadoPersona_EstadoId",
                table: "CertificadoTemp",
                column: "EstadoId",
                principalTable: "EstadoPersona",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Voluntario_EstadoPersona_EstadoId",
                table: "Voluntario",
                column: "EstadoId",
                principalTable: "EstadoPersona",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Certificado_EstadoPersona_EstadoId",
                table: "Certificado");

            migrationBuilder.DropForeignKey(
                name: "FK_CertificadoTemp_EstadoPersona_EstadoId",
                table: "CertificadoTemp");

            migrationBuilder.DropForeignKey(
                name: "FK_Voluntario_EstadoPersona_EstadoId",
                table: "Voluntario");

            migrationBuilder.DropTable(
                name: "EstadoPersona");

            migrationBuilder.DropIndex(
                name: "IX_Voluntario_EstadoId",
                table: "Voluntario");

            migrationBuilder.DropIndex(
                name: "IX_CertificadoTemp_EstadoId",
                table: "CertificadoTemp");

            migrationBuilder.DropIndex(
                name: "IX_Certificado_EstadoId",
                table: "Certificado");

            migrationBuilder.DropColumn(
                name: "EstadoId",
                table: "Voluntario");

            migrationBuilder.DropColumn(
                name: "EstadoId",
                table: "CertificadoTemp");

            migrationBuilder.DropColumn(
                name: "EstadoId",
                table: "Certificado");

            migrationBuilder.AddColumn<string>(
                name: "EstadoPersona",
                table: "Voluntario",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EstadoPersona",
                table: "CertificadoTemp",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EstadoPersona",
                table: "Certificado",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
