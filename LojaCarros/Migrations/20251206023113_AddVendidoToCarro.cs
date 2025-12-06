using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LojaCarros.Migrations
{
    /// <inheritdoc />
    public partial class AddVendidoToCarro : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Vendido",
                table: "Carros",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Vendido",
                table: "Carros");
        }
    }
}
