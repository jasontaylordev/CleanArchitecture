using Microsoft.EntityFrameworkCore.Migrations;

namespace CleanArchitecture.Infrastructure.Persistence.Migrations
{
    public partial class ForeignKeyOverApplicationUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "TodoLists");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "TodoItems");

            migrationBuilder.RenameColumn(
                name: "LastModifiedBy",
                table: "TodoLists",
                newName: "LastModifiedByUserId");

            migrationBuilder.RenameColumn(
                name: "LastModifiedBy",
                table: "TodoItems",
                newName: "LastModifiedByUserId");

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUserId",
                table: "TodoLists",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ModifiedByUserId",
                table: "TodoLists",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUserId",
                table: "TodoItems",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ModifiedByUserId",
                table: "TodoItems",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_TodoLists_CreatedByUserId",
                table: "TodoLists",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_TodoLists_ModifiedByUserId",
                table: "TodoLists",
                column: "ModifiedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_TodoItems_CreatedByUserId",
                table: "TodoItems",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_TodoItems_ModifiedByUserId",
                table: "TodoItems",
                column: "ModifiedByUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_TodoItems_AspNetUsers_CreatedByUserId",
                table: "TodoItems",
                column: "CreatedByUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TodoItems_AspNetUsers_ModifiedByUserId",
                table: "TodoItems",
                column: "ModifiedByUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TodoLists_AspNetUsers_CreatedByUserId",
                table: "TodoLists",
                column: "CreatedByUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TodoLists_AspNetUsers_ModifiedByUserId",
                table: "TodoLists",
                column: "ModifiedByUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TodoItems_AspNetUsers_CreatedByUserId",
                table: "TodoItems");

            migrationBuilder.DropForeignKey(
                name: "FK_TodoItems_AspNetUsers_ModifiedByUserId",
                table: "TodoItems");

            migrationBuilder.DropForeignKey(
                name: "FK_TodoLists_AspNetUsers_CreatedByUserId",
                table: "TodoLists");

            migrationBuilder.DropForeignKey(
                name: "FK_TodoLists_AspNetUsers_ModifiedByUserId",
                table: "TodoLists");

            migrationBuilder.DropIndex(
                name: "IX_TodoLists_CreatedByUserId",
                table: "TodoLists");

            migrationBuilder.DropIndex(
                name: "IX_TodoLists_ModifiedByUserId",
                table: "TodoLists");

            migrationBuilder.DropIndex(
                name: "IX_TodoItems_CreatedByUserId",
                table: "TodoItems");

            migrationBuilder.DropIndex(
                name: "IX_TodoItems_ModifiedByUserId",
                table: "TodoItems");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "TodoLists");

            migrationBuilder.DropColumn(
                name: "ModifiedByUserId",
                table: "TodoLists");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "TodoItems");

            migrationBuilder.DropColumn(
                name: "ModifiedByUserId",
                table: "TodoItems");

            migrationBuilder.RenameColumn(
                name: "LastModifiedByUserId",
                table: "TodoLists",
                newName: "LastModifiedBy");

            migrationBuilder.RenameColumn(
                name: "LastModifiedByUserId",
                table: "TodoItems",
                newName: "LastModifiedBy");

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "TodoLists",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "TodoItems",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
