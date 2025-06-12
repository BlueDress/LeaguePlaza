using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LeaguePlaza.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddMountRatingEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Rating",
                table: "Mounts");

            migrationBuilder.CreateTable(
                name: "MountRatings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Rating = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    MountId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MountRatings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MountRatings_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MountRatings_Mounts_MountId",
                        column: x => x.MountId,
                        principalTable: "Mounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MountRatings_MountId",
                table: "MountRatings",
                column: "MountId");

            migrationBuilder.CreateIndex(
                name: "IX_MountRatings_UserId",
                table: "MountRatings",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MountRatings");

            migrationBuilder.AddColumn<double>(
                name: "Rating",
                table: "Mounts",
                type: "float",
                nullable: false,
                defaultValue: 0.0);
        }
    }
}
