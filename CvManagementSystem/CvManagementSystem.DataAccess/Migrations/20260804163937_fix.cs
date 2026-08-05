using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UserService.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class fix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_PositionApiTokens_PositionId",
                table: "PositionApiTokens");

            migrationBuilder.CreateIndex(
                name: "IX_PositionApiTokens_PositionId",
                table: "PositionApiTokens",
                column: "PositionId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_PositionApiTokens_PositionId",
                table: "PositionApiTokens");

            migrationBuilder.CreateIndex(
                name: "IX_PositionApiTokens_PositionId",
                table: "PositionApiTokens",
                column: "PositionId",
                unique: true);
        }
    }
}
