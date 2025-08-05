using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace OrderFlow.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddLoadCapacityAndAssignedDateAndRefactorTruckOrderRelationshipAndTruckStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TrucksOrders_Orders_OrderID",
                table: "TrucksOrders");

            migrationBuilder.DropForeignKey(
                name: "FK_TrucksOrders_Trucks_TruckID",
                table: "TrucksOrders");

            migrationBuilder.DropIndex(
                name: "IX_TrucksOrders_OrderID",
                table: "TrucksOrders");

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

            migrationBuilder.AddColumn<DateTime>(
                name: "AssignedDate",
                table: "TrucksOrders",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "DeliverAddress",
                table: "TrucksOrders",
                type: "nvarchar(250)",
                maxLength: 250,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "DeliveryDate",
                table: "TrucksOrders",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Status",
                table: "Trucks",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AddColumn<int>(
                name: "LoadCapacity",
                table: "Orders",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { new Guid("18295388-f647-46f2-8f57-6e712cbbdc87"), null, "Admin", "ADMIN" },
                    { new Guid("255813d2-d369-4a9d-916b-1f0ad1ef42d9"), null, "Speditor", "SPEDITOR" },
                    { new Guid("63567691-b254-4f41-a444-5aad3b8cc1b1"), null, "Driver", "DRIVER" },
                    { new Guid("dd91d1f6-5413-4276-be31-3e78b7d17113"), null, "User", "USER" }
                });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Email", "EmailConfirmed", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[,]
                {
                    { new Guid("32cac083-4221-4058-b71e-b2ae9b2a9d15"), 0, "524d2d1e-ff17-41e3-8676-a5d2f71f67e0", "driver@gmail.com", true, true, null, "DRIVER@GMAIL.COM", "DRIVER", "AQAAAAIAAYagAAAAEBdl1T8JFtEsDYmxeN6xWWbsYd5oui1PwziUyG4MjZoHiPe9HYI4QVHqieT77rhbvw==", "1234567890", true, "f5e928f6-bd0a-4ba6-835c-6239168ec3ea", false, "Driver" },
                    { new Guid("39e4540c-d048-49a7-ba70-6f6dd545cc4f"), 0, "12505ba3-0c1c-4f22-8ae1-38f6e6946e6b", "user@gmail.com", true, true, null, "USER@GMAIL.COM", "USER", "AQAAAAIAAYagAAAAEBPOzpP9VshGx8HU3hgrrQQ9TZ1DWNL9NLYf5sNWURtHWvETGonOi+fKgMG8R2Hk3A==", "1234567890", true, "f665f43e-6a03-4165-bdc6-4d8700e3eeb3", false, "User" },
                    { new Guid("894d8e03-9256-484a-9b00-70ba94722652"), 0, "c4dc7d21-893f-4267-bca1-76a0e83f65e8", "admin@gmail.com", true, true, null, "ADMIN@GMAIL.COM", "ADMIN", "AQAAAAIAAYagAAAAEIAY8Gg+SnzPvnjSXBUzV6/Ur/8D6PV/cUAXa74onTrHIWp2WtKWa87WD+D78hIUyw==", "1234567890", true, "9efa6256-bca9-403b-b211-cc6f4391875f", false, "Admin" },
                    { new Guid("ba04a88d-fb11-496e-a137-81a8e6cca776"), 0, "d5882f69-902d-4da3-98e6-1019a227e9d5", "speditor@gmail.com", true, true, null, "SPEDITOR@GMAIL.COM", "SPEDITOR", "AQAAAAIAAYagAAAAEHGFdTlCvR0BAwFeM8BdUcm1a2N8/aQBrDR/eSHvtIRBR5GMXwmo+yhUojUve2ENYQ==", "1234567890", true, "cfb4b061-9b30-4202-b9fc-558c50726efb", false, "Speditor" }
                });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[,]
                {
                    { new Guid("63567691-b254-4f41-a444-5aad3b8cc1b1"), new Guid("32cac083-4221-4058-b71e-b2ae9b2a9d15") },
                    { new Guid("dd91d1f6-5413-4276-be31-3e78b7d17113"), new Guid("39e4540c-d048-49a7-ba70-6f6dd545cc4f") },
                    { new Guid("18295388-f647-46f2-8f57-6e712cbbdc87"), new Guid("894d8e03-9256-484a-9b00-70ba94722652") },
                    { new Guid("255813d2-d369-4a9d-916b-1f0ad1ef42d9"), new Guid("ba04a88d-fb11-496e-a137-81a8e6cca776") }
                });

            migrationBuilder.AddForeignKey(
                name: "FK_TrucksOrders_Orders_OrderID",
                table: "TrucksOrders",
                column: "OrderID",
                principalTable: "Orders",
                principalColumn: "OrderID");

            migrationBuilder.AddForeignKey(
                name: "FK_TrucksOrders_Trucks_TruckID",
                table: "TrucksOrders",
                column: "TruckID",
                principalTable: "Trucks",
                principalColumn: "TruckID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TrucksOrders_Orders_OrderID",
                table: "TrucksOrders");

            migrationBuilder.DropForeignKey(
                name: "FK_TrucksOrders_Trucks_TruckID",
                table: "TrucksOrders");

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { new Guid("63567691-b254-4f41-a444-5aad3b8cc1b1"), new Guid("32cac083-4221-4058-b71e-b2ae9b2a9d15") });

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { new Guid("dd91d1f6-5413-4276-be31-3e78b7d17113"), new Guid("39e4540c-d048-49a7-ba70-6f6dd545cc4f") });

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { new Guid("18295388-f647-46f2-8f57-6e712cbbdc87"), new Guid("894d8e03-9256-484a-9b00-70ba94722652") });

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { new Guid("255813d2-d369-4a9d-916b-1f0ad1ef42d9"), new Guid("ba04a88d-fb11-496e-a137-81a8e6cca776") });

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("18295388-f647-46f2-8f57-6e712cbbdc87"));

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("255813d2-d369-4a9d-916b-1f0ad1ef42d9"));

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("63567691-b254-4f41-a444-5aad3b8cc1b1"));

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("dd91d1f6-5413-4276-be31-3e78b7d17113"));

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("32cac083-4221-4058-b71e-b2ae9b2a9d15"));

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("39e4540c-d048-49a7-ba70-6f6dd545cc4f"));

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("894d8e03-9256-484a-9b00-70ba94722652"));

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("ba04a88d-fb11-496e-a137-81a8e6cca776"));

            migrationBuilder.DropColumn(
                name: "AssignedDate",
                table: "TrucksOrders");

            migrationBuilder.DropColumn(
                name: "DeliverAddress",
                table: "TrucksOrders");

            migrationBuilder.DropColumn(
                name: "DeliveryDate",
                table: "TrucksOrders");

            migrationBuilder.DropColumn(
                name: "LoadCapacity",
                table: "Orders");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "Trucks",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

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

            migrationBuilder.CreateIndex(
                name: "IX_TrucksOrders_OrderID",
                table: "TrucksOrders",
                column: "OrderID",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_TrucksOrders_Orders_OrderID",
                table: "TrucksOrders",
                column: "OrderID",
                principalTable: "Orders",
                principalColumn: "OrderID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TrucksOrders_Trucks_TruckID",
                table: "TrucksOrders",
                column: "TruckID",
                principalTable: "Trucks",
                principalColumn: "TruckID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
