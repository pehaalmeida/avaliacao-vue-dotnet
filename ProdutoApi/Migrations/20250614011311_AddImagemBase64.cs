using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProdutoApi.Migrations
{
    /// <inheritdoc />
    public partial class AddImagemBase64 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Imagem",
                table: "Produtos");

            migrationBuilder.AddColumn<string>(
                name: "ImagemBase64",
                table: "Produtos",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImagemBase64",
                table: "Produtos");

            migrationBuilder.AddColumn<byte[]>(
                name: "Imagem",
                table: "Produtos",
                type: "varbinary(max)",
                nullable: true);
        }
    }
}
