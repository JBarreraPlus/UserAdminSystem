using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UserAdminSystem.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddModules : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ModulePermissions_Module_ModuleId",
                table: "ModulePermissions");

            migrationBuilder.DropForeignKey(
                name: "FK_RoleModules_Module_ModuleId",
                table: "RoleModules");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Module",
                table: "Module");

            migrationBuilder.RenameTable(
                name: "Module",
                newName: "Modules");

            migrationBuilder.RenameIndex(
                name: "IX_Module_Name",
                table: "Modules",
                newName: "IX_Modules_Name");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Modules",
                table: "Modules",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ModulePermissions_Modules_ModuleId",
                table: "ModulePermissions",
                column: "ModuleId",
                principalTable: "Modules",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RoleModules_Modules_ModuleId",
                table: "RoleModules",
                column: "ModuleId",
                principalTable: "Modules",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ModulePermissions_Modules_ModuleId",
                table: "ModulePermissions");

            migrationBuilder.DropForeignKey(
                name: "FK_RoleModules_Modules_ModuleId",
                table: "RoleModules");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Modules",
                table: "Modules");

            migrationBuilder.RenameTable(
                name: "Modules",
                newName: "Module");

            migrationBuilder.RenameIndex(
                name: "IX_Modules_Name",
                table: "Module",
                newName: "IX_Module_Name");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Module",
                table: "Module",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ModulePermissions_Module_ModuleId",
                table: "ModulePermissions",
                column: "ModuleId",
                principalTable: "Module",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RoleModules_Module_ModuleId",
                table: "RoleModules",
                column: "ModuleId",
                principalTable: "Module",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
