using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AlquimiaEsencial.Data.Migrations
{
    /// <inheritdoc />
    public partial class AgregarProductoAPedido : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ProductoId",
                table: "Pedidos",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Pedidos_ProductoId",
                table: "Pedidos",
                column: "ProductoId");

            migrationBuilder.AddForeignKey(
                name: "FK_Pedidos_Productos_ProductoId",
                table: "Pedidos",
                column: "ProductoId",
                principalTable: "Productos",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Pedidos_Productos_ProductoId",
                table: "Pedidos");

            migrationBuilder.DropIndex(
                name: "IX_Pedidos_ProductoId",
                table: "Pedidos");

            migrationBuilder.DropColumn(
                name: "ProductoId",
                table: "Pedidos");
        }
    }
}
