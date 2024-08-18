using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations
{
    /// <inheritdoc />
    public partial class MIGRAR_FK_PARA_CODE : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Despesas_Categorias_CategoriaId",
                table: "Despesas");

            migrationBuilder.DropForeignKey(
                name: "FK_Despesas_Grupo_Fatura_GrupoFaturaId",
                table: "Despesas");

            migrationBuilder.DropForeignKey(
                name: "FK_Status_Faturas_Grupo_Fatura_GrupoFaturaId",
                table: "Status_Faturas");

            migrationBuilder.DropIndex(
                name: "IX_Status_Faturas_GrupoFaturaId",
                table: "Status_Faturas");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Grupo_Fatura",
                table: "Grupo_Fatura");

            migrationBuilder.DropIndex(
                name: "IX_Grupo_Fatura_Id",
                table: "Grupo_Fatura");

            migrationBuilder.DropIndex(
                name: "IX_Despesas_CategoriaId",
                table: "Despesas");

            migrationBuilder.DropIndex(
                name: "IX_Despesas_GrupoFaturaId",
                table: "Despesas");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Categorias",
                table: "Categorias");

            migrationBuilder.DropColumn(
                name: "GrupoFaturaId",
                table: "Status_Faturas");

            migrationBuilder.DropColumn(
                name: "CategoriaId",
                table: "Despesas");

            migrationBuilder.DropColumn(
                name: "GrupoFaturaId",
                table: "Despesas");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Grupo_Fatura",
                table: "Grupo_Fatura",
                column: "Code");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Categorias",
                table: "Categorias",
                column: "Code");

            migrationBuilder.CreateIndex(
                name: "IX_Status_Faturas_GrupoFaturaCode",
                table: "Status_Faturas",
                column: "GrupoFaturaCode");

            migrationBuilder.CreateIndex(
                name: "IX_Grupo_Fatura_Code",
                table: "Grupo_Fatura",
                column: "Code");

            migrationBuilder.CreateIndex(
                name: "IX_Despesas_CategoriaCode",
                table: "Despesas",
                column: "CategoriaCode");

            migrationBuilder.CreateIndex(
                name: "IX_Despesas_GrupoFaturaCode",
                table: "Despesas",
                column: "GrupoFaturaCode");

            migrationBuilder.AddForeignKey(
                name: "FK_Despesas_Categorias_CategoriaCode",
                table: "Despesas",
                column: "CategoriaCode",
                principalTable: "Categorias",
                principalColumn: "Code",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Despesas_Grupo_Fatura_GrupoFaturaCode",
                table: "Despesas",
                column: "GrupoFaturaCode",
                principalTable: "Grupo_Fatura",
                principalColumn: "Code",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Status_Faturas_Grupo_Fatura_GrupoFaturaCode",
                table: "Status_Faturas",
                column: "GrupoFaturaCode",
                principalTable: "Grupo_Fatura",
                principalColumn: "Code",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Despesas_Categorias_CategoriaCode",
                table: "Despesas");

            migrationBuilder.DropForeignKey(
                name: "FK_Despesas_Grupo_Fatura_GrupoFaturaCode",
                table: "Despesas");

            migrationBuilder.DropForeignKey(
                name: "FK_Status_Faturas_Grupo_Fatura_GrupoFaturaCode",
                table: "Status_Faturas");

            migrationBuilder.DropIndex(
                name: "IX_Status_Faturas_GrupoFaturaCode",
                table: "Status_Faturas");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Grupo_Fatura",
                table: "Grupo_Fatura");

            migrationBuilder.DropIndex(
                name: "IX_Grupo_Fatura_Code",
                table: "Grupo_Fatura");

            migrationBuilder.DropIndex(
                name: "IX_Despesas_CategoriaCode",
                table: "Despesas");

            migrationBuilder.DropIndex(
                name: "IX_Despesas_GrupoFaturaCode",
                table: "Despesas");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Categorias",
                table: "Categorias");

            migrationBuilder.AddColumn<int>(
                name: "GrupoFaturaId",
                table: "Status_Faturas",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "CategoriaId",
                table: "Despesas",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "GrupoFaturaId",
                table: "Despesas",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Grupo_Fatura",
                table: "Grupo_Fatura",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Categorias",
                table: "Categorias",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Status_Faturas_GrupoFaturaId",
                table: "Status_Faturas",
                column: "GrupoFaturaId");

            migrationBuilder.CreateIndex(
                name: "IX_Grupo_Fatura_Id",
                table: "Grupo_Fatura",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Despesas_CategoriaId",
                table: "Despesas",
                column: "CategoriaId");

            migrationBuilder.CreateIndex(
                name: "IX_Despesas_GrupoFaturaId",
                table: "Despesas",
                column: "GrupoFaturaId");

            migrationBuilder.AddForeignKey(
                name: "FK_Despesas_Categorias_CategoriaId",
                table: "Despesas",
                column: "CategoriaId",
                principalTable: "Categorias",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Despesas_Grupo_Fatura_GrupoFaturaId",
                table: "Despesas",
                column: "GrupoFaturaId",
                principalTable: "Grupo_Fatura",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Status_Faturas_Grupo_Fatura_GrupoFaturaId",
                table: "Status_Faturas",
                column: "GrupoFaturaId",
                principalTable: "Grupo_Fatura",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
