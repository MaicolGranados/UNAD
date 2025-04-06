using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FUNSAR.Data.Migrations
{
    public partial class actividadservicio : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ServicioId",
                table: "Articulo",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Articulo_ServicioId",
                table: "Articulo",
                column: "ServicioId");

            migrationBuilder.AddForeignKey(
                name: "FK_Articulo_Servicio_ServicioId",
                table: "Articulo",
                column: "ServicioId",
                principalTable: "Servicio",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Articulo_Servicio_ServicioId",
                table: "Articulo");

            migrationBuilder.DropIndex(
                name: "IX_Articulo_ServicioId",
                table: "Articulo");

            migrationBuilder.DropColumn(
                name: "ServicioId",
                table: "Articulo");
        }
    }
}
