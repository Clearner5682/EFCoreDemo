using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Database.Migrations
{
    public partial class 增加MoguGoodItem表 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MoguGoodItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreateTime = table.Column<DateTime>(nullable: false),
                    LastUpdateTime = table.Column<DateTime>(nullable: false),
                    tradeItemId = table.Column<string>(nullable: true),
                    itemType = table.Column<int>(nullable: false),
                    img = table.Column<string>(nullable: true),
                    clientUrl = table.Column<string>(nullable: true),
                    link = table.Column<string>(nullable: true),
                    itemMarks = table.Column<string>(nullable: true),
                    acm = table.Column<string>(nullable: true),
                    title = table.Column<string>(nullable: true),
                    type = table.Column<int>(nullable: false),
                    cparam = table.Column<string>(nullable: true),
                    orgPrice = table.Column<decimal>(nullable: false),
                    useTitle = table.Column<bool>(nullable: false),
                    price = table.Column<decimal>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MoguGoodItems", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MoguGoodItems");
        }
    }
}
