using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FUNSAR.Data.Migrations
{
    public partial class CreacionEstadoPFE : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PFE",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Detalle = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Estado = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    VoluntarioId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PFE", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PFE_Voluntario_VoluntarioId",
                        column: x => x.VoluntarioId,
                        principalTable: "Voluntario",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });
            migrationBuilder.CreateIndex(
                name: "IX_PFE_VoluntarioId",
                table: "PFE",
                column: "VoluntarioId");

            migrationBuilder.DropColumn(
                name: "Estado",
                table: "PFE");

            migrationBuilder.AddColumn<int>(
                name: "EstadoPFEId",
                table: "PFE",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "EstadoPFE",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Detalle = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EstadoPFE", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PFE_EstadoPFEId",
                table: "PFE",
                column: "EstadoPFEId");

            migrationBuilder.AddForeignKey(
                name: "FK_PFE_EstadoPFE_EstadoPFEId",
                table: "PFE",
                column: "EstadoPFEId",
                principalTable: "EstadoPFE",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PFE_EstadoPFE_EstadoPFEId",
                table: "PFE");

            migrationBuilder.DropTable(
                name: "EstadoPFE");

            migrationBuilder.DropIndex(
                name: "IX_PFE_EstadoPFEId",
                table: "PFE");

            migrationBuilder.DropColumn(
                name: "EstadoPFEId",
                table: "PFE");

            migrationBuilder.AddColumn<string>(
                name: "Estado",
                table: "PFE",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
