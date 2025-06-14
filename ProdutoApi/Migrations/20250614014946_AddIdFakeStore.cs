using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProdutoApi.Migrations
{
    /// <inheritdoc />
    public partial class AddIdFakeStore : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "IdFakeStore",
                table: "Produtos",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IdFakeStore",
                table: "Produtos");
        }
    }
}
