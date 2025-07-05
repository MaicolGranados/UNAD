using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FUNSAR.Data.Migrations
{
    public partial class idasistencia : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Asistencia_EstadoAsistencias_EstadoAsistenciaId",
                table: "Asistencia");

            migrationBuilder.AlterColumn<int>(
                name: "EstadoAsistenciaId",
                table: "Asistencia",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Asistencia_EstadoAsistencias_EstadoAsistenciaId",
                table: "Asistencia",
                column: "EstadoAsistenciaId",
                principalTable: "EstadoAsistencias",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Asistencia_EstadoAsistencias_EstadoAsistenciaId",
                table: "Asistencia");

            migrationBuilder.AlterColumn<int>(
                name: "EstadoAsistenciaId",
                table: "Asistencia",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_Asistencia_EstadoAsistencias_EstadoAsistenciaId",
                table: "Asistencia",
                column: "EstadoAsistenciaId",
                principalTable: "EstadoAsistencias",
                principalColumn: "Id");
        }
    }
}
