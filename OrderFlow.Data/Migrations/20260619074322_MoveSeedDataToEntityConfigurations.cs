using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace OrderFlow.Data.Migrations
{
    /// <inheritdoc />
    public partial class MoveSeedDataToEntityConfigurations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                """
                DELETE FROM AspNetUserRoles
                WHERE UserId IN (
                    SELECT Id FROM AspNetUsers
                    WHERE NormalizedUserName IN (N'ADMIN', N'SPEDITOR', N'DRIVER', N'USER'))
                   OR RoleId IN (
                    SELECT Id FROM AspNetRoles
                    WHERE NormalizedName IN (N'ADMIN', N'SPEDITOR', N'DRIVER', N'USER'));

                DELETE FROM AspNetUsers
                WHERE NormalizedUserName IN (N'ADMIN', N'SPEDITOR', N'DRIVER', N'USER');

                DELETE FROM AspNetRoles
                WHERE NormalizedName IN (N'ADMIN', N'SPEDITOR', N'DRIVER', N'USER');

                IF NOT EXISTS (SELECT 1 FROM AspNetRoles WHERE Id = 'b91c3da6-5bde-478d-bf2c-257b2fc9567c')
                    INSERT INTO AspNetRoles (Id, ConcurrencyStamp, Name, NormalizedName)
                    VALUES ('b91c3da6-5bde-478d-bf2c-257b2fc9567c', NULL, N'Admin', N'ADMIN');

                IF NOT EXISTS (SELECT 1 FROM AspNetRoles WHERE Id = '206f9e28-522f-4d34-8bfd-ec37b093a331')
                    INSERT INTO AspNetRoles (Id, ConcurrencyStamp, Name, NormalizedName)
                    VALUES ('206f9e28-522f-4d34-8bfd-ec37b093a331', NULL, N'Speditor', N'SPEDITOR');

                IF NOT EXISTS (SELECT 1 FROM AspNetRoles WHERE Id = 'd349122c-ed4a-43ca-b939-b3a9ff9fa0fb')
                    INSERT INTO AspNetRoles (Id, ConcurrencyStamp, Name, NormalizedName)
                    VALUES ('d349122c-ed4a-43ca-b939-b3a9ff9fa0fb', NULL, N'Driver', N'DRIVER');

                IF NOT EXISTS (SELECT 1 FROM AspNetRoles WHERE Id = '782b39c1-d0e7-4f18-8a2b-1077a438c048')
                    INSERT INTO AspNetRoles (Id, ConcurrencyStamp, Name, NormalizedName)
                    VALUES ('782b39c1-d0e7-4f18-8a2b-1077a438c048', NULL, N'User', N'USER');

                IF NOT EXISTS (SELECT 1 FROM AspNetUsers WHERE Id = '922b605c-25eb-4ded-b2a7-7966b38b8685')
                    INSERT INTO AspNetUsers
                        (Id, AccessFailedCount, AccountType, ConcurrencyStamp, Email, EmailConfirmed, LockoutEnabled,
                         LockoutEnd, NormalizedEmail, NormalizedUserName, PasswordHash, PhoneNumber,
                         PhoneNumberConfirmed, SecurityStamp, TwoFactorEnabled, UserName)
                    VALUES
                        ('922b605c-25eb-4ded-b2a7-7966b38b8685', 0, 1, N'admin-concurrency-stamp',
                         N'admin@gmail.com', 1, 1, NULL, N'ADMIN@GMAIL.COM', N'ADMIN',
                         N'AQAAAAIAAYagAAAAEE9yZGVyRmxvdy1BZG1pbjA5PifeCHRqVmoLRpJeI0xoYRKND7a+WeS4Sp75s4JzsA==',
                         N'1234567890', 1, N'admin-security-stamp', 0, N'Admin');

                IF NOT EXISTS (SELECT 1 FROM AspNetUsers WHERE Id = '051eafea-deaa-4fbe-b442-25b7be18fa8e')
                    INSERT INTO AspNetUsers
                        (Id, AccessFailedCount, AccountType, ConcurrencyStamp, Email, EmailConfirmed, LockoutEnabled,
                         LockoutEnd, NormalizedEmail, NormalizedUserName, PasswordHash, PhoneNumber,
                         PhoneNumberConfirmed, SecurityStamp, TwoFactorEnabled, UserName)
                    VALUES
                        ('051eafea-deaa-4fbe-b442-25b7be18fa8e', 0, 1, N'speditor-concurrency-stamp',
                         N'speditor@gmail.com', 1, 1, NULL, N'SPEDITOR@GMAIL.COM', N'SPEDITOR',
                         N'AQAAAAIAAYagAAAAEE9yZGVyRmxvdy1TcGVkaXSSvU91xg3cFsOcphJ7ZWFCRJLpoDgFkVc6JF0woYnPow==',
                         N'1234567890', 1, N'speditor-security-stamp', 0, N'Speditor');

                IF NOT EXISTS (SELECT 1 FROM AspNetUsers WHERE Id = 'cca021da-da51-4bd7-b968-8294cf006dfd')
                    INSERT INTO AspNetUsers
                        (Id, AccessFailedCount, AccountType, ConcurrencyStamp, Email, EmailConfirmed, LockoutEnabled,
                         LockoutEnd, NormalizedEmail, NormalizedUserName, PasswordHash, PhoneNumber,
                         PhoneNumberConfirmed, SecurityStamp, TwoFactorEnabled, UserName)
                    VALUES
                        ('cca021da-da51-4bd7-b968-8294cf006dfd', 0, 1, N'driver-concurrency-stamp',
                         N'driver@gmail.com', 1, 1, NULL, N'DRIVER@GMAIL.COM', N'DRIVER',
                         N'AQAAAAIAAYagAAAAEE9yZGVyRmxvdy1Ecml2ZXKMfnSIV9cEheURCTgDayjrZhIOatwnpr+eCOv9tIbz1Q==',
                         N'1234567890', 1, N'driver-security-stamp', 0, N'Driver');

                IF NOT EXISTS (SELECT 1 FROM AspNetUsers WHERE Id = '862c30cd-17ee-4557-8222-c926bebf0a66')
                    INSERT INTO AspNetUsers
                        (Id, AccessFailedCount, AccountType, ConcurrencyStamp, Email, EmailConfirmed, LockoutEnabled,
                         LockoutEnd, NormalizedEmail, NormalizedUserName, PasswordHash, PhoneNumber,
                         PhoneNumberConfirmed, SecurityStamp, TwoFactorEnabled, UserName)
                    VALUES
                        ('862c30cd-17ee-4557-8222-c926bebf0a66', 0, 1, N'user-concurrency-stamp',
                         N'user@gmail.com', 1, 1, NULL, N'USER@GMAIL.COM', N'USER',
                         N'AQAAAAIAAYagAAAAEE9yZGVyRmxvdy1Vc2VyMDAiHf5dIXjh1QwjRFIRIMb1wl2FH5K0ZPkGmRa0p3RBJA==',
                         N'1234567890', 1, N'user-security-stamp', 0, N'User');

                IF NOT EXISTS (SELECT 1 FROM AspNetUserRoles WHERE UserId = '922b605c-25eb-4ded-b2a7-7966b38b8685')
                    INSERT INTO AspNetUserRoles (UserId, RoleId)
                    VALUES ('922b605c-25eb-4ded-b2a7-7966b38b8685', 'b91c3da6-5bde-478d-bf2c-257b2fc9567c');

                IF NOT EXISTS (SELECT 1 FROM AspNetUserRoles WHERE UserId = '051eafea-deaa-4fbe-b442-25b7be18fa8e')
                    INSERT INTO AspNetUserRoles (UserId, RoleId)
                    VALUES ('051eafea-deaa-4fbe-b442-25b7be18fa8e', '206f9e28-522f-4d34-8bfd-ec37b093a331');

                IF NOT EXISTS (SELECT 1 FROM AspNetUserRoles WHERE UserId = 'cca021da-da51-4bd7-b968-8294cf006dfd')
                    INSERT INTO AspNetUserRoles (UserId, RoleId)
                    VALUES ('cca021da-da51-4bd7-b968-8294cf006dfd', 'd349122c-ed4a-43ca-b939-b3a9ff9fa0fb');

                IF NOT EXISTS (SELECT 1 FROM AspNetUserRoles WHERE UserId = '862c30cd-17ee-4557-8222-c926bebf0a66')
                    INSERT INTO AspNetUserRoles (UserId, RoleId)
                    VALUES ('862c30cd-17ee-4557-8222-c926bebf0a66', '782b39c1-d0e7-4f18-8a2b-1077a438c048');
                """);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("051eafea-deaa-4fbe-b442-25b7be18fa8e"),
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "speditor-concurrency-stamp", "AQAAAAIAAYagAAAAEE9yZGVyRmxvdy1TcGVkaXSSvU91xg3cFsOcphJ7ZWFCRJLpoDgFkVc6JF0woYnPow==", "speditor-security-stamp" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("862c30cd-17ee-4557-8222-c926bebf0a66"),
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "user-concurrency-stamp", "AQAAAAIAAYagAAAAEE9yZGVyRmxvdy1Vc2VyMDAiHf5dIXjh1QwjRFIRIMb1wl2FH5K0ZPkGmRa0p3RBJA==", "user-security-stamp" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("922b605c-25eb-4ded-b2a7-7966b38b8685"),
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "admin-concurrency-stamp", "AQAAAAIAAYagAAAAEE9yZGVyRmxvdy1BZG1pbjA5PifeCHRqVmoLRpJeI0xoYRKND7a+WeS4Sp75s4JzsA==", "admin-security-stamp" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("cca021da-da51-4bd7-b968-8294cf006dfd"),
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "driver-concurrency-stamp", "AQAAAAIAAYagAAAAEE9yZGVyRmxvdy1Ecml2ZXKMfnSIV9cEheURCTgDayjrZhIOatwnpr+eCOv9tIbz1Q==", "driver-security-stamp" });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "AccountType", "ConcurrencyStamp", "Email", "EmailConfirmed", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[,]
                {
                    { new Guid("583560aa-73c7-4c08-9e32-958581be98eb"), 0, 2, "company-user-concurrency-stamp", "company.user@gmail.com", true, true, null, "COMPANY.USER@GMAIL.COM", "COMPANYUSER", "AQAAAAIAAYagAAAAEE9yZGVyRmxvdy1Db21wYW6WF4dt7DOmRu3CVZWpcl9cjFmtsLRrJ7wv3OhfkgWqPg==", "0888123456", true, "company-user-security-stamp", false, "CompanyUser" },
                    { new Guid("c835750b-0e0e-4fb0-88b6-a4b557e52c21"), 0, 1, "driver-two-concurrency-stamp", "driver.two@gmail.com", true, true, null, "DRIVER.TWO@GMAIL.COM", "DRIVERTWO", "AQAAAAIAAYagAAAAEE9yZGVyRmxvdy1Ecml2ZXKmM70g+ZLq3lK0EY9ly3Kx+5ZB6Yx85Ajv+WSevCIv5g==", "0888654321", true, "driver-two-security-stamp", false, "DriverTwo" }
                });

            migrationBuilder.InsertData(
                table: "Orders",
                columns: new[] { "OrderID", "DeliveryAddress", "DeliveryDate", "DeliveryInstructions", "LoadCapacity", "OrderDate", "PickupAddress", "Status", "UserID" },
                values: new object[,]
                {
                    { new Guid("197325b6-f955-44d9-82a3-d7432a5d77d4"), "Plovdiv, 25 Hristo Botev Blvd.", null, "Seeded sample order 1.", 3.0, new DateTime(2026, 6, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), "Sofia, 100 Tsarigradsko Shose Blvd.", 1, new Guid("862c30cd-17ee-4557-8222-c926bebf0a66") },
                    { new Guid("24b95dc1-69d4-4ba9-b91e-d0b691ad729c"), "Varna, 55 Slivnitsa Blvd.", new DateTime(2026, 6, 13, 0, 0, 0, 0, DateTimeKind.Unspecified), "Seeded sample order 3.", 5.0, new DateTime(2026, 6, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), "Plovdiv, 12 Vasil Aprilov Blvd.", 2, new Guid("862c30cd-17ee-4557-8222-c926bebf0a66") },
                    { new Guid("2be2d17f-f954-4396-a169-0efe0f58c43d"), "Razgrad, 8 Aprilsko Vastanie Blvd.", null, "Seeded sample order 8.", 4.0, new DateTime(2026, 6, 9, 0, 0, 0, 0, DateTimeKind.Unspecified), "Burgas, 42 Todor Aleksandrov Blvd.", 1, new Guid("862c30cd-17ee-4557-8222-c926bebf0a66") },
                    { new Guid("32276d6e-9c7d-46a9-ad2f-3f41b72639a0"), "Pazardzhik, 10 Bulgaria Blvd.", null, "Seeded sample order 2.", 4.0, new DateTime(2026, 6, 3, 0, 0, 0, 0, DateTimeKind.Unspecified), "Sofia, 15 Botevgradsko Shose Blvd.", 1, new Guid("862c30cd-17ee-4557-8222-c926bebf0a66") },
                    { new Guid("3627fcd5-dfce-4967-9372-f084648cc02b"), "Yambol, 16 Graf Ignatiev St.", null, "Seeded sample order 6.", 5.0, new DateTime(2026, 6, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), "Varna, 18 Vladislav Varnenchik Blvd.", 1, new Guid("862c30cd-17ee-4557-8222-c926bebf0a66") },
                    { new Guid("64cce443-8d08-4ca8-8874-14b50097dcfb"), "Pleven, 29 Danail Popov St.", null, "Seeded sample order 9.", 5.0, new DateTime(2026, 6, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), "Ruse, 5 Danube Str.", 1, new Guid("862c30cd-17ee-4557-8222-c926bebf0a66") },
                    { new Guid("756e41df-edf9-417d-9586-52cd39fd6649"), "Dobrich, 3 25-ti Septemvri Blvd.", null, "Seeded sample order 4.", 3.0, new DateTime(2026, 6, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), "Plovdiv, 8 Kuklensko Shose Blvd.", 1, new Guid("862c30cd-17ee-4557-8222-c926bebf0a66") },
                    { new Guid("838aad0d-d377-46fc-8425-e00bae35680f"), "Lovech, 12 Bulgaria Blvd.", null, "Seeded sample order 10.", 3.0, new DateTime(2026, 6, 11, 0, 0, 0, 0, DateTimeKind.Unspecified), "Ruse, 17 Lipnik Blvd.", 1, new Guid("862c30cd-17ee-4557-8222-c926bebf0a66") },
                    { new Guid("946d7fb1-7b00-4aed-a55b-47f4776f3a87"), "Burgas, 21 Stefan Stambolov Blvd.", null, "Seeded sample order 5.", 4.0, new DateTime(2026, 6, 6, 0, 0, 0, 0, DateTimeKind.Unspecified), "Varna, 24 Devnya Industrial Zone", 1, new Guid("862c30cd-17ee-4557-8222-c926bebf0a66") },
                    { new Guid("9790ca1e-e41a-467b-a237-1d4041e07692"), "Ruse, 40 Bulgaria Blvd.", new DateTime(2026, 6, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), "Seeded sample order 7.", 3.0, new DateTime(2026, 6, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), "Burgas, 6 Transportna St.", 2, new Guid("862c30cd-17ee-4557-8222-c926bebf0a66") }
                });

            migrationBuilder.InsertData(
                table: "Trucks",
                columns: new[] { "TruckID", "Capacity", "DriverID", "LicensePlate", "Status" },
                values: new object[,]
                {
                    { new Guid("24095c26-af58-43fa-9a87-6443d4ad469d"), 21.0, new Guid("cca021da-da51-4bd7-b968-8294cf006dfd"), "CB2756HK", 2 },
                    { new Guid("75c3da6c-8cc5-4bbc-b365-80c24b0216fc"), 15.0, new Guid("cca021da-da51-4bd7-b968-8294cf006dfd"), "CB0534HK", 0 },
                    { new Guid("b6858daf-c51a-4a70-8246-6fc477d53111"), 18.0, new Guid("cca021da-da51-4bd7-b968-8294cf006dfd"), "CB1645HK", 0 }
                });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[,]
                {
                    { new Guid("782b39c1-d0e7-4f18-8a2b-1077a438c048"), new Guid("583560aa-73c7-4c08-9e32-958581be98eb") },
                    { new Guid("d349122c-ed4a-43ca-b939-b3a9ff9fa0fb"), new Guid("c835750b-0e0e-4fb0-88b6-a4b557e52c21") }
                });

            migrationBuilder.InsertData(
                table: "CompanyProfiles",
                columns: new[] { "UserId", "CompanyAdress", "CompanyName", "ContactPersonName", "ContactPhone", "VATNumber" },
                values: new object[] { new Guid("583560aa-73c7-4c08-9e32-958581be98eb"), "Sofia, 100 Tsarigradsko Shose Blvd.", "Demo Logistics Ltd.", "Elena Petrova", "0888123456", "BG123456789" });

            migrationBuilder.InsertData(
                table: "Notifications",
                columns: new[] { "NotificationID", "CanRespond", "CourseID", "CreatedAt", "IsRead", "Message", "OrderID", "PaymentID", "ReceiverID", "SenderID", "Title", "TruckID", "TruckSpendingID" },
                values: new object[] { new Guid("acfc3587-f1c7-476d-a8f8-26e535df971c"), true, null, new DateTime(2026, 6, 16, 8, 0, 0, 0, DateTimeKind.Unspecified), true, "Please confirm that the vehicle documents are available.", null, null, new Guid("cca021da-da51-4bd7-b968-8294cf006dfd"), new Guid("922b605c-25eb-4ded-b2a7-7966b38b8685"), "Vehicle document check", new Guid("75c3da6c-8cc5-4bbc-b365-80c24b0216fc"), null });

            migrationBuilder.InsertData(
                table: "Notifications",
                columns: new[] { "NotificationID", "CanRespond", "CourseID", "CreatedAt", "Message", "OrderID", "PaymentID", "ReceiverID", "SenderID", "Title", "TruckID", "TruckSpendingID" },
                values: new object[] { new Guid("d4948efb-fa00-4e9b-8664-e468bcbdb701"), true, null, new DateTime(2026, 6, 16, 11, 0, 0, 0, DateTimeKind.Unspecified), "Please confirm whether the proposed delivery window is suitable.", new Guid("32276d6e-9c7d-46a9-ad2f-3f41b72639a0"), null, new Guid("862c30cd-17ee-4557-8222-c926bebf0a66"), new Guid("051eafea-deaa-4fbe-b442-25b7be18fa8e"), "Delivery window", null, null });

            migrationBuilder.InsertData(
                table: "Notifications",
                columns: new[] { "NotificationID", "CanRespond", "CourseID", "CreatedAt", "IsRead", "Message", "OrderID", "PaymentID", "ReceiverID", "SenderID", "Title", "TruckID", "TruckSpendingID" },
                values: new object[] { new Guid("fea90b4a-ec4c-4fc8-b5d2-07342c709fad"), true, null, new DateTime(2026, 6, 16, 6, 0, 0, 0, DateTimeKind.Unspecified), true, "Please confirm the delivery contact details.", new Guid("197325b6-f955-44d9-82a3-d7432a5d77d4"), null, new Guid("862c30cd-17ee-4557-8222-c926bebf0a66"), new Guid("922b605c-25eb-4ded-b2a7-7966b38b8685"), "Delivery details required", null, null });

            migrationBuilder.InsertData(
                table: "Orders",
                columns: new[] { "OrderID", "DeliveryAddress", "DeliveryDate", "DeliveryInstructions", "LoadCapacity", "OrderDate", "PickupAddress", "Status", "UserID" },
                values: new object[,]
                {
                    { new Guid("12a1bf6e-1406-47f3-8ed0-7b2aedbdf487"), "Montana, 13 Treti Mart Blvd.", null, "Seeded sample order 8.", 5.0, new DateTime(2026, 6, 19, 0, 0, 0, 0, DateTimeKind.Unspecified), "Shumen, 7 Simeon Veliki Blvd.", 0, new Guid("583560aa-73c7-4c08-9e32-958581be98eb") },
                    { new Guid("3f5b28dc-e9a6-4838-84c5-8c4a61e09266"), "Shumen, 45 Rishki Prohod Blvd.", new DateTime(2026, 6, 19, 0, 0, 0, 0, DateTimeKind.Unspecified), "Seeded sample order 5.", 5.0, new DateTime(2026, 6, 16, 0, 0, 0, 0, DateTimeKind.Unspecified), "Veliko Tarnovo, 11 Magistralna St.", 2, new Guid("583560aa-73c7-4c08-9e32-958581be98eb") },
                    { new Guid("45de5b74-7437-4bef-a4dd-239a861b4d91"), "Targovishte, 9 Mimi Balkanska St.", null, "Seeded sample order 6.", 3.0, new DateTime(2026, 6, 17, 0, 0, 0, 0, DateTimeKind.Unspecified), "Veliko Tarnovo, 44 Nikola Gabrovski Blvd.", 1, new Guid("583560aa-73c7-4c08-9e32-958581be98eb") },
                    { new Guid("58c318c5-696c-4008-9f0e-e362d8b04403"), "Pernik, 22 Yug Gagarin St.", null, "Seeded sample order 4.", 4.0, new DateTime(2026, 6, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), "Pleven, 27 Ruse Blvd.", 1, new Guid("583560aa-73c7-4c08-9e32-958581be98eb") },
                    { new Guid("74268bf2-b71b-4794-afd3-928ab2afc6fe"), "Haskovo, 26 Saedinenie Blvd.", null, "Seeded sample order 9.", 3.0, new DateTime(2026, 6, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), "Dobrich, 19 Dobruja Blvd.", 0, new Guid("583560aa-73c7-4c08-9e32-958581be98eb") },
                    { new Guid("881b936f-5d52-46f1-9df1-0d6a83e6f412"), "Veliko Tarnovo, 30 Bulgaria Blvd.", new DateTime(2026, 6, 17, 0, 0, 0, 0, DateTimeKind.Unspecified), "Seeded sample order 1.", 4.0, new DateTime(2026, 6, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), "Stara Zagora, 31 Patriarch Evtimiy Blvd.", 2, new Guid("583560aa-73c7-4c08-9e32-958581be98eb") },
                    { new Guid("9b2f9565-ca15-438a-99c6-6ab02173b96e"), "Gabrovo, 6 Aprilov Blvd.", null, "Seeded sample order 2.", 5.0, new DateTime(2026, 6, 13, 0, 0, 0, 0, DateTimeKind.Unspecified), "Stara Zagora, 9 Industrialna St.", 1, new Guid("583560aa-73c7-4c08-9e32-958581be98eb") },
                    { new Guid("a6442421-97fc-4637-b743-6599840ecdf5"), "Sofia, 88 Bulgaria Blvd.", null, "Seeded sample order 7.", 4.0, new DateTime(2026, 6, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), "Blagoevgrad, 20 Sveti Dimitar Solunski Blvd.", 0, new Guid("583560aa-73c7-4c08-9e32-958581be98eb") },
                    { new Guid("cf0bb218-93c0-49f4-b0b0-6fbcf31387ce"), "Blagoevgrad, 14 Vasil Levski Blvd.", null, "Seeded sample order 3.", 3.0, new DateTime(2026, 6, 14, 0, 0, 0, 0, DateTimeKind.Unspecified), "Pleven, 14 Bulgaria Blvd.", 1, new Guid("583560aa-73c7-4c08-9e32-958581be98eb") },
                    { new Guid("ee541fb1-c5a1-464e-b33e-5d639454c4b1"), "Kardzhali, 5 Bulgaria Blvd.", null, "Seeded sample order 10.", 4.0, new DateTime(2026, 6, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), "Sliven, 33 Bratya Miladinovi St.", 0, new Guid("583560aa-73c7-4c08-9e32-958581be98eb") }
                });

            migrationBuilder.InsertData(
                table: "Payments",
                columns: new[] { "PaymentID", "Amount", "CreatedOn", "OrderID", "PaymentDate", "PaymentDescription", "PaymentMethod" },
                values: new object[,]
                {
                    { new Guid("00348107-1dbe-40fe-bf57-6c3d5d244300"), 25m, new DateTime(2026, 6, 2, 0, 1, 0, 0, DateTimeKind.Unspecified), new Guid("197325b6-f955-44d9-82a3-d7432a5d77d4"), null, "Parking fee", null },
                    { new Guid("0967ba5c-d0a8-4dc9-8117-ef33030c560d"), 35m, new DateTime(2026, 6, 10, 0, 1, 0, 0, DateTimeKind.Unspecified), new Guid("64cce443-8d08-4ca8-8874-14b50097dcfb"), null, "Parking fee", null },
                    { new Guid("0f566482-5add-4d44-8bea-e0952509a381"), 25m, new DateTime(2026, 6, 8, 0, 1, 0, 0, DateTimeKind.Unspecified), new Guid("9790ca1e-e41a-467b-a237-1d4041e07692"), new DateTime(2026, 6, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), "Parking fee", 0 },
                    { new Guid("1e520388-1eed-456d-b2fa-5a85212e9c27"), 525m, new DateTime(2026, 6, 3, 0, 0, 0, 0, DateTimeKind.Unspecified), new Guid("32276d6e-9c7d-46a9-ad2f-3f41b72639a0"), null, "Transport fee", null },
                    { new Guid("31982e7a-68c6-4bea-95e9-421d3c684401"), 25m, new DateTime(2026, 6, 5, 0, 1, 0, 0, DateTimeKind.Unspecified), new Guid("756e41df-edf9-417d-9586-52cd39fd6649"), null, "Parking fee", null },
                    { new Guid("3644313f-feb7-4f6e-8c2f-281d3fb44d75"), -50m, new DateTime(2026, 6, 10, 0, 3, 0, 0, DateTimeKind.Unspecified), new Guid("64cce443-8d08-4ca8-8874-14b50097dcfb"), null, "Customer discount", null },
                    { new Guid("3f748138-84c9-4c59-b2e6-74c104e30883"), 30m, new DateTime(2026, 6, 3, 0, 1, 0, 0, DateTimeKind.Unspecified), new Guid("32276d6e-9c7d-46a9-ad2f-3f41b72639a0"), null, "Parking fee", null },
                    { new Guid("4b91ce9b-b133-4910-b328-d8209b87aad5"), 50m, new DateTime(2026, 6, 3, 0, 2, 0, 0, DateTimeKind.Unspecified), new Guid("32276d6e-9c7d-46a9-ad2f-3f41b72639a0"), null, "Toll fee", null },
                    { new Guid("50647686-134d-45e8-90c8-9ad5dbdc6038"), 70m, new DateTime(2026, 6, 9, 0, 2, 0, 0, DateTimeKind.Unspecified), new Guid("2be2d17f-f954-4396-a169-0efe0f58c43d"), null, "Toll fee", null },
                    { new Guid("525be9f3-02a9-47cb-86ba-f4c441e99d17"), -60m, new DateTime(2026, 6, 9, 0, 3, 0, 0, DateTimeKind.Unspecified), new Guid("2be2d17f-f954-4396-a169-0efe0f58c43d"), null, "Customer discount", null },
                    { new Guid("59378b0a-b9a5-43be-b0d1-7012731202d5"), 50m, new DateTime(2026, 6, 11, 0, 2, 0, 0, DateTimeKind.Unspecified), new Guid("838aad0d-d377-46fc-8425-e00bae35680f"), null, "Toll fee", null },
                    { new Guid("5e7818a1-9284-487f-995f-05a5e47c32c6"), 60m, new DateTime(2026, 6, 8, 0, 2, 0, 0, DateTimeKind.Unspecified), new Guid("9790ca1e-e41a-467b-a237-1d4041e07692"), new DateTime(2026, 6, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), "Toll fee", 0 },
                    { new Guid("64308068-9c94-47e6-a9b3-d98d91898d13"), 625m, new DateTime(2026, 6, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new Guid("3627fcd5-dfce-4967-9372-f084648cc02b"), null, "Transport fee", null },
                    { new Guid("64db478c-86db-47fd-ae54-8a551f9e238a"), 500m, new DateTime(2026, 6, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), new Guid("197325b6-f955-44d9-82a3-d7432a5d77d4"), null, "Transport fee", null },
                    { new Guid("663b3320-c0cf-47cc-b642-564a43271f6c"), -50m, new DateTime(2026, 6, 2, 0, 3, 0, 0, DateTimeKind.Unspecified), new Guid("197325b6-f955-44d9-82a3-d7432a5d77d4"), null, "Customer discount", null },
                    { new Guid("6be17a37-220c-46b9-8e82-fd268ef54049"), 40m, new DateTime(2026, 6, 6, 0, 2, 0, 0, DateTimeKind.Unspecified), new Guid("946d7fb1-7b00-4aed-a55b-47f4776f3a87"), null, "Toll fee", null },
                    { new Guid("74a07ab0-6c8f-48f3-93f7-ec98219c8090"), -50m, new DateTime(2026, 6, 4, 0, 3, 0, 0, DateTimeKind.Unspecified), new Guid("24b95dc1-69d4-4ba9-b91e-d0b691ad729c"), new DateTime(2026, 6, 6, 0, 0, 0, 0, DateTimeKind.Unspecified), "Customer discount", 0 },
                    { new Guid("7517c585-dc47-4c0b-9bb9-b9afee7cbbac"), 675m, new DateTime(2026, 6, 9, 0, 0, 0, 0, DateTimeKind.Unspecified), new Guid("2be2d17f-f954-4396-a169-0efe0f58c43d"), null, "Transport fee", null },
                    { new Guid("77543160-c90a-468f-b01a-29072dc45acd"), 30m, new DateTime(2026, 6, 9, 0, 1, 0, 0, DateTimeKind.Unspecified), new Guid("2be2d17f-f954-4396-a169-0efe0f58c43d"), null, "Parking fee", null },
                    { new Guid("824546f6-9d10-45eb-b902-6e07d43f5cb8"), 550m, new DateTime(2026, 6, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new Guid("24b95dc1-69d4-4ba9-b91e-d0b691ad729c"), new DateTime(2026, 6, 6, 0, 0, 0, 0, DateTimeKind.Unspecified), "Transport fee", 0 },
                    { new Guid("87ce77c6-50d3-4d42-ab35-db03001a64f7"), -60m, new DateTime(2026, 6, 7, 0, 3, 0, 0, DateTimeKind.Unspecified), new Guid("3627fcd5-dfce-4967-9372-f084648cc02b"), null, "Customer discount", null },
                    { new Guid("8be3b081-5e59-4e6d-b718-ccac43b0f4e1"), -60m, new DateTime(2026, 6, 5, 0, 3, 0, 0, DateTimeKind.Unspecified), new Guid("756e41df-edf9-417d-9586-52cd39fd6649"), null, "Customer discount", null },
                    { new Guid("8ef41387-4470-435c-8c15-86b422347b52"), 40m, new DateTime(2026, 6, 2, 0, 2, 0, 0, DateTimeKind.Unspecified), new Guid("197325b6-f955-44d9-82a3-d7432a5d77d4"), null, "Toll fee", null },
                    { new Guid("91b2516a-16b1-47cf-87da-cd708af569d2"), 650m, new DateTime(2026, 6, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), new Guid("9790ca1e-e41a-467b-a237-1d4041e07692"), new DateTime(2026, 6, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), "Transport fee", 0 },
                    { new Guid("91c5ab1b-ff4f-404a-bcb8-29ce474c50b9"), 40m, new DateTime(2026, 6, 10, 0, 2, 0, 0, DateTimeKind.Unspecified), new Guid("64cce443-8d08-4ca8-8874-14b50097dcfb"), null, "Toll fee", null },
                    { new Guid("995da2c1-a429-48b0-9bb6-41742a4f95d2"), 35m, new DateTime(2026, 6, 7, 0, 1, 0, 0, DateTimeKind.Unspecified), new Guid("3627fcd5-dfce-4967-9372-f084648cc02b"), null, "Parking fee", null },
                    { new Guid("a2649491-7455-4e9d-a158-61de1c7c0a5b"), 25m, new DateTime(2026, 6, 11, 0, 1, 0, 0, DateTimeKind.Unspecified), new Guid("838aad0d-d377-46fc-8425-e00bae35680f"), null, "Parking fee", null },
                    { new Guid("a93caa41-1c70-464f-be4f-3f9d836d6a2b"), 30m, new DateTime(2026, 6, 6, 0, 1, 0, 0, DateTimeKind.Unspecified), new Guid("946d7fb1-7b00-4aed-a55b-47f4776f3a87"), null, "Parking fee", null },
                    { new Guid("b2c1e70b-c270-48e3-b82b-0025ba0aee51"), -60m, new DateTime(2026, 6, 11, 0, 3, 0, 0, DateTimeKind.Unspecified), new Guid("838aad0d-d377-46fc-8425-e00bae35680f"), null, "Customer discount", null },
                    { new Guid("b4437aae-2b71-4442-8f3b-57b3747b45f1"), -60m, new DateTime(2026, 6, 3, 0, 3, 0, 0, DateTimeKind.Unspecified), new Guid("32276d6e-9c7d-46a9-ad2f-3f41b72639a0"), null, "Customer discount", null },
                    { new Guid("b93fc098-2bad-475c-84b2-2957c0945a25"), 50m, new DateTime(2026, 6, 7, 0, 2, 0, 0, DateTimeKind.Unspecified), new Guid("3627fcd5-dfce-4967-9372-f084648cc02b"), null, "Toll fee", null },
                    { new Guid("c24d76aa-d290-4f1e-aa29-a5a0328e21d3"), 700m, new DateTime(2026, 6, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), new Guid("64cce443-8d08-4ca8-8874-14b50097dcfb"), null, "Transport fee", null },
                    { new Guid("d4ddf38e-0676-4129-bc44-474408cbe273"), -50m, new DateTime(2026, 6, 8, 0, 3, 0, 0, DateTimeKind.Unspecified), new Guid("9790ca1e-e41a-467b-a237-1d4041e07692"), new DateTime(2026, 6, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), "Customer discount", 0 },
                    { new Guid("d7fac92d-9985-48c3-83ae-55550f10a40a"), 600m, new DateTime(2026, 6, 6, 0, 0, 0, 0, DateTimeKind.Unspecified), new Guid("946d7fb1-7b00-4aed-a55b-47f4776f3a87"), null, "Transport fee", null },
                    { new Guid("e37aaca1-e9f3-4116-a6fd-9f292a334e12"), 575m, new DateTime(2026, 6, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), new Guid("756e41df-edf9-417d-9586-52cd39fd6649"), null, "Transport fee", null },
                    { new Guid("e916f797-a08f-4b41-befd-d45b2129ff61"), 70m, new DateTime(2026, 6, 5, 0, 2, 0, 0, DateTimeKind.Unspecified), new Guid("756e41df-edf9-417d-9586-52cd39fd6649"), null, "Toll fee", null },
                    { new Guid("eb81ff22-ad1d-4449-bdb8-82c9c9d3ed9d"), 35m, new DateTime(2026, 6, 4, 0, 1, 0, 0, DateTimeKind.Unspecified), new Guid("24b95dc1-69d4-4ba9-b91e-d0b691ad729c"), new DateTime(2026, 6, 6, 0, 0, 0, 0, DateTimeKind.Unspecified), "Parking fee", 0 },
                    { new Guid("f5fe8642-b20e-4e19-9e7f-5032cd376313"), 60m, new DateTime(2026, 6, 4, 0, 2, 0, 0, DateTimeKind.Unspecified), new Guid("24b95dc1-69d4-4ba9-b91e-d0b691ad729c"), new DateTime(2026, 6, 6, 0, 0, 0, 0, DateTimeKind.Unspecified), "Toll fee", 0 },
                    { new Guid("f9a1dd71-d6a0-4ee7-8d6e-7451a548ff68"), -50m, new DateTime(2026, 6, 6, 0, 3, 0, 0, DateTimeKind.Unspecified), new Guid("946d7fb1-7b00-4aed-a55b-47f4776f3a87"), null, "Customer discount", null },
                    { new Guid("fe79695e-8d83-4e79-98d1-b952fcccf69e"), 725m, new DateTime(2026, 6, 11, 0, 0, 0, 0, DateTimeKind.Unspecified), new Guid("838aad0d-d377-46fc-8425-e00bae35680f"), null, "Transport fee", null }
                });

            migrationBuilder.InsertData(
                table: "PersonalProfiles",
                columns: new[] { "UserId", "Adress", "DateOfBirth", "FirstName", "LastName", "PersonalNumber" },
                values: new object[] { new Guid("c835750b-0e0e-4fb0-88b6-a4b557e52c21"), "Ruse, 5 Danube Str.", new DateTime(1988, 5, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), "Nikolay", "Dimitrov", "8805050005" });

            migrationBuilder.InsertData(
                table: "Trucks",
                columns: new[] { "TruckID", "Capacity", "DriverID", "LicensePlate", "Status" },
                values: new object[,]
                {
                    { new Guid("0cada001-5206-4aff-a0e5-4600bcfed88f"), 21.0, new Guid("c835750b-0e0e-4fb0-88b6-a4b557e52c21"), "CB2756HK", 2 },
                    { new Guid("d578effb-04cd-4c11-817d-a9ac580ea8be"), 15.0, new Guid("c835750b-0e0e-4fb0-88b6-a4b557e52c21"), "CB0534HK", 0 },
                    { new Guid("e83b600b-b9a9-4f36-a4ce-3f4f002a2d1c"), 18.0, new Guid("c835750b-0e0e-4fb0-88b6-a4b557e52c21"), "CB1645HK", 0 }
                });

            migrationBuilder.InsertData(
                table: "TrucksCourses",
                columns: new[] { "TruckCourseID", "AssignedDate", "DeliverAddress", "DeliveryDate", "Income", "PickupAddress", "Status", "TruckID" },
                values: new object[,]
                {
                    { new Guid("0399dfb7-4de1-4ab5-8db2-d45cb6f0932f"), new DateTime(2026, 6, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), "Pleven, 29 Danail Popov St.", null, 1000m, "Ruse, 5 Danube Str.", 1, new Guid("b6858daf-c51a-4a70-8246-6fc477d53111") },
                    { new Guid("1c9204d6-e8cb-4929-a926-d55206215421"), new DateTime(2026, 6, 16, 0, 0, 0, 0, DateTimeKind.Unspecified), "Veliko Tarnovo, 30 Bulgaria Blvd.", new DateTime(2026, 6, 17, 0, 0, 0, 0, DateTimeKind.Unspecified), 1150m, "Stara Zagora, 31 Patriarch Evtimiy Blvd.", 3, new Guid("b6858daf-c51a-4a70-8246-6fc477d53111") },
                    { new Guid("29b586bf-5f04-4882-9cdf-5946ea3ecec3"), new DateTime(2026, 6, 14, 0, 0, 0, 0, DateTimeKind.Unspecified), "Ruse, 40 Bulgaria Blvd.", new DateTime(2026, 6, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), 850m, "Burgas, 6 Transportna St.", 3, new Guid("b6858daf-c51a-4a70-8246-6fc477d53111") },
                    { new Guid("2f671305-cd8f-40fa-a9c3-4d77525b6316"), new DateTime(2026, 6, 13, 0, 0, 0, 0, DateTimeKind.Unspecified), "Burgas, 21 Stefan Stambolov Blvd.", null, 1150m, "Varna, 24 Devnya Industrial Zone", 1, new Guid("75c3da6c-8cc5-4bbc-b365-80c24b0216fc") },
                    { new Guid("5d8e5934-4800-4aca-b678-b859fcbf2c1b"), new DateTime(2026, 6, 19, 0, 0, 0, 0, DateTimeKind.Unspecified), "Pleven, 29 Danail Popov St.", null, 1150m, "Ruse, 5 Danube Str.", 1, new Guid("24095c26-af58-43fa-9a87-6443d4ad469d") },
                    { new Guid("6a8d56fc-d2b4-4aa0-9ea3-e460b72c9317"), new DateTime(2026, 6, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), "Varna, 55 Slivnitsa Blvd.", new DateTime(2026, 6, 13, 0, 0, 0, 0, DateTimeKind.Unspecified), 1000m, "Plovdiv, 12 Vasil Aprilov Blvd.", 3, new Guid("75c3da6c-8cc5-4bbc-b365-80c24b0216fc") },
                    { new Guid("92077f74-61b3-4893-86f9-8f7026c952a0"), new DateTime(2026, 6, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), "Shumen, 45 Rishki Prohod Blvd.", new DateTime(2026, 6, 19, 0, 0, 0, 0, DateTimeKind.Unspecified), 1000m, "Veliko Tarnovo, 11 Magistralna St.", 3, new Guid("24095c26-af58-43fa-9a87-6443d4ad469d") },
                    { new Guid("dbd66272-bee6-4114-a386-33090c0ba172"), new DateTime(2026, 6, 11, 0, 0, 0, 0, DateTimeKind.Unspecified), "Plovdiv, 25 Hristo Botev Blvd.", null, 850m, "Sofia, 100 Tsarigradsko Shose Blvd.", 1, new Guid("75c3da6c-8cc5-4bbc-b365-80c24b0216fc") },
                    { new Guid("ff91895b-3124-4f22-9198-989aaa34a799"), new DateTime(2026, 6, 17, 0, 0, 0, 0, DateTimeKind.Unspecified), "Blagoevgrad, 14 Vasil Levski Blvd.", null, 850m, "Pleven, 14 Bulgaria Blvd.", 1, new Guid("24095c26-af58-43fa-9a87-6443d4ad469d") }
                });

            migrationBuilder.InsertData(
                table: "CoursesOrders",
                columns: new[] { "OrderID", "TruckCourseID" },
                values: new object[,]
                {
                    { new Guid("197325b6-f955-44d9-82a3-d7432a5d77d4"), new Guid("dbd66272-bee6-4114-a386-33090c0ba172") },
                    { new Guid("24b95dc1-69d4-4ba9-b91e-d0b691ad729c"), new Guid("6a8d56fc-d2b4-4aa0-9ea3-e460b72c9317") },
                    { new Guid("2be2d17f-f954-4396-a169-0efe0f58c43d"), new Guid("29b586bf-5f04-4882-9cdf-5946ea3ecec3") },
                    { new Guid("32276d6e-9c7d-46a9-ad2f-3f41b72639a0"), new Guid("dbd66272-bee6-4114-a386-33090c0ba172") },
                    { new Guid("3627fcd5-dfce-4967-9372-f084648cc02b"), new Guid("2f671305-cd8f-40fa-a9c3-4d77525b6316") },
                    { new Guid("3f5b28dc-e9a6-4838-84c5-8c4a61e09266"), new Guid("92077f74-61b3-4893-86f9-8f7026c952a0") },
                    { new Guid("45de5b74-7437-4bef-a4dd-239a861b4d91"), new Guid("92077f74-61b3-4893-86f9-8f7026c952a0") },
                    { new Guid("58c318c5-696c-4008-9f0e-e362d8b04403"), new Guid("ff91895b-3124-4f22-9198-989aaa34a799") },
                    { new Guid("64cce443-8d08-4ca8-8874-14b50097dcfb"), new Guid("0399dfb7-4de1-4ab5-8db2-d45cb6f0932f") },
                    { new Guid("756e41df-edf9-417d-9586-52cd39fd6649"), new Guid("6a8d56fc-d2b4-4aa0-9ea3-e460b72c9317") },
                    { new Guid("838aad0d-d377-46fc-8425-e00bae35680f"), new Guid("0399dfb7-4de1-4ab5-8db2-d45cb6f0932f") },
                    { new Guid("881b936f-5d52-46f1-9df1-0d6a83e6f412"), new Guid("1c9204d6-e8cb-4929-a926-d55206215421") },
                    { new Guid("946d7fb1-7b00-4aed-a55b-47f4776f3a87"), new Guid("2f671305-cd8f-40fa-a9c3-4d77525b6316") },
                    { new Guid("9790ca1e-e41a-467b-a237-1d4041e07692"), new Guid("29b586bf-5f04-4882-9cdf-5946ea3ecec3") },
                    { new Guid("9b2f9565-ca15-438a-99c6-6ab02173b96e"), new Guid("1c9204d6-e8cb-4929-a926-d55206215421") },
                    { new Guid("cf0bb218-93c0-49f4-b0b0-6fbcf31387ce"), new Guid("ff91895b-3124-4f22-9198-989aaa34a799") }
                });

            migrationBuilder.InsertData(
                table: "Messages",
                columns: new[] { "MessageID", "Content", "IsRead", "NotificationID", "ReceiverID", "SenderID", "SentAt" },
                values: new object[,]
                {
                    { new Guid("33131dcd-474e-4c3f-bf36-0bfb4dc30965"), "Please send the contact name and phone number.", true, new Guid("fea90b4a-ec4c-4fc8-b5d2-07342c709fad"), new Guid("862c30cd-17ee-4557-8222-c926bebf0a66"), new Guid("922b605c-25eb-4ded-b2a7-7966b38b8685"), new DateTime(2026, 6, 17, 6, 0, 0, 0, DateTimeKind.Unspecified) },
                    { new Guid("86eb832d-3427-40d3-b346-ed51b84860a3"), "Are the insurance and technical inspection documents in the truck?", true, new Guid("acfc3587-f1c7-476d-a8f8-26e535df971c"), new Guid("cca021da-da51-4bd7-b968-8294cf006dfd"), new Guid("922b605c-25eb-4ded-b2a7-7966b38b8685"), new DateTime(2026, 6, 17, 8, 0, 0, 0, DateTimeKind.Unspecified) }
                });

            migrationBuilder.InsertData(
                table: "Messages",
                columns: new[] { "MessageID", "Content", "NotificationID", "ReceiverID", "SenderID", "SentAt" },
                values: new object[,]
                {
                    { new Guid("883093da-81c0-4f16-aceb-2414d570f128"), "The contact person is Ivan Petrov, phone 0888000001.", new Guid("fea90b4a-ec4c-4fc8-b5d2-07342c709fad"), new Guid("922b605c-25eb-4ded-b2a7-7966b38b8685"), new Guid("862c30cd-17ee-4557-8222-c926bebf0a66"), new DateTime(2026, 6, 17, 6, 1, 0, 0, DateTimeKind.Unspecified) },
                    { new Guid("e62daba9-4779-4012-a6eb-700ab315b294"), "Yes, both documents are available.", new Guid("acfc3587-f1c7-476d-a8f8-26e535df971c"), new Guid("922b605c-25eb-4ded-b2a7-7966b38b8685"), new Guid("cca021da-da51-4bd7-b968-8294cf006dfd"), new DateTime(2026, 6, 17, 8, 1, 0, 0, DateTimeKind.Unspecified) }
                });

            migrationBuilder.InsertData(
                table: "Notifications",
                columns: new[] { "NotificationID", "CourseID", "CreatedAt", "Message", "OrderID", "PaymentID", "ReceiverID", "SenderID", "Title", "TruckID", "TruckSpendingID" },
                values: new object[,]
                {
                    { new Guid("277ac80f-bd18-4a13-89f5-3c4dec699124"), null, new DateTime(2026, 6, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), "Your order has been received and is waiting to be assigned.", new Guid("a6442421-97fc-4637-b743-6599840ecdf5"), null, new Guid("862c30cd-17ee-4557-8222-c926bebf0a66"), null, "Order received", null, null },
                    { new Guid("27bd020a-fd3d-47f5-a635-de61f6fee0bc"), null, new DateTime(2026, 6, 15, 4, 0, 0, 0, DateTimeKind.Unspecified), "The payment for your completed order has been confirmed.", new Guid("24b95dc1-69d4-4ba9-b91e-d0b691ad729c"), new Guid("824546f6-9d10-45eb-b902-6e07d43f5cb8"), new Guid("862c30cd-17ee-4557-8222-c926bebf0a66"), null, "Payment confirmed", null, null }
                });

            migrationBuilder.InsertData(
                table: "Notifications",
                columns: new[] { "NotificationID", "CourseID", "CreatedAt", "IsRead", "Message", "OrderID", "PaymentID", "ReceiverID", "SenderID", "Title", "TruckID", "TruckSpendingID" },
                values: new object[] { new Guid("68f34703-4ace-4490-91db-770f19eda186"), new Guid("29b586bf-5f04-4882-9cdf-5946ea3ecec3"), new DateTime(2026, 6, 15, 3, 0, 0, 0, DateTimeKind.Unspecified), true, "The assigned course has been completed.", null, null, new Guid("c835750b-0e0e-4fb0-88b6-a4b557e52c21"), null, "Course completed", new Guid("d578effb-04cd-4c11-817d-a9ac580ea8be"), null });

            migrationBuilder.InsertData(
                table: "Notifications",
                columns: new[] { "NotificationID", "CourseID", "CreatedAt", "Message", "OrderID", "PaymentID", "ReceiverID", "SenderID", "Title", "TruckID", "TruckSpendingID" },
                values: new object[] { new Guid("6fb7a9ee-7b69-479b-b761-7982186a2db0"), new Guid("dbd66272-bee6-4114-a386-33090c0ba172"), new DateTime(2026, 6, 15, 2, 0, 0, 0, DateTimeKind.Unspecified), "A new course has been assigned to your truck.", null, null, new Guid("cca021da-da51-4bd7-b968-8294cf006dfd"), null, "Course assigned", new Guid("75c3da6c-8cc5-4bbc-b365-80c24b0216fc"), null });

            migrationBuilder.InsertData(
                table: "Notifications",
                columns: new[] { "NotificationID", "CanRespond", "CourseID", "CreatedAt", "Message", "OrderID", "PaymentID", "ReceiverID", "SenderID", "Title", "TruckID", "TruckSpendingID" },
                values: new object[] { new Guid("79fec001-e9c9-4806-b96f-25daeb5de2ff"), true, new Guid("29b586bf-5f04-4882-9cdf-5946ea3ecec3"), new DateTime(2026, 6, 16, 9, 0, 0, 0, DateTimeKind.Unspecified), "Additional route information is available for your course.", null, null, new Guid("c835750b-0e0e-4fb0-88b6-a4b557e52c21"), new Guid("051eafea-deaa-4fbe-b442-25b7be18fa8e"), "Route information", new Guid("d578effb-04cd-4c11-817d-a9ac580ea8be"), null });

            migrationBuilder.InsertData(
                table: "Notifications",
                columns: new[] { "NotificationID", "CourseID", "CreatedAt", "IsRead", "Message", "OrderID", "PaymentID", "ReceiverID", "SenderID", "Title", "TruckID", "TruckSpendingID" },
                values: new object[,]
                {
                    { new Guid("825924f2-3f9d-4bf9-879a-89ca9386c2a1"), null, new DateTime(2026, 6, 16, 10, 0, 0, 0, DateTimeKind.Unspecified), true, "The invoice for your completed order is now available.", new Guid("3f5b28dc-e9a6-4838-84c5-8c4a61e09266"), null, new Guid("583560aa-73c7-4c08-9e32-958581be98eb"), new Guid("922b605c-25eb-4ded-b2a7-7966b38b8685"), "Invoice available", null, null },
                    { new Guid("a194bc33-df7b-4a12-b53e-541833d8c20c"), null, new DateTime(2026, 6, 15, 1, 0, 0, 0, DateTimeKind.Unspecified), true, "Your order has been delivered successfully.", new Guid("3f5b28dc-e9a6-4838-84c5-8c4a61e09266"), null, new Guid("583560aa-73c7-4c08-9e32-958581be98eb"), null, "Order completed", null, null }
                });

            migrationBuilder.InsertData(
                table: "Notifications",
                columns: new[] { "NotificationID", "CourseID", "CreatedAt", "Message", "OrderID", "PaymentID", "ReceiverID", "SenderID", "Title", "TruckID", "TruckSpendingID" },
                values: new object[] { new Guid("c2c5dc86-8205-44ca-b9e0-5c35b27dbfda"), null, new DateTime(2026, 6, 16, 7, 0, 0, 0, DateTimeKind.Unspecified), "The pickup time for your order has been updated.", new Guid("881b936f-5d52-46f1-9df1-0d6a83e6f412"), null, new Guid("583560aa-73c7-4c08-9e32-958581be98eb"), new Guid("051eafea-deaa-4fbe-b442-25b7be18fa8e"), "Pickup time updated", null, null });

            migrationBuilder.InsertData(
                table: "Payments",
                columns: new[] { "PaymentID", "Amount", "CreatedOn", "OrderID", "PaymentDate", "PaymentDescription", "PaymentMethod" },
                values: new object[,]
                {
                    { new Guid("0129248c-b047-44b2-9a45-835cc752fa02"), -50m, new DateTime(2026, 6, 16, 0, 3, 0, 0, DateTimeKind.Unspecified), new Guid("3f5b28dc-e9a6-4838-84c5-8c4a61e09266"), new DateTime(2026, 6, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), "Customer discount", 0 },
                    { new Guid("10899709-c9c2-4396-9606-06b4d060a2c2"), 60m, new DateTime(2026, 6, 12, 0, 2, 0, 0, DateTimeKind.Unspecified), new Guid("881b936f-5d52-46f1-9df1-0d6a83e6f412"), new DateTime(2026, 6, 14, 0, 0, 0, 0, DateTimeKind.Unspecified), "Toll fee", 0 },
                    { new Guid("14748ef4-f20f-440d-a9f6-6a26021fc601"), -60m, new DateTime(2026, 6, 15, 0, 3, 0, 0, DateTimeKind.Unspecified), new Guid("58c318c5-696c-4008-9f0e-e362d8b04403"), null, "Customer discount", null },
                    { new Guid("1ba7af08-41ea-4d9a-8511-400b48a6a8ba"), -60m, new DateTime(2026, 6, 21, 0, 3, 0, 0, DateTimeKind.Unspecified), new Guid("ee541fb1-c5a1-464e-b33e-5d639454c4b1"), null, "Customer discount", null },
                    { new Guid("1d29c9f2-f86c-4175-aa2e-18f18a99ec7c"), 25m, new DateTime(2026, 6, 14, 0, 1, 0, 0, DateTimeKind.Unspecified), new Guid("cf0bb218-93c0-49f4-b0b0-6fbcf31387ce"), null, "Parking fee", null },
                    { new Guid("22a142c6-cebc-4dc6-a1be-5789cf88f5d7"), 30m, new DateTime(2026, 6, 12, 0, 1, 0, 0, DateTimeKind.Unspecified), new Guid("881b936f-5d52-46f1-9df1-0d6a83e6f412"), new DateTime(2026, 6, 14, 0, 0, 0, 0, DateTimeKind.Unspecified), "Parking fee", 0 },
                    { new Guid("27c4198b-ad0d-46a1-8dae-087c62418c9c"), 850m, new DateTime(2026, 6, 16, 0, 0, 0, 0, DateTimeKind.Unspecified), new Guid("3f5b28dc-e9a6-4838-84c5-8c4a61e09266"), new DateTime(2026, 6, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), "Transport fee", 0 },
                    { new Guid("2dc5f51b-16aa-4dd9-aeb8-5cce6a1e2f0c"), 825m, new DateTime(2026, 6, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), new Guid("58c318c5-696c-4008-9f0e-e362d8b04403"), null, "Transport fee", null },
                    { new Guid("3a03a709-5821-4aee-b781-b66fc27fa63e"), -50m, new DateTime(2026, 6, 14, 0, 3, 0, 0, DateTimeKind.Unspecified), new Guid("cf0bb218-93c0-49f4-b0b0-6fbcf31387ce"), null, "Customer discount", null },
                    { new Guid("3dcc0402-42b8-403e-a2ce-1462b2eeaf86"), 30m, new DateTime(2026, 6, 21, 0, 1, 0, 0, DateTimeKind.Unspecified), new Guid("ee541fb1-c5a1-464e-b33e-5d639454c4b1"), null, "Parking fee", null },
                    { new Guid("3ed5b89d-bf22-410d-98d4-59b801a96e9a"), 40m, new DateTime(2026, 6, 14, 0, 2, 0, 0, DateTimeKind.Unspecified), new Guid("cf0bb218-93c0-49f4-b0b0-6fbcf31387ce"), null, "Toll fee", null },
                    { new Guid("4228b425-c828-4917-bdb1-4eef9fd96af4"), 25m, new DateTime(2026, 6, 20, 0, 1, 0, 0, DateTimeKind.Unspecified), new Guid("74268bf2-b71b-4794-afd3-928ab2afc6fe"), null, "Parking fee", null },
                    { new Guid("45c92a5a-59f4-4c74-a7b4-8235fe73f531"), -60m, new DateTime(2026, 6, 19, 0, 3, 0, 0, DateTimeKind.Unspecified), new Guid("12a1bf6e-1406-47f3-8ed0-7b2aedbdf487"), null, "Customer discount", null },
                    { new Guid("49f6e057-7519-4786-b790-e2d9ad63649e"), 35m, new DateTime(2026, 6, 13, 0, 1, 0, 0, DateTimeKind.Unspecified), new Guid("9b2f9565-ca15-438a-99c6-6ab02173b96e"), null, "Parking fee", null },
                    { new Guid("4ff0719b-f996-46d0-94a6-adceba80e20f"), -50m, new DateTime(2026, 6, 18, 0, 3, 0, 0, DateTimeKind.Unspecified), new Guid("a6442421-97fc-4637-b743-6599840ecdf5"), null, "Customer discount", null },
                    { new Guid("6d14af15-d97d-4c3d-a38b-7714d417b9f4"), 800m, new DateTime(2026, 6, 14, 0, 0, 0, 0, DateTimeKind.Unspecified), new Guid("cf0bb218-93c0-49f4-b0b0-6fbcf31387ce"), null, "Transport fee", null },
                    { new Guid("6d6d7170-4369-459b-ac13-fb356887c30f"), 25m, new DateTime(2026, 6, 17, 0, 1, 0, 0, DateTimeKind.Unspecified), new Guid("45de5b74-7437-4bef-a4dd-239a861b4d91"), null, "Parking fee", null },
                    { new Guid("756f2e21-abaa-4830-a6b5-5d55543ecbea"), 900m, new DateTime(2026, 6, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), new Guid("a6442421-97fc-4637-b743-6599840ecdf5"), null, "Transport fee", null },
                    { new Guid("7a36ba67-6436-4f42-8cac-4701466a2e0c"), 35m, new DateTime(2026, 6, 19, 0, 1, 0, 0, DateTimeKind.Unspecified), new Guid("12a1bf6e-1406-47f3-8ed0-7b2aedbdf487"), null, "Parking fee", null },
                    { new Guid("830f0a3e-7fb8-4627-8082-8df26d88c6a2"), 50m, new DateTime(2026, 6, 19, 0, 2, 0, 0, DateTimeKind.Unspecified), new Guid("12a1bf6e-1406-47f3-8ed0-7b2aedbdf487"), null, "Toll fee", null },
                    { new Guid("89900e4a-c2d7-4327-a664-bbc28dd4ea6a"), -60m, new DateTime(2026, 6, 17, 0, 3, 0, 0, DateTimeKind.Unspecified), new Guid("45de5b74-7437-4bef-a4dd-239a861b4d91"), null, "Customer discount", null },
                    { new Guid("8bab5990-bed8-4ad8-af5a-2ec68f5ad022"), 35m, new DateTime(2026, 6, 16, 0, 1, 0, 0, DateTimeKind.Unspecified), new Guid("3f5b28dc-e9a6-4838-84c5-8c4a61e09266"), new DateTime(2026, 6, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), "Parking fee", 0 },
                    { new Guid("981130e4-c0a9-4c05-8b21-ff19abfd06a0"), 30m, new DateTime(2026, 6, 15, 0, 1, 0, 0, DateTimeKind.Unspecified), new Guid("58c318c5-696c-4008-9f0e-e362d8b04403"), null, "Parking fee", null },
                    { new Guid("a3445a8d-fab2-4718-a57b-99d9a82f0a02"), 50m, new DateTime(2026, 6, 15, 0, 2, 0, 0, DateTimeKind.Unspecified), new Guid("58c318c5-696c-4008-9f0e-e362d8b04403"), null, "Toll fee", null },
                    { new Guid("a398835b-f3fd-4671-9c19-898a1b24d05b"), 925m, new DateTime(2026, 6, 19, 0, 0, 0, 0, DateTimeKind.Unspecified), new Guid("12a1bf6e-1406-47f3-8ed0-7b2aedbdf487"), null, "Transport fee", null },
                    { new Guid("aa9b9bcd-3e04-43e5-9024-a2d064d6fe06"), -50m, new DateTime(2026, 6, 12, 0, 3, 0, 0, DateTimeKind.Unspecified), new Guid("881b936f-5d52-46f1-9df1-0d6a83e6f412"), new DateTime(2026, 6, 14, 0, 0, 0, 0, DateTimeKind.Unspecified), "Customer discount", 0 },
                    { new Guid("acd51f72-6f76-4cf7-9682-3af3618bc3bb"), 70m, new DateTime(2026, 6, 17, 0, 2, 0, 0, DateTimeKind.Unspecified), new Guid("45de5b74-7437-4bef-a4dd-239a861b4d91"), null, "Toll fee", null },
                    { new Guid("ad14df5a-4f27-44c6-9e9a-d77464c78d4e"), 30m, new DateTime(2026, 6, 18, 0, 1, 0, 0, DateTimeKind.Unspecified), new Guid("a6442421-97fc-4637-b743-6599840ecdf5"), null, "Parking fee", null },
                    { new Guid("b2e5ccf7-dac8-4a17-afd2-721926eb7aa6"), 950m, new DateTime(2026, 6, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), new Guid("74268bf2-b71b-4794-afd3-928ab2afc6fe"), null, "Transport fee", null },
                    { new Guid("cab81b58-c239-408a-8036-8aa67fcb6cd5"), 975m, new DateTime(2026, 6, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), new Guid("ee541fb1-c5a1-464e-b33e-5d639454c4b1"), null, "Transport fee", null },
                    { new Guid("cd2ed419-fe48-4d38-b6a2-e85af4989936"), 60m, new DateTime(2026, 6, 20, 0, 2, 0, 0, DateTimeKind.Unspecified), new Guid("74268bf2-b71b-4794-afd3-928ab2afc6fe"), null, "Toll fee", null },
                    { new Guid("ce26f948-48b5-4d56-8bc8-f54d5831aeb1"), 60m, new DateTime(2026, 6, 16, 0, 2, 0, 0, DateTimeKind.Unspecified), new Guid("3f5b28dc-e9a6-4838-84c5-8c4a61e09266"), new DateTime(2026, 6, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), "Toll fee", 0 },
                    { new Guid("d16607ad-661c-4bbb-8888-a2f42ced9183"), 70m, new DateTime(2026, 6, 13, 0, 2, 0, 0, DateTimeKind.Unspecified), new Guid("9b2f9565-ca15-438a-99c6-6ab02173b96e"), null, "Toll fee", null },
                    { new Guid("d9a8a8a4-4247-474a-8744-3428b64df8ce"), -50m, new DateTime(2026, 6, 20, 0, 3, 0, 0, DateTimeKind.Unspecified), new Guid("74268bf2-b71b-4794-afd3-928ab2afc6fe"), null, "Customer discount", null },
                    { new Guid("dcf53413-6297-47dd-af97-1a03f5fbd360"), 70m, new DateTime(2026, 6, 21, 0, 2, 0, 0, DateTimeKind.Unspecified), new Guid("ee541fb1-c5a1-464e-b33e-5d639454c4b1"), null, "Toll fee", null },
                    { new Guid("e2d06768-43db-4fdb-a9de-76f3e3b3d33f"), 775m, new DateTime(2026, 6, 13, 0, 0, 0, 0, DateTimeKind.Unspecified), new Guid("9b2f9565-ca15-438a-99c6-6ab02173b96e"), null, "Transport fee", null },
                    { new Guid("e2f82cf7-d30d-435d-9b69-1624966b4206"), 750m, new DateTime(2026, 6, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), new Guid("881b936f-5d52-46f1-9df1-0d6a83e6f412"), new DateTime(2026, 6, 14, 0, 0, 0, 0, DateTimeKind.Unspecified), "Transport fee", 0 },
                    { new Guid("f7a63390-55bc-4f97-8120-621f1ec0333e"), -60m, new DateTime(2026, 6, 13, 0, 3, 0, 0, DateTimeKind.Unspecified), new Guid("9b2f9565-ca15-438a-99c6-6ab02173b96e"), null, "Customer discount", null },
                    { new Guid("fb5b70d7-78e5-4fa2-ac5f-856212570c5f"), 40m, new DateTime(2026, 6, 18, 0, 2, 0, 0, DateTimeKind.Unspecified), new Guid("a6442421-97fc-4637-b743-6599840ecdf5"), null, "Toll fee", null },
                    { new Guid("fe6c87f7-885f-452a-af29-0154a1ed95d3"), 875m, new DateTime(2026, 6, 17, 0, 0, 0, 0, DateTimeKind.Unspecified), new Guid("45de5b74-7437-4bef-a4dd-239a861b4d91"), null, "Transport fee", null }
                });

            migrationBuilder.InsertData(
                table: "TrucksCourses",
                columns: new[] { "TruckCourseID", "AssignedDate", "DeliverAddress", "DeliveryDate", "Income", "PickupAddress", "Status", "TruckID" },
                values: new object[,]
                {
                    { new Guid("0b832a33-ced7-4023-99e7-cef1e086b753"), new DateTime(2026, 6, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), "Shumen, 45 Rishki Prohod Blvd.", null, 1150m, "Veliko Tarnovo, 11 Magistralna St.", 1, new Guid("e83b600b-b9a9-4f36-a4ce-3f4f002a2d1c") },
                    { new Guid("2ce8d831-bf38-48d6-878d-5951acb9c165"), new DateTime(2026, 6, 28, 0, 0, 0, 0, DateTimeKind.Unspecified), "Montana, 13 Treti Mart Blvd.", null, 1150m, "Shumen, 7 Simeon Veliki Blvd.", 1, new Guid("0cada001-5206-4aff-a0e5-4600bcfed88f") },
                    { new Guid("3bd2400a-914e-4c52-9bed-c72408668e06"), new DateTime(2026, 6, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), "Gabrovo, 6 Aprilov Blvd.", null, 1150m, "Stara Zagora, 9 Industrialna St.", 1, new Guid("d578effb-04cd-4c11-817d-a9ac580ea8be") },
                    { new Guid("4c9ad6ca-b469-4a5b-b79b-114116bb1780"), new DateTime(2026, 6, 27, 0, 0, 0, 0, DateTimeKind.Unspecified), "Sofia, 88 Bulgaria Blvd.", null, 1000m, "Blagoevgrad, 20 Sveti Dimitar Solunski Blvd.", 1, new Guid("0cada001-5206-4aff-a0e5-4600bcfed88f") },
                    { new Guid("5201ffcb-3020-4f81-9621-8e6495a75e68"), new DateTime(2026, 6, 24, 0, 0, 0, 0, DateTimeKind.Unspecified), "Pernik, 22 Yug Gagarin St.", null, 1000m, "Pleven, 27 Ruse Blvd.", 1, new Guid("e83b600b-b9a9-4f36-a4ce-3f4f002a2d1c") },
                    { new Guid("86e50922-8bfe-442a-9eef-5e0c5c9c45e0"), new DateTime(2026, 6, 23, 0, 0, 0, 0, DateTimeKind.Unspecified), "Blagoevgrad, 14 Vasil Levski Blvd.", null, 850m, "Pleven, 14 Bulgaria Blvd.", 1, new Guid("e83b600b-b9a9-4f36-a4ce-3f4f002a2d1c") },
                    { new Guid("c9510b2e-7cfd-47fd-b403-9cd1c61fc6af"), new DateTime(2026, 6, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), "Veliko Tarnovo, 30 Bulgaria Blvd.", null, 1000m, "Stara Zagora, 31 Patriarch Evtimiy Blvd.", 1, new Guid("d578effb-04cd-4c11-817d-a9ac580ea8be") },
                    { new Guid("ccdab565-ec35-4ee0-9038-89737a44882b"), new DateTime(2026, 6, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), "Lovech, 12 Bulgaria Blvd.", null, 850m, "Ruse, 17 Lipnik Blvd.", 1, new Guid("d578effb-04cd-4c11-817d-a9ac580ea8be") },
                    { new Guid("de561ca6-88c1-47cd-ae21-994d103e91ab"), new DateTime(2026, 6, 26, 0, 0, 0, 0, DateTimeKind.Unspecified), "Targovishte, 9 Mimi Balkanska St.", null, 850m, "Veliko Tarnovo, 44 Nikola Gabrovski Blvd.", 1, new Guid("0cada001-5206-4aff-a0e5-4600bcfed88f") }
                });

            migrationBuilder.InsertData(
                table: "TrucksSpendings",
                columns: new[] { "TruckSpendingID", "Amount", "PaymentDate", "PaymentDescription", "PaymentMethod", "SpendingType", "TruckCourseID", "TruckID" },
                values: new object[,]
                {
                    { new Guid("12a374f8-6ac2-4269-9705-0dc124e05116"), 160m, new DateTime(2026, 6, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), "Seed spending for course 2", 1, 1, new Guid("0399dfb7-4de1-4ab5-8db2-d45cb6f0932f"), new Guid("b6858daf-c51a-4a70-8246-6fc477d53111") },
                    { new Guid("17dcfc1c-e08e-428a-8bf0-bb1230a5d5cb"), 125m, new DateTime(2026, 6, 11, 0, 0, 0, 0, DateTimeKind.Unspecified), "Seed spending for course 1", 0, 0, new Guid("dbd66272-bee6-4114-a386-33090c0ba172"), new Guid("75c3da6c-8cc5-4bbc-b365-80c24b0216fc") },
                    { new Guid("43fdf95c-c128-4a9e-93fe-e389ebf87b7c"), 195m, new DateTime(2026, 6, 16, 0, 0, 0, 0, DateTimeKind.Unspecified), "Seed spending for course 3", 0, 0, new Guid("1c9204d6-e8cb-4929-a926-d55206215421"), new Guid("b6858daf-c51a-4a70-8246-6fc477d53111") },
                    { new Guid("c59ed4a7-e93c-4bc1-b311-da978616bac8"), 160m, new DateTime(2026, 6, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), "Seed spending for course 2", 1, 1, new Guid("6a8d56fc-d2b4-4aa0-9ea3-e460b72c9317"), new Guid("75c3da6c-8cc5-4bbc-b365-80c24b0216fc") },
                    { new Guid("c77617d6-115a-448c-8b1c-c9875e3313b7"), 195m, new DateTime(2026, 6, 13, 0, 0, 0, 0, DateTimeKind.Unspecified), "Seed spending for course 3", 0, 0, new Guid("2f671305-cd8f-40fa-a9c3-4d77525b6316"), new Guid("75c3da6c-8cc5-4bbc-b365-80c24b0216fc") },
                    { new Guid("d8e43760-1338-497b-8985-e88ae3cbfb05"), 195m, new DateTime(2026, 6, 19, 0, 0, 0, 0, DateTimeKind.Unspecified), "Seed spending for course 3", 0, 0, new Guid("5d8e5934-4800-4aca-b678-b859fcbf2c1b"), new Guid("24095c26-af58-43fa-9a87-6443d4ad469d") },
                    { new Guid("e2381928-4486-4926-b63a-8d124ebf9699"), 160m, new DateTime(2026, 6, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), "Seed spending for course 2", 1, 1, new Guid("92077f74-61b3-4893-86f9-8f7026c952a0"), new Guid("24095c26-af58-43fa-9a87-6443d4ad469d") },
                    { new Guid("f050380e-6758-43e6-8e49-e018eb2d46f8"), 125m, new DateTime(2026, 6, 14, 0, 0, 0, 0, DateTimeKind.Unspecified), "Seed spending for course 1", 0, 0, new Guid("29b586bf-5f04-4882-9cdf-5946ea3ecec3"), new Guid("b6858daf-c51a-4a70-8246-6fc477d53111") },
                    { new Guid("fc8fcab6-8607-4304-9f4d-35ec127ada6c"), 125m, new DateTime(2026, 6, 17, 0, 0, 0, 0, DateTimeKind.Unspecified), "Seed spending for course 1", 0, 0, new Guid("ff91895b-3124-4f22-9198-989aaa34a799"), new Guid("24095c26-af58-43fa-9a87-6443d4ad469d") }
                });

            migrationBuilder.InsertData(
                table: "Messages",
                columns: new[] { "MessageID", "Content", "NotificationID", "ReceiverID", "SenderID", "SentAt" },
                values: new object[] { new Guid("5a63a646-d8f5-4cb5-a27f-767cd29308f5"), "Understood, I will use the eastern bypass.", new Guid("79fec001-e9c9-4806-b96f-25daeb5de2ff"), new Guid("051eafea-deaa-4fbe-b442-25b7be18fa8e"), new Guid("c835750b-0e0e-4fb0-88b6-a4b557e52c21"), new DateTime(2026, 6, 17, 9, 1, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.InsertData(
                table: "Messages",
                columns: new[] { "MessageID", "Content", "IsRead", "NotificationID", "ReceiverID", "SenderID", "SentAt" },
                values: new object[] { new Guid("ef02728a-b6bf-46df-8efc-70ed2deb0386"), "Use the eastern bypass because of roadworks.", true, new Guid("79fec001-e9c9-4806-b96f-25daeb5de2ff"), new Guid("c835750b-0e0e-4fb0-88b6-a4b557e52c21"), new Guid("051eafea-deaa-4fbe-b442-25b7be18fa8e"), new DateTime(2026, 6, 17, 9, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.InsertData(
                table: "Notifications",
                columns: new[] { "NotificationID", "CourseID", "CreatedAt", "Message", "OrderID", "PaymentID", "ReceiverID", "SenderID", "Title", "TruckID", "TruckSpendingID" },
                values: new object[] { new Guid("48590d68-bd83-4386-9524-ac634ce4e559"), new Guid("dbd66272-bee6-4114-a386-33090c0ba172"), new DateTime(2026, 6, 15, 5, 0, 0, 0, DateTimeKind.Unspecified), "A truck spending entry has been recorded.", null, null, new Guid("cca021da-da51-4bd7-b968-8294cf006dfd"), null, "Truck spending recorded", new Guid("75c3da6c-8cc5-4bbc-b365-80c24b0216fc"), new Guid("17dcfc1c-e08e-428a-8bf0-bb1230a5d5cb") });

            migrationBuilder.InsertData(
                table: "TrucksSpendings",
                columns: new[] { "TruckSpendingID", "Amount", "PaymentDate", "PaymentDescription", "PaymentMethod", "SpendingType", "TruckCourseID", "TruckID" },
                values: new object[,]
                {
                    { new Guid("1d4c2088-f55b-406f-8269-3cb03f4883e2"), 195m, new DateTime(2026, 6, 28, 0, 0, 0, 0, DateTimeKind.Unspecified), "Seed spending for course 3", 0, 0, new Guid("2ce8d831-bf38-48d6-878d-5951acb9c165"), new Guid("0cada001-5206-4aff-a0e5-4600bcfed88f") },
                    { new Guid("3f65d30c-f1ec-4bbc-9adb-1459e8445f43"), 125m, new DateTime(2026, 6, 23, 0, 0, 0, 0, DateTimeKind.Unspecified), "Seed spending for course 1", 0, 0, new Guid("86e50922-8bfe-442a-9eef-5e0c5c9c45e0"), new Guid("e83b600b-b9a9-4f36-a4ce-3f4f002a2d1c") },
                    { new Guid("4f49a12d-f149-41a0-bbaa-72ed9a2be55a"), 160m, new DateTime(2026, 6, 24, 0, 0, 0, 0, DateTimeKind.Unspecified), "Seed spending for course 2", 1, 1, new Guid("5201ffcb-3020-4f81-9621-8e6495a75e68"), new Guid("e83b600b-b9a9-4f36-a4ce-3f4f002a2d1c") },
                    { new Guid("83a8f9a2-c0f5-4efb-af60-e68832885dc0"), 125m, new DateTime(2026, 6, 26, 0, 0, 0, 0, DateTimeKind.Unspecified), "Seed spending for course 1", 0, 0, new Guid("de561ca6-88c1-47cd-ae21-994d103e91ab"), new Guid("0cada001-5206-4aff-a0e5-4600bcfed88f") },
                    { new Guid("8d2aed33-c631-48eb-bddf-1d4cbec22f51"), 160m, new DateTime(2026, 6, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), "Seed spending for course 2", 1, 1, new Guid("c9510b2e-7cfd-47fd-b403-9cd1c61fc6af"), new Guid("d578effb-04cd-4c11-817d-a9ac580ea8be") },
                    { new Guid("abe84274-a582-4b5a-a065-e612dfdf3e91"), 160m, new DateTime(2026, 6, 27, 0, 0, 0, 0, DateTimeKind.Unspecified), "Seed spending for course 2", 1, 1, new Guid("4c9ad6ca-b469-4a5b-b79b-114116bb1780"), new Guid("0cada001-5206-4aff-a0e5-4600bcfed88f") },
                    { new Guid("c4663446-da52-4ec1-a89f-ec9e03b90fbd"), 195m, new DateTime(2026, 6, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), "Seed spending for course 3", 0, 0, new Guid("3bd2400a-914e-4c52-9bed-c72408668e06"), new Guid("d578effb-04cd-4c11-817d-a9ac580ea8be") },
                    { new Guid("ee08dc73-321a-4158-96d0-efa130b114e7"), 125m, new DateTime(2026, 6, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), "Seed spending for course 1", 0, 0, new Guid("ccdab565-ec35-4ee0-9038-89737a44882b"), new Guid("d578effb-04cd-4c11-817d-a9ac580ea8be") },
                    { new Guid("f985a12c-6716-45a7-8b1f-a12bbdf3b655"), 195m, new DateTime(2026, 6, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), "Seed spending for course 3", 0, 0, new Guid("0b832a33-ced7-4023-99e7-cef1e086b753"), new Guid("e83b600b-b9a9-4f36-a4ce-3f4f002a2d1c") }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { new Guid("782b39c1-d0e7-4f18-8a2b-1077a438c048"), new Guid("583560aa-73c7-4c08-9e32-958581be98eb") });

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { new Guid("d349122c-ed4a-43ca-b939-b3a9ff9fa0fb"), new Guid("c835750b-0e0e-4fb0-88b6-a4b557e52c21") });

            migrationBuilder.DeleteData(
                table: "CompanyProfiles",
                keyColumn: "UserId",
                keyValue: new Guid("583560aa-73c7-4c08-9e32-958581be98eb"));

            migrationBuilder.DeleteData(
                table: "CoursesOrders",
                keyColumns: new[] { "OrderID", "TruckCourseID" },
                keyValues: new object[] { new Guid("197325b6-f955-44d9-82a3-d7432a5d77d4"), new Guid("dbd66272-bee6-4114-a386-33090c0ba172") });

            migrationBuilder.DeleteData(
                table: "CoursesOrders",
                keyColumns: new[] { "OrderID", "TruckCourseID" },
                keyValues: new object[] { new Guid("24b95dc1-69d4-4ba9-b91e-d0b691ad729c"), new Guid("6a8d56fc-d2b4-4aa0-9ea3-e460b72c9317") });

            migrationBuilder.DeleteData(
                table: "CoursesOrders",
                keyColumns: new[] { "OrderID", "TruckCourseID" },
                keyValues: new object[] { new Guid("2be2d17f-f954-4396-a169-0efe0f58c43d"), new Guid("29b586bf-5f04-4882-9cdf-5946ea3ecec3") });

            migrationBuilder.DeleteData(
                table: "CoursesOrders",
                keyColumns: new[] { "OrderID", "TruckCourseID" },
                keyValues: new object[] { new Guid("32276d6e-9c7d-46a9-ad2f-3f41b72639a0"), new Guid("dbd66272-bee6-4114-a386-33090c0ba172") });

            migrationBuilder.DeleteData(
                table: "CoursesOrders",
                keyColumns: new[] { "OrderID", "TruckCourseID" },
                keyValues: new object[] { new Guid("3627fcd5-dfce-4967-9372-f084648cc02b"), new Guid("2f671305-cd8f-40fa-a9c3-4d77525b6316") });

            migrationBuilder.DeleteData(
                table: "CoursesOrders",
                keyColumns: new[] { "OrderID", "TruckCourseID" },
                keyValues: new object[] { new Guid("3f5b28dc-e9a6-4838-84c5-8c4a61e09266"), new Guid("92077f74-61b3-4893-86f9-8f7026c952a0") });

            migrationBuilder.DeleteData(
                table: "CoursesOrders",
                keyColumns: new[] { "OrderID", "TruckCourseID" },
                keyValues: new object[] { new Guid("45de5b74-7437-4bef-a4dd-239a861b4d91"), new Guid("92077f74-61b3-4893-86f9-8f7026c952a0") });

            migrationBuilder.DeleteData(
                table: "CoursesOrders",
                keyColumns: new[] { "OrderID", "TruckCourseID" },
                keyValues: new object[] { new Guid("58c318c5-696c-4008-9f0e-e362d8b04403"), new Guid("ff91895b-3124-4f22-9198-989aaa34a799") });

            migrationBuilder.DeleteData(
                table: "CoursesOrders",
                keyColumns: new[] { "OrderID", "TruckCourseID" },
                keyValues: new object[] { new Guid("64cce443-8d08-4ca8-8874-14b50097dcfb"), new Guid("0399dfb7-4de1-4ab5-8db2-d45cb6f0932f") });

            migrationBuilder.DeleteData(
                table: "CoursesOrders",
                keyColumns: new[] { "OrderID", "TruckCourseID" },
                keyValues: new object[] { new Guid("756e41df-edf9-417d-9586-52cd39fd6649"), new Guid("6a8d56fc-d2b4-4aa0-9ea3-e460b72c9317") });

            migrationBuilder.DeleteData(
                table: "CoursesOrders",
                keyColumns: new[] { "OrderID", "TruckCourseID" },
                keyValues: new object[] { new Guid("838aad0d-d377-46fc-8425-e00bae35680f"), new Guid("0399dfb7-4de1-4ab5-8db2-d45cb6f0932f") });

            migrationBuilder.DeleteData(
                table: "CoursesOrders",
                keyColumns: new[] { "OrderID", "TruckCourseID" },
                keyValues: new object[] { new Guid("881b936f-5d52-46f1-9df1-0d6a83e6f412"), new Guid("1c9204d6-e8cb-4929-a926-d55206215421") });

            migrationBuilder.DeleteData(
                table: "CoursesOrders",
                keyColumns: new[] { "OrderID", "TruckCourseID" },
                keyValues: new object[] { new Guid("946d7fb1-7b00-4aed-a55b-47f4776f3a87"), new Guid("2f671305-cd8f-40fa-a9c3-4d77525b6316") });

            migrationBuilder.DeleteData(
                table: "CoursesOrders",
                keyColumns: new[] { "OrderID", "TruckCourseID" },
                keyValues: new object[] { new Guid("9790ca1e-e41a-467b-a237-1d4041e07692"), new Guid("29b586bf-5f04-4882-9cdf-5946ea3ecec3") });

            migrationBuilder.DeleteData(
                table: "CoursesOrders",
                keyColumns: new[] { "OrderID", "TruckCourseID" },
                keyValues: new object[] { new Guid("9b2f9565-ca15-438a-99c6-6ab02173b96e"), new Guid("1c9204d6-e8cb-4929-a926-d55206215421") });

            migrationBuilder.DeleteData(
                table: "CoursesOrders",
                keyColumns: new[] { "OrderID", "TruckCourseID" },
                keyValues: new object[] { new Guid("cf0bb218-93c0-49f4-b0b0-6fbcf31387ce"), new Guid("ff91895b-3124-4f22-9198-989aaa34a799") });

            migrationBuilder.DeleteData(
                table: "Messages",
                keyColumn: "MessageID",
                keyValue: new Guid("33131dcd-474e-4c3f-bf36-0bfb4dc30965"));

            migrationBuilder.DeleteData(
                table: "Messages",
                keyColumn: "MessageID",
                keyValue: new Guid("5a63a646-d8f5-4cb5-a27f-767cd29308f5"));

            migrationBuilder.DeleteData(
                table: "Messages",
                keyColumn: "MessageID",
                keyValue: new Guid("86eb832d-3427-40d3-b346-ed51b84860a3"));

            migrationBuilder.DeleteData(
                table: "Messages",
                keyColumn: "MessageID",
                keyValue: new Guid("883093da-81c0-4f16-aceb-2414d570f128"));

            migrationBuilder.DeleteData(
                table: "Messages",
                keyColumn: "MessageID",
                keyValue: new Guid("e62daba9-4779-4012-a6eb-700ab315b294"));

            migrationBuilder.DeleteData(
                table: "Messages",
                keyColumn: "MessageID",
                keyValue: new Guid("ef02728a-b6bf-46df-8efc-70ed2deb0386"));

            migrationBuilder.DeleteData(
                table: "Notifications",
                keyColumn: "NotificationID",
                keyValue: new Guid("277ac80f-bd18-4a13-89f5-3c4dec699124"));

            migrationBuilder.DeleteData(
                table: "Notifications",
                keyColumn: "NotificationID",
                keyValue: new Guid("27bd020a-fd3d-47f5-a635-de61f6fee0bc"));

            migrationBuilder.DeleteData(
                table: "Notifications",
                keyColumn: "NotificationID",
                keyValue: new Guid("48590d68-bd83-4386-9524-ac634ce4e559"));

            migrationBuilder.DeleteData(
                table: "Notifications",
                keyColumn: "NotificationID",
                keyValue: new Guid("68f34703-4ace-4490-91db-770f19eda186"));

            migrationBuilder.DeleteData(
                table: "Notifications",
                keyColumn: "NotificationID",
                keyValue: new Guid("6fb7a9ee-7b69-479b-b761-7982186a2db0"));

            migrationBuilder.DeleteData(
                table: "Notifications",
                keyColumn: "NotificationID",
                keyValue: new Guid("825924f2-3f9d-4bf9-879a-89ca9386c2a1"));

            migrationBuilder.DeleteData(
                table: "Notifications",
                keyColumn: "NotificationID",
                keyValue: new Guid("a194bc33-df7b-4a12-b53e-541833d8c20c"));

            migrationBuilder.DeleteData(
                table: "Notifications",
                keyColumn: "NotificationID",
                keyValue: new Guid("c2c5dc86-8205-44ca-b9e0-5c35b27dbfda"));

            migrationBuilder.DeleteData(
                table: "Notifications",
                keyColumn: "NotificationID",
                keyValue: new Guid("d4948efb-fa00-4e9b-8664-e468bcbdb701"));

            migrationBuilder.DeleteData(
                table: "Payments",
                keyColumn: "PaymentID",
                keyValue: new Guid("00348107-1dbe-40fe-bf57-6c3d5d244300"));

            migrationBuilder.DeleteData(
                table: "Payments",
                keyColumn: "PaymentID",
                keyValue: new Guid("0129248c-b047-44b2-9a45-835cc752fa02"));

            migrationBuilder.DeleteData(
                table: "Payments",
                keyColumn: "PaymentID",
                keyValue: new Guid("0967ba5c-d0a8-4dc9-8117-ef33030c560d"));

            migrationBuilder.DeleteData(
                table: "Payments",
                keyColumn: "PaymentID",
                keyValue: new Guid("0f566482-5add-4d44-8bea-e0952509a381"));

            migrationBuilder.DeleteData(
                table: "Payments",
                keyColumn: "PaymentID",
                keyValue: new Guid("10899709-c9c2-4396-9606-06b4d060a2c2"));

            migrationBuilder.DeleteData(
                table: "Payments",
                keyColumn: "PaymentID",
                keyValue: new Guid("14748ef4-f20f-440d-a9f6-6a26021fc601"));

            migrationBuilder.DeleteData(
                table: "Payments",
                keyColumn: "PaymentID",
                keyValue: new Guid("1ba7af08-41ea-4d9a-8511-400b48a6a8ba"));

            migrationBuilder.DeleteData(
                table: "Payments",
                keyColumn: "PaymentID",
                keyValue: new Guid("1d29c9f2-f86c-4175-aa2e-18f18a99ec7c"));

            migrationBuilder.DeleteData(
                table: "Payments",
                keyColumn: "PaymentID",
                keyValue: new Guid("1e520388-1eed-456d-b2fa-5a85212e9c27"));

            migrationBuilder.DeleteData(
                table: "Payments",
                keyColumn: "PaymentID",
                keyValue: new Guid("22a142c6-cebc-4dc6-a1be-5789cf88f5d7"));

            migrationBuilder.DeleteData(
                table: "Payments",
                keyColumn: "PaymentID",
                keyValue: new Guid("27c4198b-ad0d-46a1-8dae-087c62418c9c"));

            migrationBuilder.DeleteData(
                table: "Payments",
                keyColumn: "PaymentID",
                keyValue: new Guid("2dc5f51b-16aa-4dd9-aeb8-5cce6a1e2f0c"));

            migrationBuilder.DeleteData(
                table: "Payments",
                keyColumn: "PaymentID",
                keyValue: new Guid("31982e7a-68c6-4bea-95e9-421d3c684401"));

            migrationBuilder.DeleteData(
                table: "Payments",
                keyColumn: "PaymentID",
                keyValue: new Guid("3644313f-feb7-4f6e-8c2f-281d3fb44d75"));

            migrationBuilder.DeleteData(
                table: "Payments",
                keyColumn: "PaymentID",
                keyValue: new Guid("3a03a709-5821-4aee-b781-b66fc27fa63e"));

            migrationBuilder.DeleteData(
                table: "Payments",
                keyColumn: "PaymentID",
                keyValue: new Guid("3dcc0402-42b8-403e-a2ce-1462b2eeaf86"));

            migrationBuilder.DeleteData(
                table: "Payments",
                keyColumn: "PaymentID",
                keyValue: new Guid("3ed5b89d-bf22-410d-98d4-59b801a96e9a"));

            migrationBuilder.DeleteData(
                table: "Payments",
                keyColumn: "PaymentID",
                keyValue: new Guid("3f748138-84c9-4c59-b2e6-74c104e30883"));

            migrationBuilder.DeleteData(
                table: "Payments",
                keyColumn: "PaymentID",
                keyValue: new Guid("4228b425-c828-4917-bdb1-4eef9fd96af4"));

            migrationBuilder.DeleteData(
                table: "Payments",
                keyColumn: "PaymentID",
                keyValue: new Guid("45c92a5a-59f4-4c74-a7b4-8235fe73f531"));

            migrationBuilder.DeleteData(
                table: "Payments",
                keyColumn: "PaymentID",
                keyValue: new Guid("49f6e057-7519-4786-b790-e2d9ad63649e"));

            migrationBuilder.DeleteData(
                table: "Payments",
                keyColumn: "PaymentID",
                keyValue: new Guid("4b91ce9b-b133-4910-b328-d8209b87aad5"));

            migrationBuilder.DeleteData(
                table: "Payments",
                keyColumn: "PaymentID",
                keyValue: new Guid("4ff0719b-f996-46d0-94a6-adceba80e20f"));

            migrationBuilder.DeleteData(
                table: "Payments",
                keyColumn: "PaymentID",
                keyValue: new Guid("50647686-134d-45e8-90c8-9ad5dbdc6038"));

            migrationBuilder.DeleteData(
                table: "Payments",
                keyColumn: "PaymentID",
                keyValue: new Guid("525be9f3-02a9-47cb-86ba-f4c441e99d17"));

            migrationBuilder.DeleteData(
                table: "Payments",
                keyColumn: "PaymentID",
                keyValue: new Guid("59378b0a-b9a5-43be-b0d1-7012731202d5"));

            migrationBuilder.DeleteData(
                table: "Payments",
                keyColumn: "PaymentID",
                keyValue: new Guid("5e7818a1-9284-487f-995f-05a5e47c32c6"));

            migrationBuilder.DeleteData(
                table: "Payments",
                keyColumn: "PaymentID",
                keyValue: new Guid("64308068-9c94-47e6-a9b3-d98d91898d13"));

            migrationBuilder.DeleteData(
                table: "Payments",
                keyColumn: "PaymentID",
                keyValue: new Guid("64db478c-86db-47fd-ae54-8a551f9e238a"));

            migrationBuilder.DeleteData(
                table: "Payments",
                keyColumn: "PaymentID",
                keyValue: new Guid("663b3320-c0cf-47cc-b642-564a43271f6c"));

            migrationBuilder.DeleteData(
                table: "Payments",
                keyColumn: "PaymentID",
                keyValue: new Guid("6be17a37-220c-46b9-8e82-fd268ef54049"));

            migrationBuilder.DeleteData(
                table: "Payments",
                keyColumn: "PaymentID",
                keyValue: new Guid("6d14af15-d97d-4c3d-a38b-7714d417b9f4"));

            migrationBuilder.DeleteData(
                table: "Payments",
                keyColumn: "PaymentID",
                keyValue: new Guid("6d6d7170-4369-459b-ac13-fb356887c30f"));

            migrationBuilder.DeleteData(
                table: "Payments",
                keyColumn: "PaymentID",
                keyValue: new Guid("74a07ab0-6c8f-48f3-93f7-ec98219c8090"));

            migrationBuilder.DeleteData(
                table: "Payments",
                keyColumn: "PaymentID",
                keyValue: new Guid("7517c585-dc47-4c0b-9bb9-b9afee7cbbac"));

            migrationBuilder.DeleteData(
                table: "Payments",
                keyColumn: "PaymentID",
                keyValue: new Guid("756f2e21-abaa-4830-a6b5-5d55543ecbea"));

            migrationBuilder.DeleteData(
                table: "Payments",
                keyColumn: "PaymentID",
                keyValue: new Guid("77543160-c90a-468f-b01a-29072dc45acd"));

            migrationBuilder.DeleteData(
                table: "Payments",
                keyColumn: "PaymentID",
                keyValue: new Guid("7a36ba67-6436-4f42-8cac-4701466a2e0c"));

            migrationBuilder.DeleteData(
                table: "Payments",
                keyColumn: "PaymentID",
                keyValue: new Guid("830f0a3e-7fb8-4627-8082-8df26d88c6a2"));

            migrationBuilder.DeleteData(
                table: "Payments",
                keyColumn: "PaymentID",
                keyValue: new Guid("87ce77c6-50d3-4d42-ab35-db03001a64f7"));

            migrationBuilder.DeleteData(
                table: "Payments",
                keyColumn: "PaymentID",
                keyValue: new Guid("89900e4a-c2d7-4327-a664-bbc28dd4ea6a"));

            migrationBuilder.DeleteData(
                table: "Payments",
                keyColumn: "PaymentID",
                keyValue: new Guid("8bab5990-bed8-4ad8-af5a-2ec68f5ad022"));

            migrationBuilder.DeleteData(
                table: "Payments",
                keyColumn: "PaymentID",
                keyValue: new Guid("8be3b081-5e59-4e6d-b718-ccac43b0f4e1"));

            migrationBuilder.DeleteData(
                table: "Payments",
                keyColumn: "PaymentID",
                keyValue: new Guid("8ef41387-4470-435c-8c15-86b422347b52"));

            migrationBuilder.DeleteData(
                table: "Payments",
                keyColumn: "PaymentID",
                keyValue: new Guid("91b2516a-16b1-47cf-87da-cd708af569d2"));

            migrationBuilder.DeleteData(
                table: "Payments",
                keyColumn: "PaymentID",
                keyValue: new Guid("91c5ab1b-ff4f-404a-bcb8-29ce474c50b9"));

            migrationBuilder.DeleteData(
                table: "Payments",
                keyColumn: "PaymentID",
                keyValue: new Guid("981130e4-c0a9-4c05-8b21-ff19abfd06a0"));

            migrationBuilder.DeleteData(
                table: "Payments",
                keyColumn: "PaymentID",
                keyValue: new Guid("995da2c1-a429-48b0-9bb6-41742a4f95d2"));

            migrationBuilder.DeleteData(
                table: "Payments",
                keyColumn: "PaymentID",
                keyValue: new Guid("a2649491-7455-4e9d-a158-61de1c7c0a5b"));

            migrationBuilder.DeleteData(
                table: "Payments",
                keyColumn: "PaymentID",
                keyValue: new Guid("a3445a8d-fab2-4718-a57b-99d9a82f0a02"));

            migrationBuilder.DeleteData(
                table: "Payments",
                keyColumn: "PaymentID",
                keyValue: new Guid("a398835b-f3fd-4671-9c19-898a1b24d05b"));

            migrationBuilder.DeleteData(
                table: "Payments",
                keyColumn: "PaymentID",
                keyValue: new Guid("a93caa41-1c70-464f-be4f-3f9d836d6a2b"));

            migrationBuilder.DeleteData(
                table: "Payments",
                keyColumn: "PaymentID",
                keyValue: new Guid("aa9b9bcd-3e04-43e5-9024-a2d064d6fe06"));

            migrationBuilder.DeleteData(
                table: "Payments",
                keyColumn: "PaymentID",
                keyValue: new Guid("acd51f72-6f76-4cf7-9682-3af3618bc3bb"));

            migrationBuilder.DeleteData(
                table: "Payments",
                keyColumn: "PaymentID",
                keyValue: new Guid("ad14df5a-4f27-44c6-9e9a-d77464c78d4e"));

            migrationBuilder.DeleteData(
                table: "Payments",
                keyColumn: "PaymentID",
                keyValue: new Guid("b2c1e70b-c270-48e3-b82b-0025ba0aee51"));

            migrationBuilder.DeleteData(
                table: "Payments",
                keyColumn: "PaymentID",
                keyValue: new Guid("b2e5ccf7-dac8-4a17-afd2-721926eb7aa6"));

            migrationBuilder.DeleteData(
                table: "Payments",
                keyColumn: "PaymentID",
                keyValue: new Guid("b4437aae-2b71-4442-8f3b-57b3747b45f1"));

            migrationBuilder.DeleteData(
                table: "Payments",
                keyColumn: "PaymentID",
                keyValue: new Guid("b93fc098-2bad-475c-84b2-2957c0945a25"));

            migrationBuilder.DeleteData(
                table: "Payments",
                keyColumn: "PaymentID",
                keyValue: new Guid("c24d76aa-d290-4f1e-aa29-a5a0328e21d3"));

            migrationBuilder.DeleteData(
                table: "Payments",
                keyColumn: "PaymentID",
                keyValue: new Guid("cab81b58-c239-408a-8036-8aa67fcb6cd5"));

            migrationBuilder.DeleteData(
                table: "Payments",
                keyColumn: "PaymentID",
                keyValue: new Guid("cd2ed419-fe48-4d38-b6a2-e85af4989936"));

            migrationBuilder.DeleteData(
                table: "Payments",
                keyColumn: "PaymentID",
                keyValue: new Guid("ce26f948-48b5-4d56-8bc8-f54d5831aeb1"));

            migrationBuilder.DeleteData(
                table: "Payments",
                keyColumn: "PaymentID",
                keyValue: new Guid("d16607ad-661c-4bbb-8888-a2f42ced9183"));

            migrationBuilder.DeleteData(
                table: "Payments",
                keyColumn: "PaymentID",
                keyValue: new Guid("d4ddf38e-0676-4129-bc44-474408cbe273"));

            migrationBuilder.DeleteData(
                table: "Payments",
                keyColumn: "PaymentID",
                keyValue: new Guid("d7fac92d-9985-48c3-83ae-55550f10a40a"));

            migrationBuilder.DeleteData(
                table: "Payments",
                keyColumn: "PaymentID",
                keyValue: new Guid("d9a8a8a4-4247-474a-8744-3428b64df8ce"));

            migrationBuilder.DeleteData(
                table: "Payments",
                keyColumn: "PaymentID",
                keyValue: new Guid("dcf53413-6297-47dd-af97-1a03f5fbd360"));

            migrationBuilder.DeleteData(
                table: "Payments",
                keyColumn: "PaymentID",
                keyValue: new Guid("e2d06768-43db-4fdb-a9de-76f3e3b3d33f"));

            migrationBuilder.DeleteData(
                table: "Payments",
                keyColumn: "PaymentID",
                keyValue: new Guid("e2f82cf7-d30d-435d-9b69-1624966b4206"));

            migrationBuilder.DeleteData(
                table: "Payments",
                keyColumn: "PaymentID",
                keyValue: new Guid("e37aaca1-e9f3-4116-a6fd-9f292a334e12"));

            migrationBuilder.DeleteData(
                table: "Payments",
                keyColumn: "PaymentID",
                keyValue: new Guid("e916f797-a08f-4b41-befd-d45b2129ff61"));

            migrationBuilder.DeleteData(
                table: "Payments",
                keyColumn: "PaymentID",
                keyValue: new Guid("eb81ff22-ad1d-4449-bdb8-82c9c9d3ed9d"));

            migrationBuilder.DeleteData(
                table: "Payments",
                keyColumn: "PaymentID",
                keyValue: new Guid("f5fe8642-b20e-4e19-9e7f-5032cd376313"));

            migrationBuilder.DeleteData(
                table: "Payments",
                keyColumn: "PaymentID",
                keyValue: new Guid("f7a63390-55bc-4f97-8120-621f1ec0333e"));

            migrationBuilder.DeleteData(
                table: "Payments",
                keyColumn: "PaymentID",
                keyValue: new Guid("f9a1dd71-d6a0-4ee7-8d6e-7451a548ff68"));

            migrationBuilder.DeleteData(
                table: "Payments",
                keyColumn: "PaymentID",
                keyValue: new Guid("fb5b70d7-78e5-4fa2-ac5f-856212570c5f"));

            migrationBuilder.DeleteData(
                table: "Payments",
                keyColumn: "PaymentID",
                keyValue: new Guid("fe6c87f7-885f-452a-af29-0154a1ed95d3"));

            migrationBuilder.DeleteData(
                table: "Payments",
                keyColumn: "PaymentID",
                keyValue: new Guid("fe79695e-8d83-4e79-98d1-b952fcccf69e"));

            migrationBuilder.DeleteData(
                table: "PersonalProfiles",
                keyColumn: "UserId",
                keyValue: new Guid("c835750b-0e0e-4fb0-88b6-a4b557e52c21"));

            migrationBuilder.DeleteData(
                table: "TrucksSpendings",
                keyColumn: "TruckSpendingID",
                keyValue: new Guid("12a374f8-6ac2-4269-9705-0dc124e05116"));

            migrationBuilder.DeleteData(
                table: "TrucksSpendings",
                keyColumn: "TruckSpendingID",
                keyValue: new Guid("1d4c2088-f55b-406f-8269-3cb03f4883e2"));

            migrationBuilder.DeleteData(
                table: "TrucksSpendings",
                keyColumn: "TruckSpendingID",
                keyValue: new Guid("3f65d30c-f1ec-4bbc-9adb-1459e8445f43"));

            migrationBuilder.DeleteData(
                table: "TrucksSpendings",
                keyColumn: "TruckSpendingID",
                keyValue: new Guid("43fdf95c-c128-4a9e-93fe-e389ebf87b7c"));

            migrationBuilder.DeleteData(
                table: "TrucksSpendings",
                keyColumn: "TruckSpendingID",
                keyValue: new Guid("4f49a12d-f149-41a0-bbaa-72ed9a2be55a"));

            migrationBuilder.DeleteData(
                table: "TrucksSpendings",
                keyColumn: "TruckSpendingID",
                keyValue: new Guid("83a8f9a2-c0f5-4efb-af60-e68832885dc0"));

            migrationBuilder.DeleteData(
                table: "TrucksSpendings",
                keyColumn: "TruckSpendingID",
                keyValue: new Guid("8d2aed33-c631-48eb-bddf-1d4cbec22f51"));

            migrationBuilder.DeleteData(
                table: "TrucksSpendings",
                keyColumn: "TruckSpendingID",
                keyValue: new Guid("abe84274-a582-4b5a-a065-e612dfdf3e91"));

            migrationBuilder.DeleteData(
                table: "TrucksSpendings",
                keyColumn: "TruckSpendingID",
                keyValue: new Guid("c4663446-da52-4ec1-a89f-ec9e03b90fbd"));

            migrationBuilder.DeleteData(
                table: "TrucksSpendings",
                keyColumn: "TruckSpendingID",
                keyValue: new Guid("c59ed4a7-e93c-4bc1-b311-da978616bac8"));

            migrationBuilder.DeleteData(
                table: "TrucksSpendings",
                keyColumn: "TruckSpendingID",
                keyValue: new Guid("c77617d6-115a-448c-8b1c-c9875e3313b7"));

            migrationBuilder.DeleteData(
                table: "TrucksSpendings",
                keyColumn: "TruckSpendingID",
                keyValue: new Guid("d8e43760-1338-497b-8985-e88ae3cbfb05"));

            migrationBuilder.DeleteData(
                table: "TrucksSpendings",
                keyColumn: "TruckSpendingID",
                keyValue: new Guid("e2381928-4486-4926-b63a-8d124ebf9699"));

            migrationBuilder.DeleteData(
                table: "TrucksSpendings",
                keyColumn: "TruckSpendingID",
                keyValue: new Guid("ee08dc73-321a-4158-96d0-efa130b114e7"));

            migrationBuilder.DeleteData(
                table: "TrucksSpendings",
                keyColumn: "TruckSpendingID",
                keyValue: new Guid("f050380e-6758-43e6-8e49-e018eb2d46f8"));

            migrationBuilder.DeleteData(
                table: "TrucksSpendings",
                keyColumn: "TruckSpendingID",
                keyValue: new Guid("f985a12c-6716-45a7-8b1f-a12bbdf3b655"));

            migrationBuilder.DeleteData(
                table: "TrucksSpendings",
                keyColumn: "TruckSpendingID",
                keyValue: new Guid("fc8fcab6-8607-4304-9f4d-35ec127ada6c"));

            migrationBuilder.DeleteData(
                table: "Notifications",
                keyColumn: "NotificationID",
                keyValue: new Guid("79fec001-e9c9-4806-b96f-25daeb5de2ff"));

            migrationBuilder.DeleteData(
                table: "Notifications",
                keyColumn: "NotificationID",
                keyValue: new Guid("acfc3587-f1c7-476d-a8f8-26e535df971c"));

            migrationBuilder.DeleteData(
                table: "Notifications",
                keyColumn: "NotificationID",
                keyValue: new Guid("fea90b4a-ec4c-4fc8-b5d2-07342c709fad"));

            migrationBuilder.DeleteData(
                table: "Orders",
                keyColumn: "OrderID",
                keyValue: new Guid("12a1bf6e-1406-47f3-8ed0-7b2aedbdf487"));

            migrationBuilder.DeleteData(
                table: "Orders",
                keyColumn: "OrderID",
                keyValue: new Guid("2be2d17f-f954-4396-a169-0efe0f58c43d"));

            migrationBuilder.DeleteData(
                table: "Orders",
                keyColumn: "OrderID",
                keyValue: new Guid("32276d6e-9c7d-46a9-ad2f-3f41b72639a0"));

            migrationBuilder.DeleteData(
                table: "Orders",
                keyColumn: "OrderID",
                keyValue: new Guid("3627fcd5-dfce-4967-9372-f084648cc02b"));

            migrationBuilder.DeleteData(
                table: "Orders",
                keyColumn: "OrderID",
                keyValue: new Guid("3f5b28dc-e9a6-4838-84c5-8c4a61e09266"));

            migrationBuilder.DeleteData(
                table: "Orders",
                keyColumn: "OrderID",
                keyValue: new Guid("45de5b74-7437-4bef-a4dd-239a861b4d91"));

            migrationBuilder.DeleteData(
                table: "Orders",
                keyColumn: "OrderID",
                keyValue: new Guid("58c318c5-696c-4008-9f0e-e362d8b04403"));

            migrationBuilder.DeleteData(
                table: "Orders",
                keyColumn: "OrderID",
                keyValue: new Guid("64cce443-8d08-4ca8-8874-14b50097dcfb"));

            migrationBuilder.DeleteData(
                table: "Orders",
                keyColumn: "OrderID",
                keyValue: new Guid("74268bf2-b71b-4794-afd3-928ab2afc6fe"));

            migrationBuilder.DeleteData(
                table: "Orders",
                keyColumn: "OrderID",
                keyValue: new Guid("756e41df-edf9-417d-9586-52cd39fd6649"));

            migrationBuilder.DeleteData(
                table: "Orders",
                keyColumn: "OrderID",
                keyValue: new Guid("838aad0d-d377-46fc-8425-e00bae35680f"));

            migrationBuilder.DeleteData(
                table: "Orders",
                keyColumn: "OrderID",
                keyValue: new Guid("881b936f-5d52-46f1-9df1-0d6a83e6f412"));

            migrationBuilder.DeleteData(
                table: "Orders",
                keyColumn: "OrderID",
                keyValue: new Guid("946d7fb1-7b00-4aed-a55b-47f4776f3a87"));

            migrationBuilder.DeleteData(
                table: "Orders",
                keyColumn: "OrderID",
                keyValue: new Guid("9790ca1e-e41a-467b-a237-1d4041e07692"));

            migrationBuilder.DeleteData(
                table: "Orders",
                keyColumn: "OrderID",
                keyValue: new Guid("9b2f9565-ca15-438a-99c6-6ab02173b96e"));

            migrationBuilder.DeleteData(
                table: "Orders",
                keyColumn: "OrderID",
                keyValue: new Guid("a6442421-97fc-4637-b743-6599840ecdf5"));

            migrationBuilder.DeleteData(
                table: "Orders",
                keyColumn: "OrderID",
                keyValue: new Guid("cf0bb218-93c0-49f4-b0b0-6fbcf31387ce"));

            migrationBuilder.DeleteData(
                table: "Orders",
                keyColumn: "OrderID",
                keyValue: new Guid("ee541fb1-c5a1-464e-b33e-5d639454c4b1"));

            migrationBuilder.DeleteData(
                table: "Payments",
                keyColumn: "PaymentID",
                keyValue: new Guid("824546f6-9d10-45eb-b902-6e07d43f5cb8"));

            migrationBuilder.DeleteData(
                table: "TrucksCourses",
                keyColumn: "TruckCourseID",
                keyValue: new Guid("0399dfb7-4de1-4ab5-8db2-d45cb6f0932f"));

            migrationBuilder.DeleteData(
                table: "TrucksCourses",
                keyColumn: "TruckCourseID",
                keyValue: new Guid("0b832a33-ced7-4023-99e7-cef1e086b753"));

            migrationBuilder.DeleteData(
                table: "TrucksCourses",
                keyColumn: "TruckCourseID",
                keyValue: new Guid("1c9204d6-e8cb-4929-a926-d55206215421"));

            migrationBuilder.DeleteData(
                table: "TrucksCourses",
                keyColumn: "TruckCourseID",
                keyValue: new Guid("2ce8d831-bf38-48d6-878d-5951acb9c165"));

            migrationBuilder.DeleteData(
                table: "TrucksCourses",
                keyColumn: "TruckCourseID",
                keyValue: new Guid("2f671305-cd8f-40fa-a9c3-4d77525b6316"));

            migrationBuilder.DeleteData(
                table: "TrucksCourses",
                keyColumn: "TruckCourseID",
                keyValue: new Guid("3bd2400a-914e-4c52-9bed-c72408668e06"));

            migrationBuilder.DeleteData(
                table: "TrucksCourses",
                keyColumn: "TruckCourseID",
                keyValue: new Guid("4c9ad6ca-b469-4a5b-b79b-114116bb1780"));

            migrationBuilder.DeleteData(
                table: "TrucksCourses",
                keyColumn: "TruckCourseID",
                keyValue: new Guid("5201ffcb-3020-4f81-9621-8e6495a75e68"));

            migrationBuilder.DeleteData(
                table: "TrucksCourses",
                keyColumn: "TruckCourseID",
                keyValue: new Guid("5d8e5934-4800-4aca-b678-b859fcbf2c1b"));

            migrationBuilder.DeleteData(
                table: "TrucksCourses",
                keyColumn: "TruckCourseID",
                keyValue: new Guid("6a8d56fc-d2b4-4aa0-9ea3-e460b72c9317"));

            migrationBuilder.DeleteData(
                table: "TrucksCourses",
                keyColumn: "TruckCourseID",
                keyValue: new Guid("86e50922-8bfe-442a-9eef-5e0c5c9c45e0"));

            migrationBuilder.DeleteData(
                table: "TrucksCourses",
                keyColumn: "TruckCourseID",
                keyValue: new Guid("92077f74-61b3-4893-86f9-8f7026c952a0"));

            migrationBuilder.DeleteData(
                table: "TrucksCourses",
                keyColumn: "TruckCourseID",
                keyValue: new Guid("c9510b2e-7cfd-47fd-b403-9cd1c61fc6af"));

            migrationBuilder.DeleteData(
                table: "TrucksCourses",
                keyColumn: "TruckCourseID",
                keyValue: new Guid("ccdab565-ec35-4ee0-9038-89737a44882b"));

            migrationBuilder.DeleteData(
                table: "TrucksCourses",
                keyColumn: "TruckCourseID",
                keyValue: new Guid("de561ca6-88c1-47cd-ae21-994d103e91ab"));

            migrationBuilder.DeleteData(
                table: "TrucksCourses",
                keyColumn: "TruckCourseID",
                keyValue: new Guid("ff91895b-3124-4f22-9198-989aaa34a799"));

            migrationBuilder.DeleteData(
                table: "TrucksSpendings",
                keyColumn: "TruckSpendingID",
                keyValue: new Guid("17dcfc1c-e08e-428a-8bf0-bb1230a5d5cb"));

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("583560aa-73c7-4c08-9e32-958581be98eb"));

            migrationBuilder.DeleteData(
                table: "Orders",
                keyColumn: "OrderID",
                keyValue: new Guid("197325b6-f955-44d9-82a3-d7432a5d77d4"));

            migrationBuilder.DeleteData(
                table: "Orders",
                keyColumn: "OrderID",
                keyValue: new Guid("24b95dc1-69d4-4ba9-b91e-d0b691ad729c"));

            migrationBuilder.DeleteData(
                table: "Trucks",
                keyColumn: "TruckID",
                keyValue: new Guid("0cada001-5206-4aff-a0e5-4600bcfed88f"));

            migrationBuilder.DeleteData(
                table: "Trucks",
                keyColumn: "TruckID",
                keyValue: new Guid("24095c26-af58-43fa-9a87-6443d4ad469d"));

            migrationBuilder.DeleteData(
                table: "Trucks",
                keyColumn: "TruckID",
                keyValue: new Guid("d578effb-04cd-4c11-817d-a9ac580ea8be"));

            migrationBuilder.DeleteData(
                table: "Trucks",
                keyColumn: "TruckID",
                keyValue: new Guid("e83b600b-b9a9-4f36-a4ce-3f4f002a2d1c"));

            migrationBuilder.DeleteData(
                table: "TrucksCourses",
                keyColumn: "TruckCourseID",
                keyValue: new Guid("29b586bf-5f04-4882-9cdf-5946ea3ecec3"));

            migrationBuilder.DeleteData(
                table: "TrucksCourses",
                keyColumn: "TruckCourseID",
                keyValue: new Guid("dbd66272-bee6-4114-a386-33090c0ba172"));

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("c835750b-0e0e-4fb0-88b6-a4b557e52c21"));

            migrationBuilder.DeleteData(
                table: "Trucks",
                keyColumn: "TruckID",
                keyValue: new Guid("75c3da6c-8cc5-4bbc-b365-80c24b0216fc"));

            migrationBuilder.DeleteData(
                table: "Trucks",
                keyColumn: "TruckID",
                keyValue: new Guid("b6858daf-c51a-4a70-8246-6fc477d53111"));

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("051eafea-deaa-4fbe-b442-25b7be18fa8e"),
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "df2b7fe5-0158-4cdf-8eec-c1f660590919", "AQAAAAIAAYagAAAAEL9Ps09TT5d/dtrX80yK6H/KdC0Kh9Rr+tFJtAvD2MrWfoSrwAMJ/jJqYYa1GXJwlQ==", "77560b6c-d614-451f-8b10-aaa59d40108e" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("862c30cd-17ee-4557-8222-c926bebf0a66"),
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "6edb66da-be0a-4a50-baf5-ca406103a6b4", "AQAAAAIAAYagAAAAEC+0D9crqWZFGHRJK+X/2FSbmoWYbBzxZ6t0shCx3/3ALqljPRuCq9yw0U4gkWuh8A==", "24472bcf-990a-49bf-b2e5-343897fe1c68" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("922b605c-25eb-4ded-b2a7-7966b38b8685"),
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "9d3d41db-b3d8-4e59-8a85-6c125ab6c6cf", "AQAAAAIAAYagAAAAEHVYKQgsgG5GJLVN5GvdLD1kD2Vkx58fLxzi9jdmMH3wIbH4UsvW9+t3E4bfAXoVTQ==", "66545229-a1b8-408d-afd9-ce9ce3edc048" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("cca021da-da51-4bd7-b968-8294cf006dfd"),
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "1a645d87-508f-4081-89ca-e62d845060c8", "AQAAAAIAAYagAAAAELc41rT3Vb6VnyQjCoTyd5iD6QA3wKspGtImItv6xC3ZdXKAOvBWj6ahbUytwMjo1A==", "6aeebefd-d08b-4ade-8a87-91ebeba082f1" });
        }
    }
}
