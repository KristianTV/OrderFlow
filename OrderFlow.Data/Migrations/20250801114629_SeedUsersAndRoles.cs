using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace OrderFlow.Data.Migrations
{
    /// <inheritdoc />
    public partial class SeedUsersAndRoles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "PaymentDescription",
                table: "Payments",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { new Guid("2216da39-13fa-479d-a0e8-12c7083001cc"), null, "User", "USER" },
                    { new Guid("500d6eef-f606-4bd6-84fd-69bcfcf04853"), null, "Admin", "ADMIN" },
                    { new Guid("581b6246-83b0-400f-8e27-4ad126d8e2d0"), null, "Speditor", "SPEDITOR" },
                    { new Guid("f41348a8-9404-4e29-88e5-378829f169d8"), null, "Driver", "DRIVER" }
                });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Email", "EmailConfirmed", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[,]
                {
                    { new Guid("3b0e4d23-ebf1-4431-a1ad-0c3fab984165"), 0, "23d82744-6349-4afc-9331-6b3cf67c562a", "user@gmail.com", true, true, null, "USER@GMAIL.COM", "USER", "AQAAAAIAAYagAAAAEEDTSAFuwX+otIlP0ce9loFxrdGarGa2z8kDFoKzsFLj869TKgr5iJ+GhQzZsI8EYw==", "1234567890", true, "1cbce351-1e21-4d69-8304-88d1951c27f8", false, "User" },
                    { new Guid("8d5245b1-1883-44f1-9536-a0733c355b71"), 0, "ed47cb1b-5e6a-458d-b551-2cc0d1f1de61", "speditor@gmail.com", true, true, null, "SPEDITOR@GMAIL.COM", "SPEDITOR", "AQAAAAIAAYagAAAAECqzx8idpPTJ+ea9ZfIs6x41sh+Skm/cq5xBoXSmCFzJWGBcYoQCGZRAt53EcbxyWQ==", "1234567890", true, "d7026306-c0a3-4dae-8b14-d393201c9373", false, "Speditor" },
                    { new Guid("a76a03c2-9927-4173-919f-77cd2b73aa8e"), 0, "04f98ccd-98d1-4306-b246-15ebf8e7b098", "driver@gmail.com", true, true, null, "DRIVER@GMAIL.COM", "DRIVER", "AQAAAAIAAYagAAAAEO/fe53OTdvphbVV4sco6PWY9WzT6UF4MN/p0ZTpv1FZt1FrO+5T3hf1OPZ1UoFR7w==", "1234567890", true, "2851a7f5-2525-405f-b355-fc69d4d09540", false, "Driver" },
                    { new Guid("d5dd15ec-ca54-470b-bc4f-919328a8b0cc"), 0, "b1055d1d-28e3-4a39-b90f-e568791a20f6", "admin@gmail.com", true, true, null, "ADMIN@GMAIL.COM", "ADMIN", "AQAAAAIAAYagAAAAEKTCbcw74tUR/5ZuryEbeRjno4Wt6Friol6N+Ey2KFbZoA3tOWOcO06zgFrUglgrSQ==", "1234567890", true, "2604e2ba-eb78-488c-b335-abcb39af5dc1", false, "Admin" }
                });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[,]
                {
                    { new Guid("2216da39-13fa-479d-a0e8-12c7083001cc"), new Guid("3b0e4d23-ebf1-4431-a1ad-0c3fab984165") },
                    { new Guid("581b6246-83b0-400f-8e27-4ad126d8e2d0"), new Guid("8d5245b1-1883-44f1-9536-a0733c355b71") },
                    { new Guid("f41348a8-9404-4e29-88e5-378829f169d8"), new Guid("a76a03c2-9927-4173-919f-77cd2b73aa8e") },
                    { new Guid("500d6eef-f606-4bd6-84fd-69bcfcf04853"), new Guid("d5dd15ec-ca54-470b-bc4f-919328a8b0cc") }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { new Guid("2216da39-13fa-479d-a0e8-12c7083001cc"), new Guid("3b0e4d23-ebf1-4431-a1ad-0c3fab984165") });

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { new Guid("581b6246-83b0-400f-8e27-4ad126d8e2d0"), new Guid("8d5245b1-1883-44f1-9536-a0733c355b71") });

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { new Guid("f41348a8-9404-4e29-88e5-378829f169d8"), new Guid("a76a03c2-9927-4173-919f-77cd2b73aa8e") });

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { new Guid("500d6eef-f606-4bd6-84fd-69bcfcf04853"), new Guid("d5dd15ec-ca54-470b-bc4f-919328a8b0cc") });

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("2216da39-13fa-479d-a0e8-12c7083001cc"));

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("500d6eef-f606-4bd6-84fd-69bcfcf04853"));

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("581b6246-83b0-400f-8e27-4ad126d8e2d0"));

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("f41348a8-9404-4e29-88e5-378829f169d8"));

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("3b0e4d23-ebf1-4431-a1ad-0c3fab984165"));

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("8d5245b1-1883-44f1-9536-a0733c355b71"));

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("a76a03c2-9927-4173-919f-77cd2b73aa8e"));

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("d5dd15ec-ca54-470b-bc4f-919328a8b0cc"));

            migrationBuilder.AlterColumn<string>(
                name: "PaymentDescription",
                table: "Payments",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true);
        }
    }
}
