using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FUNSAR.Data.Migrations
{
    public partial class jornadacolegio : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Jornada",
                table: "DatoColegio");

            migrationBuilder.AddColumn<int>(
                name: "JornadaColegioId",
                table: "DatoColegio",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "JornadaId",
                table: "DatoColegio",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "JornadaColegio",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Jornada = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JornadaColegio", x => x.Id);
                });

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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DatoColegio_JornadaColegio_JornadaColegioId",
                table: "DatoColegio");

            migrationBuilder.DropTable(
                name: "JornadaColegio");

            migrationBuilder.DropIndex(
                name: "IX_DatoColegio_JornadaColegioId",
                table: "DatoColegio");

            migrationBuilder.DropColumn(
                name: "JornadaColegioId",
                table: "DatoColegio");

            migrationBuilder.DropColumn(
                name: "JornadaId",
                table: "DatoColegio");

            migrationBuilder.AddColumn<string>(
                name: "Jornada",
                table: "DatoColegio",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
