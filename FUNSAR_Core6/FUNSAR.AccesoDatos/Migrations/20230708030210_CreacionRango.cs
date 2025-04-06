using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FUNSAR.Data.Migrations
{
    public partial class CreacionRango : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Brigada",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "Rango",
                table: "AspNetUsers");

            migrationBuilder.AddColumn<int>(
                name: "BrigadaId",
                table: "AspNetUsers",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RangoId",
                table: "AspNetUsers",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Rango",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RangoNombre = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rango", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_BrigadaId",
                table: "AspNetUsers",
                column: "BrigadaId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_RangoId",
                table: "AspNetUsers",
                column: "RangoId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Brigada_BrigadaId",
                table: "AspNetUsers",
                column: "BrigadaId",
                principalTable: "Brigada",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Rango_RangoId",
                table: "AspNetUsers",
                column: "RangoId",
                principalTable: "Rango",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Brigada_BrigadaId",
                table: "AspNetUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Rango_RangoId",
                table: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "Rango");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_BrigadaId",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_RangoId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "BrigadaId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "RangoId",
                table: "AspNetUsers");

            migrationBuilder.AddColumn<string>(
                name: "Brigada",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Rango",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
