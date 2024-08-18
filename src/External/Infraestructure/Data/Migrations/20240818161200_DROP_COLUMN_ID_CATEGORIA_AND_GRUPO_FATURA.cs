using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations
{
    /// <inheritdoc />
    public partial class DROP_COLUMN_ID_CATEGORIA_AND_GRUPO_FATURA : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DespesasTotalPorCategoria");

            migrationBuilder.DropIndex(
                name: "IX_Categorias_Id",
                table: "Categorias");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "Grupo_Fatura");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "Categorias");

            migrationBuilder.CreateTable(
                name: "CategoriaQueryResult",
                columns: table => new
                {
                    Descricao = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Total = table.Column<double>(type: "double", nullable: false),
                    QuantidadeDeItens = table.Column<int>(type: "int", nullable: false),
                    Code = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci")
                },
                constraints: table =>
                {
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_Categorias_Code",
                table: "Categorias",
                column: "Code");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CategoriaQueryResult");

            migrationBuilder.DropIndex(
                name: "IX_Categorias_Code",
                table: "Categorias");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "Grupo_Fatura",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "Categorias",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.CreateTable(
                name: "DespesasTotalPorCategoria",
                columns: table => new
                {
                    Categoria = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    QuantidadeDeItens = table.Column<int>(type: "int", nullable: false),
                    Total = table.Column<double>(type: "double", nullable: false)
                },
                constraints: table =>
                {
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_Categorias_Id",
                table: "Categorias",
                column: "Id");
        }
    }
}
