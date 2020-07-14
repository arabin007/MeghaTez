using Microsoft.EntityFrameworkCore.Migrations;

namespace Persistence.Migrations
{
    public partial class AddedConsumerRelationship : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "tblConsumers",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    ConsumerId = table.Column<string>(nullable: true),
                    TypeOfConsumer = table.Column<string>(nullable: true),
                    NationalId = table.Column<string>(nullable: true),
                    MeterId = table.Column<string>(nullable: true),
                    MeterCapacity = table.Column<string>(nullable: true),
                    AppIdFK = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tblConsumers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_tblConsumers_AspNetUsers_AppIdFK",
                        column: x => x.AppIdFK,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_tblConsumers_AppIdFK",
                table: "tblConsumers",
                column: "AppIdFK",
                unique: true,
                filter: "[AppIdFK] IS NOT NULL");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "tblConsumers");
        }
    }
}
