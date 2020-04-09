using Microsoft.EntityFrameworkCore.Migrations;

namespace Database.Migrations
{
    public partial class 重命名CityCompany : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CityCompanie_City_CityId",
                table: "CityCompanie");

            migrationBuilder.DropForeignKey(
                name: "FK_CityCompanie_Company_CompanyId",
                table: "CityCompanie");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CityCompanie",
                table: "CityCompanie");

            migrationBuilder.RenameTable(
                name: "CityCompanie",
                newName: "CityCompany");

            migrationBuilder.RenameIndex(
                name: "IX_CityCompanie_CompanyId",
                table: "CityCompany",
                newName: "IX_CityCompany_CompanyId");

            migrationBuilder.RenameIndex(
                name: "IX_CityCompanie_CityId",
                table: "CityCompany",
                newName: "IX_CityCompany_CityId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CityCompany",
                table: "CityCompany",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CityCompany_City_CityId",
                table: "CityCompany",
                column: "CityId",
                principalTable: "City",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CityCompany_Company_CompanyId",
                table: "CityCompany",
                column: "CompanyId",
                principalTable: "Company",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CityCompany_City_CityId",
                table: "CityCompany");

            migrationBuilder.DropForeignKey(
                name: "FK_CityCompany_Company_CompanyId",
                table: "CityCompany");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CityCompany",
                table: "CityCompany");

            migrationBuilder.RenameTable(
                name: "CityCompany",
                newName: "CityCompanie");

            migrationBuilder.RenameIndex(
                name: "IX_CityCompany_CompanyId",
                table: "CityCompanie",
                newName: "IX_CityCompanie_CompanyId");

            migrationBuilder.RenameIndex(
                name: "IX_CityCompany_CityId",
                table: "CityCompanie",
                newName: "IX_CityCompanie_CityId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CityCompanie",
                table: "CityCompanie",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CityCompanie_City_CityId",
                table: "CityCompanie",
                column: "CityId",
                principalTable: "City",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CityCompanie_Company_CompanyId",
                table: "CityCompanie",
                column: "CompanyId",
                principalTable: "Company",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
