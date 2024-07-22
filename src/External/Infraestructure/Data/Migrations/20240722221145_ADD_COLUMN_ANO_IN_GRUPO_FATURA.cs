using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations
{
    /// <inheritdoc />
    public partial class ADD_COLUMN_ANO_IN_GRUPO_FATURA : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Ano",
                table: "Grupo_Fatura",
                type: "varchar(15)",
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_Grupo_Fatura_Ano",
                table: "Grupo_Fatura",
                column: "Ano");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Grupo_Fatura_Ano",
                table: "Grupo_Fatura");

            migrationBuilder.DropColumn(
                name: "Ano",
                table: "Grupo_Fatura");
        }
    }
}
