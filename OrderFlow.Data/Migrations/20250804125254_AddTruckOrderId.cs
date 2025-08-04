using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace OrderFlow.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddTruckOrderId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_TrucksOrders",
                table: "TrucksOrders");

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

            migrationBuilder.AddColumn<Guid>(
                name: "TruckOrderId",
                table: "TrucksOrders",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddPrimaryKey(
                name: "PK_TrucksOrders",
                table: "TrucksOrders",
                column: "TruckOrderId");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { new Guid("3889245d-42c5-4dae-9ea0-aa9a2c2863b4"), null, "Speditor", "SPEDITOR" },
                    { new Guid("6bf248f8-b35e-443c-98de-2574a5430d2e"), null, "Admin", "ADMIN" },
                    { new Guid("ebc808de-3da4-487f-bc7e-be966a4b638b"), null, "User", "USER" },
                    { new Guid("f5097135-7c3f-409e-9847-9590f865645d"), null, "Driver", "DRIVER" }
                });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Email", "EmailConfirmed", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[,]
                {
                    { new Guid("8fb45fbb-c4f3-49fc-a917-2ff2d11d5966"), 0, "3ebf37c4-f609-42aa-9f30-9690fab163bc", "driver@gmail.com", true, true, null, "DRIVER@GMAIL.COM", "DRIVER", "AQAAAAIAAYagAAAAECuXVW/GvO4q8Pp8kyHq8otxX4+F6qFgkljS9X2RXonPo6dopAA26hy0qGiHUL5NSg==", "1234567890", true, "2ca07fd8-7152-44aa-9c70-229b0200b586", false, "Driver" },
                    { new Guid("945fdf06-dd29-4c55-8c9e-a6d064c1b4b4"), 0, "1157bb8e-3269-4385-bbb1-c8dae41a0384", "speditor@gmail.com", true, true, null, "SPEDITOR@GMAIL.COM", "SPEDITOR", "AQAAAAIAAYagAAAAEJZfAVDFImlSM0KszQYf287plqefD9+KKXj6k1iWDRZpsKADlTUiWXx/pJvBiWNalg==", "1234567890", true, "df998dfd-104e-4a67-91a4-1340e394a6ac", false, "Speditor" },
                    { new Guid("c61ef396-3054-454f-ba29-221941ed9449"), 0, "4b81703e-2dbc-4590-9e20-3b51873d5b24", "user@gmail.com", true, true, null, "USER@GMAIL.COM", "USER", "AQAAAAIAAYagAAAAEDrvtdIOHsZez++BKIAdhYdT86TbsiDv5KUprTpYichqFSYz12kzZ8OiD7xDnMuSmQ==", "1234567890", true, "34bc235a-97d7-40d8-a08e-bfca4dee241b", false, "User" },
                    { new Guid("e3d260ba-9569-4d10-8763-3de180aa8c62"), 0, "b71fab4f-84d2-420d-8e69-bde21c4ee27b", "admin@gmail.com", true, true, null, "ADMIN@GMAIL.COM", "ADMIN", "AQAAAAIAAYagAAAAEGZRi7VhuVAPZoKFyOFnDJRzR+Mv0dwfWfbzKeNLqN1idwTwa+0lwu5fNctpopnxMg==", "1234567890", true, "6aca5c28-77f7-4193-8655-8706e8e03986", false, "Admin" }
                });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[,]
                {
                    { new Guid("f5097135-7c3f-409e-9847-9590f865645d"), new Guid("8fb45fbb-c4f3-49fc-a917-2ff2d11d5966") },
                    { new Guid("3889245d-42c5-4dae-9ea0-aa9a2c2863b4"), new Guid("945fdf06-dd29-4c55-8c9e-a6d064c1b4b4") },
                    { new Guid("ebc808de-3da4-487f-bc7e-be966a4b638b"), new Guid("c61ef396-3054-454f-ba29-221941ed9449") },
                    { new Guid("6bf248f8-b35e-443c-98de-2574a5430d2e"), new Guid("e3d260ba-9569-4d10-8763-3de180aa8c62") }
                });

            migrationBuilder.CreateIndex(
                name: "IX_TrucksOrders_OrderID",
                table: "TrucksOrders",
                column: "OrderID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_TrucksOrders",
                table: "TrucksOrders");

            migrationBuilder.DropIndex(
                name: "IX_TrucksOrders_OrderID",
                table: "TrucksOrders");

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { new Guid("f5097135-7c3f-409e-9847-9590f865645d"), new Guid("8fb45fbb-c4f3-49fc-a917-2ff2d11d5966") });

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { new Guid("3889245d-42c5-4dae-9ea0-aa9a2c2863b4"), new Guid("945fdf06-dd29-4c55-8c9e-a6d064c1b4b4") });

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { new Guid("ebc808de-3da4-487f-bc7e-be966a4b638b"), new Guid("c61ef396-3054-454f-ba29-221941ed9449") });

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { new Guid("6bf248f8-b35e-443c-98de-2574a5430d2e"), new Guid("e3d260ba-9569-4d10-8763-3de180aa8c62") });

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("3889245d-42c5-4dae-9ea0-aa9a2c2863b4"));

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("6bf248f8-b35e-443c-98de-2574a5430d2e"));

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("ebc808de-3da4-487f-bc7e-be966a4b638b"));

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("f5097135-7c3f-409e-9847-9590f865645d"));

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("8fb45fbb-c4f3-49fc-a917-2ff2d11d5966"));

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("945fdf06-dd29-4c55-8c9e-a6d064c1b4b4"));

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("c61ef396-3054-454f-ba29-221941ed9449"));

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("e3d260ba-9569-4d10-8763-3de180aa8c62"));

            migrationBuilder.DropColumn(
                name: "TruckOrderId",
                table: "TrucksOrders");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TrucksOrders",
                table: "TrucksOrders",
                columns: new[] { "OrderID", "TruckID" });

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
        }
    }
}
