using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations
{
    /// <inheritdoc />
    public partial class ADD_COLUMNS_RECEBIMENTO_AND_COMPRAS : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Compras",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Nome = table.Column<string>(type: "varchar(70)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Parcelas = table.Column<int>(type: "int", nullable: false),
                    ValorTotal = table.Column<double>(type: "double(7,2)", nullable: false),
                    ValorPorParcela = table.Column<double>(type: "double(7,2)", nullable: false),
                    DividioPorDois = table.Column<bool>(type: "TINYINT(1)", nullable: false),
                    Code = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Compras", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Recebimentos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Data = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Valor = table.Column<double>(type: "double(7,2)", nullable: false),
                    Code = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Recebimentos", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Compras");

            migrationBuilder.DropTable(
                name: "Recebimentos");

            migrationBuilder.CreateTable(
                name: "CategoriaQueryResult",
                columns: table => new
                {
                    Code = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    Descricao = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    QuantidadeDeItens = table.Column<int>(type: "int", nullable: false),
                    Total = table.Column<double>(type: "double", nullable: false)
                },
                constraints: table =>
                {
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "DespesasPorGrupoQueryDto",
                columns: table => new
                {
                    GrupoNome = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Total = table.Column<double>(type: "double", nullable: false)
                },
                constraints: table =>
                {
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "DespesasRelatorioGastosDoGrupo",
                columns: table => new
                {
                    GrupoFaturaNome = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    TotalGastosCasa = table.Column<double>(type: "double", nullable: false),
                    TotalGastosMoradia = table.Column<double>(type: "double", nullable: false),
                    TotalGeral = table.Column<double>(type: "double", nullable: false)
                },
                constraints: table =>
                {
                })
                .Annotation("MySql:CharSet", "utf8mb4");
        }
    }
}
