using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Samad.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddNewConcilerCartable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_CouncilReviews_RequestId",
                table: "CouncilReviews");

            migrationBuilder.CreateTable(
                name: "RequestCouncilAssignment",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RequestId = table.Column<int>(type: "int", nullable: false),
                    CouncilMemberId = table.Column<int>(type: "int", nullable: false),
                    AssignedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RequestCouncilAssignment", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RequestCouncilAssignment_Requests_RequestId",
                        column: x => x.RequestId,
                        principalTable: "Requests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RequestCouncilAssignment_Users_CouncilMemberId",
                        column: x => x.CouncilMemberId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CouncilReviews_RequestId_CouncilMemberId",
                table: "CouncilReviews",
                columns: new[] { "RequestId", "CouncilMemberId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RequestCouncilAssignment_CouncilMemberId",
                table: "RequestCouncilAssignment",
                column: "CouncilMemberId");

            migrationBuilder.CreateIndex(
                name: "IX_RequestCouncilAssignment_RequestId_CouncilMemberId",
                table: "RequestCouncilAssignment",
                columns: new[] { "RequestId", "CouncilMemberId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RequestCouncilAssignment");

            migrationBuilder.DropIndex(
                name: "IX_CouncilReviews_RequestId_CouncilMemberId",
                table: "CouncilReviews");

            migrationBuilder.CreateIndex(
                name: "IX_CouncilReviews_RequestId",
                table: "CouncilReviews",
                column: "RequestId");
        }
    }
}
