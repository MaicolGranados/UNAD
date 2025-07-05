using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FUNSAR.Data.Migrations
{
    public partial class estadosasistencias : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Asistio",
                table: "Asistencia");

            migrationBuilder.AddColumn<int>(
                name: "EstadoAsistenciaId",
                table: "Asistencia",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "EstadoAsistencias",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    estadoAsistencia = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EstadoAsistencias", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Asistencia_EstadoAsistenciaId",
                table: "Asistencia",
                column: "EstadoAsistenciaId");

            migrationBuilder.AddForeignKey(
                name: "FK_Asistencia_EstadoAsistencias_EstadoAsistenciaId",
                table: "Asistencia",
                column: "EstadoAsistenciaId",
                principalTable: "EstadoAsistencias",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Asistencia_EstadoAsistencias_EstadoAsistenciaId",
                table: "Asistencia");

            migrationBuilder.DropTable(
                name: "EstadoAsistencias");

            migrationBuilder.DropIndex(
                name: "IX_Asistencia_EstadoAsistenciaId",
                table: "Asistencia");

            migrationBuilder.DropColumn(
                name: "EstadoAsistenciaId",
                table: "Asistencia");

            migrationBuilder.AddColumn<bool>(
                name: "Asistio",
                table: "Asistencia",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
