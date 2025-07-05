using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FUNSAR.Data.Migrations
{
    public partial class usuariocolegios : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DatoColegio",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NombreEncargado = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Correo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NumeroContacto = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Cargo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Jornada = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ColegioId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DatoColegio", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DatoColegio_Colegio_ColegioId",
                        column: x => x.ColegioId,
                        principalTable: "Colegio",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Otp",
                columns: table => new
                {
                    id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Mail = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Code = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Otp", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DatoColegio_ColegioId",
                table: "DatoColegio",
                column: "ColegioId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DatoColegio");

            migrationBuilder.DropTable(
                name: "Otp");
        }
    }
}
