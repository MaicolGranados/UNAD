using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FUNSAR.Data.Migrations
{
    public partial class nuloenacudiente : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Acudiente_Voluntario_VoluntarioId",
                table: "Acudiente");

            migrationBuilder.AlterColumn<int>(
                name: "VoluntarioId",
                table: "Acudiente",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_Acudiente_Voluntario_VoluntarioId",
                table: "Acudiente",
                column: "VoluntarioId",
                principalTable: "Voluntario",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Acudiente_Voluntario_VoluntarioId",
                table: "Acudiente");

            migrationBuilder.AlterColumn<int>(
                name: "VoluntarioId",
                table: "Acudiente",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Acudiente_Voluntario_VoluntarioId",
                table: "Acudiente",
                column: "VoluntarioId",
                principalTable: "Voluntario",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
