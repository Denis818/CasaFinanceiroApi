using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations
{
    /// <inheritdoc />
    public partial class CREATE_TABLE_PARAMETRO_ALERTA_GASTOS : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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
                    TotalGastosMoradia = table.Column<double>(type: "double", nullable: false),
                    TotalGastosCasa = table.Column<double>(type: "double", nullable: false),
                    TotalGeral = table.Column<double>(type: "double", nullable: false)
                },
                constraints: table =>
                {
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "DespesasTotalPorCategoria",
                columns: table => new
                {
                    Categoria = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Total = table.Column<double>(type: "double", nullable: false),
                    QuantidadeDeItens = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Parametro_Alerta_Gastos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    TipoMetrica = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    LimiteVermelho = table.Column<decimal>(type: "decimal(8,2)", nullable: false),
                    LimiteAmarelo = table.Column<decimal>(type: "decimal(8,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Parametro_Alerta_Gastos", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_Parametro_Alerta_Gastos_Id",
                table: "Parametro_Alerta_Gastos",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Parametro_Alerta_Gastos_TipoMetrica",
                table: "Parametro_Alerta_Gastos",
                column: "TipoMetrica");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DespesasPorGrupoQueryDto");

            migrationBuilder.DropTable(
                name: "DespesasRelatorioGastosDoGrupo");

            migrationBuilder.DropTable(
                name: "DespesasTotalPorCategoria");

            migrationBuilder.DropTable(
                name: "Parametro_Alerta_Gastos");
        }
    }
}
