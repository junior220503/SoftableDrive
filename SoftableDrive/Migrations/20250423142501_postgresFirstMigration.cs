using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SoftableDrive.Migrations
{
    /// <inheritdoc />
    public partial class postgresFirstMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
            ALTER TABLE ""Files"" 
            ALTER COLUMN ""UploadTime"" TYPE timestamp with time zone 
            USING ""UploadTime""::timestamp with time zone;
            ");

            migrationBuilder.AlterColumn<long>(
                name: "Size",
                table: "Files",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Files",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.Sql(@"
            ALTER TABLE ""Files"" 
            ALTER COLUMN ""Id"" TYPE uuid 
            USING ""Id""::uuid;
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "UploadTime",
                table: "Files",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<int>(
                name: "Size",
                table: "Files",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Files",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "Files",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid");
        }
    }
}
