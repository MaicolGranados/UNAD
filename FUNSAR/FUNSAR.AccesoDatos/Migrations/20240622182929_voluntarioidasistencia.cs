using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FUNSAR.Data.Migrations
{
    public partial class voluntarioidasistencia : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Asistencia_Voluntario_VoluntarioId",
                table: "Asistencia");

            migrationBuilder.AlterColumn<int>(
                name: "VoluntarioId",
                table: "Asistencia",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Asistencia_Voluntario_VoluntarioId",
                table: "Asistencia",
                column: "VoluntarioId",
                principalTable: "Voluntario",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Asistencia_Voluntario_VoluntarioId",
                table: "Asistencia");

            migrationBuilder.AlterColumn<int>(
                name: "VoluntarioId",
                table: "Asistencia",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_Asistencia_Voluntario_VoluntarioId",
                table: "Asistencia",
                column: "VoluntarioId",
                principalTable: "Voluntario",
                principalColumn: "Id");
        }
    }
}
