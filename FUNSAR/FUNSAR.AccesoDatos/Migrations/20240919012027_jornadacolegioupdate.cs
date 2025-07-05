using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FUNSAR.Data.Migrations
{
    public partial class jornadacolegioupdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DatoColegio_JornadaColegio_JornadaColegioId",
                table: "DatoColegio");

            migrationBuilder.DropIndex(
                name: "IX_DatoColegio_JornadaColegioId",
                table: "DatoColegio");

            migrationBuilder.DropColumn(
                name: "JornadaColegioId",
                table: "DatoColegio");

            migrationBuilder.CreateIndex(
                name: "IX_DatoColegio_JornadaId",
                table: "DatoColegio",
                column: "JornadaId");

            migrationBuilder.AddForeignKey(
                name: "FK_DatoColegio_JornadaColegio_JornadaId",
                table: "DatoColegio",
                column: "JornadaId",
                principalTable: "JornadaColegio",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DatoColegio_JornadaColegio_JornadaId",
                table: "DatoColegio");

            migrationBuilder.DropIndex(
                name: "IX_DatoColegio_JornadaId",
                table: "DatoColegio");

            migrationBuilder.AddColumn<int>(
                name: "JornadaColegioId",
                table: "DatoColegio",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_DatoColegio_JornadaColegioId",
                table: "DatoColegio",
                column: "JornadaColegioId");

            migrationBuilder.AddForeignKey(
                name: "FK_DatoColegio_JornadaColegio_JornadaColegioId",
                table: "DatoColegio",
                column: "JornadaColegioId",
                principalTable: "JornadaColegio",
                principalColumn: "Id");
        }
    }
}
