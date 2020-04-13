using Microsoft.EntityFrameworkCore.Migrations;

namespace Database.Migrations
{
    public partial class RecommendItems表增加RecommendType字段 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "RecommendType",
                table: "RecommendItems",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RecommendType",
                table: "RecommendItems");
        }
    }
}
