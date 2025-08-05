using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace OrderFlow.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddedTruckOrderStatusToTruckOrder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "TrucksOrders",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { new Guid("38f712e4-5106-47fc-b591-64b17dfda98d"), null, "Speditor", "SPEDITOR" },
                    { new Guid("42c7bfc0-b7b3-4614-a824-5ebc2df06b58"), null, "User", "USER" },
                    { new Guid("60ff4740-66a0-4cd1-b37a-464d47a6643f"), null, "Driver", "DRIVER" },
                    { new Guid("e622256a-bbad-4a6d-8f7f-1ad64c4fbff0"), null, "Admin", "ADMIN" }
                });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Email", "EmailConfirmed", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[,]
                {
                    { new Guid("02c7a524-25a9-4eaf-8824-9fd7f63102de"), 0, "a8f21416-c537-4166-9d30-b4e22e02acfa", "driver@gmail.com", true, true, null, "DRIVER@GMAIL.COM", "DRIVER", "AQAAAAIAAYagAAAAEI4z0c9h4Je9m9UftshOl1EgSzmXdkWtBWxaKEUR2Co5iihTcIzVheLD7emQ9s/cUA==", "1234567890", true, "83cb6dee-c5f1-4b28-8b6d-707b7329855e", false, "Driver" },
                    { new Guid("1413b3f2-7d67-4d96-8eba-98dc561ab855"), 0, "3eb0de4f-3b07-4daf-b198-0742098dc7c2", "user@gmail.com", true, true, null, "USER@GMAIL.COM", "USER", "AQAAAAIAAYagAAAAEOTZ9b1aqA3LpWes5iV2oVH7zxu8iIZWjr5RD376lJsP8W8tPRVhh3aYql2+oepnaA==", "1234567890", true, "4a61533a-2dfe-456d-bd83-aefc1ad8b2e0", false, "User" },
                    { new Guid("b9621218-5d9d-403d-8a28-a0b4aeadffee"), 0, "d0bbeac1-5ea6-4426-92e9-d58dbe8c2c0c", "speditor@gmail.com", true, true, null, "SPEDITOR@GMAIL.COM", "SPEDITOR", "AQAAAAIAAYagAAAAEPEdy6cqy8f6gIQmhVxv9VIm8GZ+h2WCYYb4QFCvdEl8FtfgBnUfkBtMJqd55Ayg5w==", "1234567890", true, "abec831a-ed5a-423f-ac20-db0795a0bba2", false, "Speditor" },
                    { new Guid("be58cb43-41ea-450f-8d6e-e772337ed88e"), 0, "d9cbf1f1-1ac5-4802-a7ed-9a8a7c1fa351", "admin@gmail.com", true, true, null, "ADMIN@GMAIL.COM", "ADMIN", "AQAAAAIAAYagAAAAEBgGzbuyPGYgcC3cfAe/DBYEf5UZBBmRdtPKDkBwzvtHHoYguFE9A+n1gA4oBtsufg==", "1234567890", true, "ab8bc231-ecc4-4c3a-9b8d-99f908fc0dba", false, "Admin" }
                });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[,]
                {
                    { new Guid("60ff4740-66a0-4cd1-b37a-464d47a6643f"), new Guid("02c7a524-25a9-4eaf-8824-9fd7f63102de") },
                    { new Guid("42c7bfc0-b7b3-4614-a824-5ebc2df06b58"), new Guid("1413b3f2-7d67-4d96-8eba-98dc561ab855") },
                    { new Guid("38f712e4-5106-47fc-b591-64b17dfda98d"), new Guid("b9621218-5d9d-403d-8a28-a0b4aeadffee") },
                    { new Guid("e622256a-bbad-4a6d-8f7f-1ad64c4fbff0"), new Guid("be58cb43-41ea-450f-8d6e-e772337ed88e") }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { new Guid("60ff4740-66a0-4cd1-b37a-464d47a6643f"), new Guid("02c7a524-25a9-4eaf-8824-9fd7f63102de") });

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { new Guid("42c7bfc0-b7b3-4614-a824-5ebc2df06b58"), new Guid("1413b3f2-7d67-4d96-8eba-98dc561ab855") });

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { new Guid("38f712e4-5106-47fc-b591-64b17dfda98d"), new Guid("b9621218-5d9d-403d-8a28-a0b4aeadffee") });

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { new Guid("e622256a-bbad-4a6d-8f7f-1ad64c4fbff0"), new Guid("be58cb43-41ea-450f-8d6e-e772337ed88e") });

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("38f712e4-5106-47fc-b591-64b17dfda98d"));

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("42c7bfc0-b7b3-4614-a824-5ebc2df06b58"));

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("60ff4740-66a0-4cd1-b37a-464d47a6643f"));

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("e622256a-bbad-4a6d-8f7f-1ad64c4fbff0"));

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("02c7a524-25a9-4eaf-8824-9fd7f63102de"));

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("1413b3f2-7d67-4d96-8eba-98dc561ab855"));

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("b9621218-5d9d-403d-8a28-a0b4aeadffee"));

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("be58cb43-41ea-450f-8d6e-e772337ed88e"));

            migrationBuilder.DropColumn(
                name: "Status",
                table: "TrucksOrders");

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
        }
    }
}
