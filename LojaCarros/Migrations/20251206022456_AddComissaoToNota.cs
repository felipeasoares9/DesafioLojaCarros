using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LojaCarros.Migrations
{
    /// <inheritdoc />
    public partial class AddComissaoToNota : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "Comissao",
                table: "Notas",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Comissao",
                table: "Notas");
        }
    }
}
