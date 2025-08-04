using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace OrderFlow.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddOptionalTruckToNotifications : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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

            migrationBuilder.AddColumn<Guid>(
                name: "TruckId",
                table: "Notifications",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { new Guid("8ec94a14-bb02-4b0e-a244-e0eb2381eb33"), null, "Speditor", "SPEDITOR" },
                    { new Guid("9451ecbc-52a1-4cf9-abd2-e53387a4a9e5"), null, "Admin", "ADMIN" },
                    { new Guid("a8eec74b-9849-4cb1-b50d-08d72b399c9b"), null, "Driver", "DRIVER" },
                    { new Guid("f4da4019-1117-458f-93df-4fc077748d43"), null, "User", "USER" }
                });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Email", "EmailConfirmed", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[,]
                {
                    { new Guid("afabf75b-6dc5-4d09-9f30-2596c35e0b61"), 0, "7cb710bc-3306-422c-bf83-4c22962cd82a", "user@gmail.com", true, true, null, "USER@GMAIL.COM", "USER", "AQAAAAIAAYagAAAAEHDHzhr0th2RRFuYB/3KfIFQGWFnTPvZkdBKASWiX4uzLtS3pHXBgXwp62uFNGzz4A==", "1234567890", true, "3a898dfd-8268-491a-851a-f3ce8a99a8eb", false, "User" },
                    { new Guid("b16257e5-039f-47fb-acf0-f68b23f4f9f3"), 0, "e0386c0f-4baa-433b-aabc-dc6aaf2ba5ff", "driver@gmail.com", true, true, null, "DRIVER@GMAIL.COM", "DRIVER", "AQAAAAIAAYagAAAAEPuZ01tnmKhAHFO9nXImmyzDwFB1+HDN76HIb6lWf+0bRj6EVxWSTdOlWsIC/SMItQ==", "1234567890", true, "1683b6d2-be30-433c-bbae-ccf56ec72f98", false, "Driver" },
                    { new Guid("b5bc30f8-6e7d-4939-8560-4d6894e9dfa7"), 0, "cb31dcb4-ed56-4c63-ad72-bd71a53fe45b", "admin@gmail.com", true, true, null, "ADMIN@GMAIL.COM", "ADMIN", "AQAAAAIAAYagAAAAEFhcFwgV2rNLeBgostD3W7Z8WIO82o0OahfynZ1tdXLtbKazmsqz8OrMfJ1XQXl1LQ==", "1234567890", true, "beb043c4-429d-49f2-b0e4-0b26c8bd92ff", false, "Admin" },
                    { new Guid("eb9ed273-de23-4578-a88f-db79c4369c0f"), 0, "f4e5d3d1-1140-48b9-bc27-1cdc2d99be50", "speditor@gmail.com", true, true, null, "SPEDITOR@GMAIL.COM", "SPEDITOR", "AQAAAAIAAYagAAAAENgwdQ2O6bX+fXoEmRHuiX9EEYARhHUMp08ZwromGG/KIVRHGWsrEuNyeHIagGswfQ==", "1234567890", true, "a7e81cca-9915-4ea5-8a8a-f8c468f08573", false, "Speditor" }
                });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[,]
                {
                    { new Guid("f4da4019-1117-458f-93df-4fc077748d43"), new Guid("afabf75b-6dc5-4d09-9f30-2596c35e0b61") },
                    { new Guid("a8eec74b-9849-4cb1-b50d-08d72b399c9b"), new Guid("b16257e5-039f-47fb-acf0-f68b23f4f9f3") },
                    { new Guid("9451ecbc-52a1-4cf9-abd2-e53387a4a9e5"), new Guid("b5bc30f8-6e7d-4939-8560-4d6894e9dfa7") },
                    { new Guid("8ec94a14-bb02-4b0e-a244-e0eb2381eb33"), new Guid("eb9ed273-de23-4578-a88f-db79c4369c0f") }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_TruckId",
                table: "Notifications",
                column: "TruckId");

            migrationBuilder.AddForeignKey(
                name: "FK_Notifications_Trucks_TruckId",
                table: "Notifications",
                column: "TruckId",
                principalTable: "Trucks",
                principalColumn: "TruckID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Notifications_Trucks_TruckId",
                table: "Notifications");

            migrationBuilder.DropIndex(
                name: "IX_Notifications_TruckId",
                table: "Notifications");

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { new Guid("f4da4019-1117-458f-93df-4fc077748d43"), new Guid("afabf75b-6dc5-4d09-9f30-2596c35e0b61") });

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { new Guid("a8eec74b-9849-4cb1-b50d-08d72b399c9b"), new Guid("b16257e5-039f-47fb-acf0-f68b23f4f9f3") });

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { new Guid("9451ecbc-52a1-4cf9-abd2-e53387a4a9e5"), new Guid("b5bc30f8-6e7d-4939-8560-4d6894e9dfa7") });

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { new Guid("8ec94a14-bb02-4b0e-a244-e0eb2381eb33"), new Guid("eb9ed273-de23-4578-a88f-db79c4369c0f") });

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("8ec94a14-bb02-4b0e-a244-e0eb2381eb33"));

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("9451ecbc-52a1-4cf9-abd2-e53387a4a9e5"));

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("a8eec74b-9849-4cb1-b50d-08d72b399c9b"));

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("f4da4019-1117-458f-93df-4fc077748d43"));

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("afabf75b-6dc5-4d09-9f30-2596c35e0b61"));

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("b16257e5-039f-47fb-acf0-f68b23f4f9f3"));

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("b5bc30f8-6e7d-4939-8560-4d6894e9dfa7"));

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("eb9ed273-de23-4578-a88f-db79c4369c0f"));

            migrationBuilder.DropColumn(
                name: "TruckId",
                table: "Notifications");

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
    }
}
