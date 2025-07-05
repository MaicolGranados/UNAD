using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FUNSAR.Data.Migrations
{
    public partial class vigenciaservicio : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Articulo_Servicio_ServicioId",
                table: "Articulo");

            migrationBuilder.AlterColumn<int>(
                name: "ServicioId",
                table: "Articulo",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "BrigadaId",
                table: "Articulo",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "VigenciaServicio",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ServicioId = table.Column<int>(type: "int", nullable: true),
                    Vigencia = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VigenciaServicio", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VigenciaServicio_Servicio_ServicioId",
                        column: x => x.ServicioId,
                        principalTable: "Servicio",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Articulo_BrigadaId",
                table: "Articulo",
                column: "BrigadaId");

            migrationBuilder.CreateIndex(
                name: "IX_VigenciaServicio_ServicioId",
                table: "VigenciaServicio",
                column: "ServicioId");

            migrationBuilder.AddForeignKey(
                name: "FK_Articulo_Brigada_BrigadaId",
                table: "Articulo",
                column: "BrigadaId",
                principalTable: "Brigada",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Articulo_Servicio_ServicioId",
                table: "Articulo",
                column: "ServicioId",
                principalTable: "Servicio",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Articulo_Brigada_BrigadaId",
                table: "Articulo");

            migrationBuilder.DropForeignKey(
                name: "FK_Articulo_Servicio_ServicioId",
                table: "Articulo");

            migrationBuilder.DropTable(
                name: "VigenciaServicio");

            migrationBuilder.DropIndex(
                name: "IX_Articulo_BrigadaId",
                table: "Articulo");

            migrationBuilder.DropColumn(
                name: "BrigadaId",
                table: "Articulo");

            migrationBuilder.AlterColumn<int>(
                name: "ServicioId",
                table: "Articulo",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_Articulo_Servicio_ServicioId",
                table: "Articulo",
                column: "ServicioId",
                principalTable: "Servicio",
                principalColumn: "Id");
        }
    }
}
