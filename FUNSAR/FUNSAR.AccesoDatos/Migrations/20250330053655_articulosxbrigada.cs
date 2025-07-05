using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FUNSAR.Data.Migrations
{
    public partial class articulosxbrigada : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Articulo_Brigada_BrigadaId",
                table: "Articulo");

            migrationBuilder.DropIndex(
                name: "IX_Articulo_BrigadaId",
                table: "Articulo");

            migrationBuilder.AlterColumn<string>(
                name: "BrigadaId",
                table: "Articulo",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "BrigadaId",
                table: "Articulo",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Articulo_BrigadaId",
                table: "Articulo",
                column: "BrigadaId");

            migrationBuilder.AddForeignKey(
                name: "FK_Articulo_Brigada_BrigadaId",
                table: "Articulo",
                column: "BrigadaId",
                principalTable: "Brigada",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
