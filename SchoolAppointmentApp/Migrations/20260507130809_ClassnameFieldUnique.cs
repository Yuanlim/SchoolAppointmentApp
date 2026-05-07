using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolAppointmentApp.Migrations
{
    /// <inheritdoc />
    public partial class ClassnameFieldUnique : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TumbsUpInfos_MainPosts_MainPostId",
                table: "TumbsUpInfos");

            migrationBuilder.DropForeignKey(
                name: "FK_TumbsUpInfos_Replies_ReplyId",
                table: "TumbsUpInfos");

            migrationBuilder.DropForeignKey(
                name: "FK_TumbsUpInfos_Users_UserId",
                table: "TumbsUpInfos");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TumbsUpInfos",
                table: "TumbsUpInfos");

            migrationBuilder.RenameTable(
                name: "TumbsUpInfos",
                newName: "ThumbsUpInfos");

            migrationBuilder.RenameIndex(
                name: "IX_TumbsUpInfos_UserId",
                table: "ThumbsUpInfos",
                newName: "IX_ThumbsUpInfos_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_TumbsUpInfos_ReplyId_UserId",
                table: "ThumbsUpInfos",
                newName: "IX_ThumbsUpInfos_ReplyId_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_TumbsUpInfos_MainPostId_UserId",
                table: "ThumbsUpInfos",
                newName: "IX_ThumbsUpInfos_MainPostId_UserId");

            migrationBuilder.AlterColumn<int>(
                name: "PointCost",
                table: "Products",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer",
                oldDefaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_ThumbsUpInfos",
                table: "ThumbsUpInfos",
                column: "ThumbsUpInfoId");

            migrationBuilder.CreateIndex(
                name: "IX_SchoolClasses_ClassName",
                table: "SchoolClasses",
                column: "ClassName",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ThumbsUpInfos_MainPosts_MainPostId",
                table: "ThumbsUpInfos",
                column: "MainPostId",
                principalTable: "MainPosts",
                principalColumn: "MainPostId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ThumbsUpInfos_Replies_ReplyId",
                table: "ThumbsUpInfos",
                column: "ReplyId",
                principalTable: "Replies",
                principalColumn: "ReplyId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ThumbsUpInfos_Users_UserId",
                table: "ThumbsUpInfos",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ThumbsUpInfos_MainPosts_MainPostId",
                table: "ThumbsUpInfos");

            migrationBuilder.DropForeignKey(
                name: "FK_ThumbsUpInfos_Replies_ReplyId",
                table: "ThumbsUpInfos");

            migrationBuilder.DropForeignKey(
                name: "FK_ThumbsUpInfos_Users_UserId",
                table: "ThumbsUpInfos");

            migrationBuilder.DropIndex(
                name: "IX_SchoolClasses_ClassName",
                table: "SchoolClasses");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ThumbsUpInfos",
                table: "ThumbsUpInfos");

            migrationBuilder.RenameTable(
                name: "ThumbsUpInfos",
                newName: "TumbsUpInfos");

            migrationBuilder.RenameIndex(
                name: "IX_ThumbsUpInfos_UserId",
                table: "TumbsUpInfos",
                newName: "IX_TumbsUpInfos_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_ThumbsUpInfos_ReplyId_UserId",
                table: "TumbsUpInfos",
                newName: "IX_TumbsUpInfos_ReplyId_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_ThumbsUpInfos_MainPostId_UserId",
                table: "TumbsUpInfos",
                newName: "IX_TumbsUpInfos_MainPostId_UserId");

            migrationBuilder.AlterColumn<int>(
                name: "PointCost",
                table: "Products",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TumbsUpInfos",
                table: "TumbsUpInfos",
                column: "ThumbsUpInfoId");

            migrationBuilder.AddForeignKey(
                name: "FK_TumbsUpInfos_MainPosts_MainPostId",
                table: "TumbsUpInfos",
                column: "MainPostId",
                principalTable: "MainPosts",
                principalColumn: "MainPostId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TumbsUpInfos_Replies_ReplyId",
                table: "TumbsUpInfos",
                column: "ReplyId",
                principalTable: "Replies",
                principalColumn: "ReplyId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TumbsUpInfos_Users_UserId",
                table: "TumbsUpInfos",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
