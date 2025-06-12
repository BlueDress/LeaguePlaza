using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LeaguePlaza.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class MountEntity_AddRating : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "Rating",
                table: "Mounts",
                type: "float",
                nullable: false,
                defaultValue: 0.0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Rating",
                table: "Mounts");
        }
    }
}
