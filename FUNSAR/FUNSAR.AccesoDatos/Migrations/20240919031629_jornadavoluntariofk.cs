using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FUNSAR.Data.Migrations
{
    public partial class jornadavoluntariofk : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Voluntario_JornadaId",
                table: "Voluntario",
                column: "JornadaId");

            migrationBuilder.AddForeignKey(
                name: "FK_Voluntario_JornadaColegio_JornadaId",
                table: "Voluntario",
                column: "JornadaId",
                principalTable: "JornadaColegio",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Voluntario_JornadaColegio_JornadaId",
                table: "Voluntario");

            migrationBuilder.DropIndex(
                name: "IX_Voluntario_JornadaId",
                table: "Voluntario");
        }
    }
}
