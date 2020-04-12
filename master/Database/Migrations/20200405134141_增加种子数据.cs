using Microsoft.EntityFrameworkCore.Migrations;

namespace Database.Migrations
{
    public partial class 增加种子数据 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Province",
                columns: new[] { "Id", "Name", "Population" },
                values: new object[] { 1, "广东省", 80000000 });

            migrationBuilder.InsertData(
                table: "Province",
                columns: new[] { "Id", "Name", "Population" },
                values: new object[] { 2, "湖北省", 50000000 });

            migrationBuilder.InsertData(
                table: "Province",
                columns: new[] { "Id", "Name", "Population" },
                values: new object[] { 3, "四川省", 60000000 });

            migrationBuilder.InsertData(
                table: "City",
                columns: new[] { "Id", "AreaCode", "Name", "ProvinceId" },
                values: new object[,]
                {
                    { 1, null, "广州市", 1 },
                    { 2, null, "深圳市", 1 },
                    { 3, null, "佛山市", 1 },
                    { 4, null, "武汉市", 2 },
                    { 5, null, "襄阳市", 2 },
                    { 6, null, "天门市", 2 },
                    { 7, null, "成都市", 3 },
                    { 8, null, "宜宾市", 3 }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "City",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "City",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "City",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "City",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "City",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "City",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "City",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "City",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Province",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Province",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Province",
                keyColumn: "Id",
                keyValue: 3);
        }
    }
}
