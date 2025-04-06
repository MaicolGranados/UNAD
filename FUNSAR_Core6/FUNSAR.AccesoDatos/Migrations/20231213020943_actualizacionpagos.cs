using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FUNSAR.Data.Migrations
{
    public partial class actualizacionpagos : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Pagos_TDocumento_DocumentoId",
                table: "Pagos");

            migrationBuilder.RenameColumn(
                name: "DocumentoId",
                table: "Pagos",
                newName: "DocumentoIdR");

            migrationBuilder.RenameColumn(
                name: "Documento",
                table: "Pagos",
                newName: "DocumentoR");

            migrationBuilder.RenameIndex(
                name: "IX_Pagos_DocumentoId",
                table: "Pagos",
                newName: "IX_Pagos_DocumentoIdR");

            migrationBuilder.AddColumn<string>(
                name: "DocumentoP",
                table: "Pagos",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddForeignKey(
                name: "FK_Pagos_TDocumento_DocumentoIdR",
                table: "Pagos",
                column: "DocumentoIdR",
                principalTable: "TDocumento",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Pagos_TDocumento_DocumentoIdR",
                table: "Pagos");

            migrationBuilder.DropColumn(
                name: "DocumentoP",
                table: "Pagos");

            migrationBuilder.RenameColumn(
                name: "DocumentoR",
                table: "Pagos",
                newName: "Documento");

            migrationBuilder.RenameColumn(
                name: "DocumentoIdR",
                table: "Pagos",
                newName: "DocumentoId");

            migrationBuilder.RenameIndex(
                name: "IX_Pagos_DocumentoIdR",
                table: "Pagos",
                newName: "IX_Pagos_DocumentoId");

            migrationBuilder.AddForeignKey(
                name: "FK_Pagos_TDocumento_DocumentoId",
                table: "Pagos",
                column: "DocumentoId",
                principalTable: "TDocumento",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
