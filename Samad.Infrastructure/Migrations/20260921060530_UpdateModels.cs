using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Samad.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateModels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Requests_Users_SecretaryId",
                table: "Requests");

            migrationBuilder.DropIndex(
                name: "IX_Requests_SecretaryId",
                table: "Requests");

            migrationBuilder.DropColumn(
                name: "FinalDecisionDate",
                table: "Requests");

            migrationBuilder.DropColumn(
                name: "FinalSecretaryComment",
                table: "Requests");

            migrationBuilder.DropColumn(
                name: "SecretaryId",
                table: "Requests");

            migrationBuilder.AddColumn<DateTime>(
                name: "DeadlineAt",
                table: "Requests",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<byte>(
                name: "DocumentType",
                table: "RequestDocuments",
                type: "tinyint",
                nullable: false,
                defaultValue: (byte)7);

            migrationBuilder.AddColumn<byte>(
                name: "AssignmentType",
                table: "RequestCouncilAssignment",
                type: "tinyint",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.CreateTable(
                name: "CouncilMember",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    Type = table.Column<byte>(type: "tinyint", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CouncilMember", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CouncilMember_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CouncilSignature",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RequestId = table.Column<int>(type: "int", nullable: false),
                    CouncilMemberId = table.Column<int>(type: "int", nullable: false),
                    SignedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsSigned = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CouncilSignature", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CouncilSignature_Requests_RequestId",
                        column: x => x.RequestId,
                        principalTable: "Requests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CouncilSignature_Users_CouncilMemberId",
                        column: x => x.CouncilMemberId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RequestStatusHistory",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RequestId = table.Column<int>(type: "int", nullable: false),
                    FromStatus = table.Column<byte>(type: "tinyint", nullable: true),
                    ToStatus = table.Column<byte>(type: "tinyint", nullable: false),
                    ChangedByUserId = table.Column<int>(type: "int", nullable: true),
                    ChangedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Comment = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RequestStatusHistory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RequestStatusHistory_Requests_RequestId",
                        column: x => x.RequestId,
                        principalTable: "Requests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RequestStatusHistory_Users_ChangedByUserId",
                        column: x => x.ChangedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SecretaryDecision",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RequestId = table.Column<int>(type: "int", nullable: false),
                    SecretaryId = table.Column<int>(type: "int", nullable: false),
                    Stage = table.Column<byte>(type: "tinyint", nullable: false),
                    Decision = table.Column<byte>(type: "tinyint", nullable: false),
                    Comment = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SecretaryDecision", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SecretaryDecision_Requests_RequestId",
                        column: x => x.RequestId,
                        principalTable: "Requests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SecretaryDecision_Users_SecretaryId",
                        column: x => x.SecretaryId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Requests_DeadlineAt",
                table: "Requests",
                column: "DeadlineAt");

            migrationBuilder.CreateIndex(
                name: "IX_CouncilMember_UserId",
                table: "CouncilMember",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CouncilSignature_CouncilMemberId",
                table: "CouncilSignature",
                column: "CouncilMemberId");

            migrationBuilder.CreateIndex(
                name: "IX_CouncilSignature_RequestId_CouncilMemberId",
                table: "CouncilSignature",
                columns: new[] { "RequestId", "CouncilMemberId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RequestStatusHistory_ChangedByUserId",
                table: "RequestStatusHistory",
                column: "ChangedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_RequestStatusHistory_RequestId_ChangedAt",
                table: "RequestStatusHistory",
                columns: new[] { "RequestId", "ChangedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_SecretaryDecision_RequestId_Stage_CreatedAt",
                table: "SecretaryDecision",
                columns: new[] { "RequestId", "Stage", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_SecretaryDecision_SecretaryId",
                table: "SecretaryDecision",
                column: "SecretaryId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CouncilMember");

            migrationBuilder.DropTable(
                name: "CouncilSignature");

            migrationBuilder.DropTable(
                name: "RequestStatusHistory");

            migrationBuilder.DropTable(
                name: "SecretaryDecision");

            migrationBuilder.DropIndex(
                name: "IX_Requests_DeadlineAt",
                table: "Requests");

            migrationBuilder.DropColumn(
                name: "DeadlineAt",
                table: "Requests");

            migrationBuilder.DropColumn(
                name: "DocumentType",
                table: "RequestDocuments");

            migrationBuilder.DropColumn(
                name: "AssignmentType",
                table: "RequestCouncilAssignment");

            migrationBuilder.AddColumn<DateTime>(
                name: "FinalDecisionDate",
                table: "Requests",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FinalSecretaryComment",
                table: "Requests",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SecretaryId",
                table: "Requests",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Requests_SecretaryId",
                table: "Requests",
                column: "SecretaryId");

            migrationBuilder.AddForeignKey(
                name: "FK_Requests_Users_SecretaryId",
                table: "Requests",
                column: "SecretaryId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
