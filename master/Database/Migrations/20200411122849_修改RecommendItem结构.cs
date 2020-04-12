using Microsoft.EntityFrameworkCore.Migrations;

namespace Database.Migrations
{
    public partial class 修改RecommendItem结构 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ImageType",
                table: "RecommendItems",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageType",
                table: "RecommendItems");
        }
    }
}
