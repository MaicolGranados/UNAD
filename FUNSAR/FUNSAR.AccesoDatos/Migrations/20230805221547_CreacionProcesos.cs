using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FUNSAR.Data.Migrations
{
    public partial class CreacionProcesos : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ProcesoId",
                table: "Voluntario",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ProcesoId",
                table: "CertificadoTemp",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ProcesoId",
                table: "Certificado",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Proceso",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    proceso = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Proceso", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Voluntario_ProcesoId",
                table: "Voluntario",
                column: "ProcesoId");

            migrationBuilder.CreateIndex(
                name: "IX_CertificadoTemp_ProcesoId",
                table: "CertificadoTemp",
                column: "ProcesoId");

            migrationBuilder.CreateIndex(
                name: "IX_Certificado_ProcesoId",
                table: "Certificado",
                column: "ProcesoId");

            migrationBuilder.AddForeignKey(
                name: "FK_Certificado_Proceso_ProcesoId",
                table: "Certificado",
                column: "ProcesoId",
                principalTable: "Proceso",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CertificadoTemp_Proceso_ProcesoId",
                table: "CertificadoTemp",
                column: "ProcesoId",
                principalTable: "Proceso",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Voluntario_Proceso_ProcesoId",
                table: "Voluntario",
                column: "ProcesoId",
                principalTable: "Proceso",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Certificado_Proceso_ProcesoId",
                table: "Certificado");

            migrationBuilder.DropForeignKey(
                name: "FK_CertificadoTemp_Proceso_ProcesoId",
                table: "CertificadoTemp");

            migrationBuilder.DropForeignKey(
                name: "FK_Voluntario_Proceso_ProcesoId",
                table: "Voluntario");

            migrationBuilder.DropTable(
                name: "Proceso");

            migrationBuilder.DropIndex(
                name: "IX_Voluntario_ProcesoId",
                table: "Voluntario");

            migrationBuilder.DropIndex(
                name: "IX_CertificadoTemp_ProcesoId",
                table: "CertificadoTemp");

            migrationBuilder.DropIndex(
                name: "IX_Certificado_ProcesoId",
                table: "Certificado");

            migrationBuilder.DropColumn(
                name: "ProcesoId",
                table: "Voluntario");

            migrationBuilder.DropColumn(
                name: "ProcesoId",
                table: "CertificadoTemp");

            migrationBuilder.DropColumn(
                name: "ProcesoId",
                table: "Certificado");
        }
    }
}
