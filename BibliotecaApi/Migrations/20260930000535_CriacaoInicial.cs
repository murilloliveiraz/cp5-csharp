using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace BibliotecaApi.Migrations
{
    /// <inheritdoc />
    public partial class CriacaoInicial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Autores",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Nome = table.Column<string>(type: "TEXT", maxLength: 150, nullable: false),
                    Nacionalidade = table.Column<string>(type: "TEXT", maxLength: 80, nullable: false),
                    DataNascimento = table.Column<DateOnly>(type: "TEXT", nullable: true),
                    CriadoEm = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Autores", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Livros",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Titulo = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Isbn = table.Column<string>(type: "TEXT", maxLength: 13, nullable: false),
                    Genero = table.Column<string>(type: "TEXT", maxLength: 60, nullable: false),
                    AnoPublicacao = table.Column<int>(type: "INTEGER", nullable: false),
                    QuantidadeEstoque = table.Column<int>(type: "INTEGER", nullable: false),
                    CriadoEm = table.Column<DateTime>(type: "TEXT", nullable: false),
                    AutorId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Livros", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Livros_Autores_AutorId",
                        column: x => x.AutorId,
                        principalTable: "Autores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "Autores",
                columns: new[] { "Id", "CriadoEm", "DataNascimento", "Nacionalidade", "Nome" },
                values: new object[,]
                {
                    { 1, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateOnly(1839, 6, 21), "Brasileira", "Machado de Assis" },
                    { 2, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateOnly(1920, 12, 10), "Brasileira", "Clarice Lispector" }
                });

            migrationBuilder.InsertData(
                table: "Livros",
                columns: new[] { "Id", "AnoPublicacao", "AutorId", "CriadoEm", "Genero", "Isbn", "QuantidadeEstoque", "Titulo" },
                values: new object[,]
                {
                    { 1, 1899, 1, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Romance", "9788535910663", 5, "Dom Casmurro" },
                    { 2, 1881, 1, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Romance", "9788535911015", 3, "Memórias Póstumas de Brás Cubas" },
                    { 3, 1977, 2, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Romance", "9788532508126", 4, "A Hora da Estrela" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Livros_AutorId",
                table: "Livros",
                column: "AutorId");

            migrationBuilder.CreateIndex(
                name: "IX_Livros_Isbn",
                table: "Livros",
                column: "Isbn",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Livros");

            migrationBuilder.DropTable(
                name: "Autores");
        }
    }
}
