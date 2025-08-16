using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace APIGerenciamento.Migrations
{
    /// <inheritdoc />
    public partial class melhoratabelas : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Cidade",
                table: "Eventos",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "Entrada",
                table: "Eventos",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Cidade",
                table: "Eventos");

            migrationBuilder.DropColumn(
                name: "Entrada",
                table: "Eventos");
        }
    }
}
