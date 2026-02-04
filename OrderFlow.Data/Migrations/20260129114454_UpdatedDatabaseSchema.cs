using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace OrderFlow.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedDatabaseSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Notifications_AspNetUsers_ReceiverId",
                table: "Notifications");

            migrationBuilder.DropForeignKey(
                name: "FK_Notifications_AspNetUsers_SenderId",
                table: "Notifications");

            migrationBuilder.DropForeignKey(
                name: "FK_Notifications_Orders_OrderId",
                table: "Notifications");

            migrationBuilder.DropForeignKey(
                name: "FK_Notifications_Trucks_TruckId",
                table: "Notifications");

            migrationBuilder.DropTable(
                name: "TrucksOrders");

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

            migrationBuilder.RenameColumn(
                name: "isDeleted",
                table: "Trucks",
                newName: "IsDeleted");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Payments",
                newName: "PaymentID");

            migrationBuilder.RenameColumn(
                name: "isCanceled",
                table: "Orders",
                newName: "IsCanceled");

            migrationBuilder.RenameColumn(
                name: "TruckId",
                table: "Notifications",
                newName: "TruckID");

            migrationBuilder.RenameColumn(
                name: "SenderId",
                table: "Notifications",
                newName: "SenderID");

            migrationBuilder.RenameColumn(
                name: "ReceiverId",
                table: "Notifications",
                newName: "ReceiverID");

            migrationBuilder.RenameColumn(
                name: "OrderId",
                table: "Notifications",
                newName: "OrderID");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Notifications",
                newName: "NotificationID");

            migrationBuilder.RenameIndex(
                name: "IX_Notifications_TruckId",
                table: "Notifications",
                newName: "IX_Notifications_TruckID");

            migrationBuilder.RenameIndex(
                name: "IX_Notifications_SenderId",
                table: "Notifications",
                newName: "IX_Notifications_SenderID");

            migrationBuilder.RenameIndex(
                name: "IX_Notifications_ReceiverId",
                table: "Notifications",
                newName: "IX_Notifications_ReceiverID");

            migrationBuilder.RenameIndex(
                name: "IX_Notifications_OrderId",
                table: "Notifications",
                newName: "IX_Notifications_OrderID");

            migrationBuilder.AlterColumn<double>(
                name: "Capacity",
                table: "Trucks",
                type: "float",
                nullable: false,
                defaultValue: 0.0,
                oldClrType: typeof(int),
                oldType: "int",
                oldDefaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PaymentMethod",
                table: "Payments",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<double>(
                name: "LoadCapacity",
                table: "Orders",
                type: "float",
                nullable: false,
                defaultValue: 0.0,
                oldClrType: typeof(int),
                oldType: "int",
                oldDefaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "CanRespond",
                table: "Notifications",
                type: "bit",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CourseID",
                table: "Notifications",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "PaymentID",
                table: "Notifications",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "TruckSpendingID",
                table: "Notifications",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "AccountType",
                table: "AspNetUsers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "CompanyProfiles",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CompanyName = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    VATNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CompanyAdress = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    ContactPersonName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ContactPhone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompanyProfiles", x => x.UserId);
                    table.ForeignKey(
                        name: "FK_CompanyProfiles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Messages",
                columns: table => new
                {
                    MessageID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SenderID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ReceiverID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NotificationID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Content = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    SentAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsRead = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Messages", x => x.MessageID);
                    table.ForeignKey(
                        name: "FK_Messages_AspNetUsers_ReceiverID",
                        column: x => x.ReceiverID,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Messages_AspNetUsers_SenderID",
                        column: x => x.SenderID,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Messages_Notifications_NotificationID",
                        column: x => x.NotificationID,
                        principalTable: "Notifications",
                        principalColumn: "NotificationID");
                });

            migrationBuilder.CreateTable(
                name: "PersonalProfiles",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    PersonalNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Adress = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    DateOfBirth = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PersonalProfiles", x => x.UserId);
                    table.ForeignKey(
                        name: "FK_PersonalProfiles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TrucksCourses",
                columns: table => new
                {
                    TruckCourseID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TruckID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    PickupAddress = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    DeliverAddress = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    AssignedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DeliveryDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Income = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrucksCourses", x => x.TruckCourseID);
                    table.ForeignKey(
                        name: "FK_TrucksCourses_Trucks_TruckID",
                        column: x => x.TruckID,
                        principalTable: "Trucks",
                        principalColumn: "TruckID",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "CoursesOrders",
                columns: table => new
                {
                    OrderID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TruckCourseID = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CoursesOrders", x => new { x.OrderID, x.TruckCourseID });
                    table.ForeignKey(
                        name: "FK_CoursesOrders_Orders_OrderID",
                        column: x => x.OrderID,
                        principalTable: "Orders",
                        principalColumn: "OrderID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CoursesOrders_TrucksCourses_TruckCourseID",
                        column: x => x.TruckCourseID,
                        principalTable: "TrucksCourses",
                        principalColumn: "TruckCourseID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TrucksSpendings",
                columns: table => new
                {
                    TruckSpendingID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TruckID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    TruckCourseID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    PaymentDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PaymentDescription = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    PaymentMethod = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrucksSpendings", x => x.TruckSpendingID);
                    table.ForeignKey(
                        name: "FK_TrucksSpendings_TrucksCourses_TruckCourseID",
                        column: x => x.TruckCourseID,
                        principalTable: "TrucksCourses",
                        principalColumn: "TruckCourseID",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_TrucksSpendings_Trucks_TruckID",
                        column: x => x.TruckID,
                        principalTable: "Trucks",
                        principalColumn: "TruckID",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { new Guid("7bd7d8f7-69ee-43c1-bceb-bf08a321c2fb"), null, "Admin", "ADMIN" },
                    { new Guid("949d1d77-0b2d-4548-a45f-a6799d95de95"), null, "User", "USER" },
                    { new Guid("9568c789-205f-41eb-a825-a5c47c9acf9b"), null, "Speditor", "SPEDITOR" },
                    { new Guid("c5ecaffb-f14d-47a4-887c-b3665593be1f"), null, "Driver", "DRIVER" }
                });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "AccountType", "ConcurrencyStamp", "Email", "EmailConfirmed", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[,]
                {
                    { new Guid("049c57b3-926d-4622-a23f-2355bd408a62"), 0, 1, "58a0189e-f0d9-4f78-8848-e966b67c0c61", "user@gmail.com", true, true, null, "USER@GMAIL.COM", "USER", "AQAAAAIAAYagAAAAEEkluOoWLgVrhnb+HnXiZZc1qwEmvx2MMNuK6YpBjzxBoymHsO2ZNknWsn7FioXfhw==", "1234567890", true, "0c8a3017-3fa1-42ed-b604-01c22dc8774a", false, "User" },
                    { new Guid("86943e13-b22d-4e32-93f3-1734738dc270"), 0, 1, "f248cbf5-c48f-4954-ab86-dd6dbc88ac55", "driver@gmail.com", true, true, null, "DRIVER@GMAIL.COM", "DRIVER", "AQAAAAIAAYagAAAAEBH9FB081cGCS+GOo2uPZ+3lS/q15j9PFTzeE+FRgylButa8rGLXbR33PccJ/V2Ujg==", "1234567890", true, "e7a4c841-64e3-4b9a-9c18-4d48469277f7", false, "Driver" },
                    { new Guid("9383ace3-1c2a-4e5c-9f3d-adfd376514bf"), 0, 1, "fff2b58b-9654-4f28-90b5-f21ac927cfbe", "speditor@gmail.com", true, true, null, "SPEDITOR@GMAIL.COM", "SPEDITOR", "AQAAAAIAAYagAAAAEHPwAf7q+6vHFxkmCOtPIi57tRyu52DWM4++3wENR1omUtUN5A8gP5Nse212FXa7rA==", "1234567890", true, "7ed82f2b-857e-4cc5-ae2f-018ea2656574", false, "Speditor" },
                    { new Guid("ecbe23b5-c252-4ad9-a31a-b99b60c7fcf4"), 0, 1, "534b6f12-39e3-478a-af38-def9af7925c0", "admin@gmail.com", true, true, null, "ADMIN@GMAIL.COM", "ADMIN", "AQAAAAIAAYagAAAAEFfXb6yif81GA5je57utlX99hAJakT250b4kDHcCyE5XuAZaY+fofCk39maTr1pLSg==", "1234567890", true, "b9fd6caf-8441-4282-9a13-1c48140a35fb", false, "Admin" }
                });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[,]
                {
                    { new Guid("949d1d77-0b2d-4548-a45f-a6799d95de95"), new Guid("049c57b3-926d-4622-a23f-2355bd408a62") },
                    { new Guid("c5ecaffb-f14d-47a4-887c-b3665593be1f"), new Guid("86943e13-b22d-4e32-93f3-1734738dc270") },
                    { new Guid("9568c789-205f-41eb-a825-a5c47c9acf9b"), new Guid("9383ace3-1c2a-4e5c-9f3d-adfd376514bf") },
                    { new Guid("7bd7d8f7-69ee-43c1-bceb-bf08a321c2fb"), new Guid("ecbe23b5-c252-4ad9-a31a-b99b60c7fcf4") }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_CourseID",
                table: "Notifications",
                column: "CourseID");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_PaymentID",
                table: "Notifications",
                column: "PaymentID");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_TruckSpendingID",
                table: "Notifications",
                column: "TruckSpendingID");

            migrationBuilder.CreateIndex(
                name: "IX_CoursesOrders_TruckCourseID",
                table: "CoursesOrders",
                column: "TruckCourseID");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_NotificationID",
                table: "Messages",
                column: "NotificationID");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_ReceiverID",
                table: "Messages",
                column: "ReceiverID");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_SenderID",
                table: "Messages",
                column: "SenderID");

            migrationBuilder.CreateIndex(
                name: "IX_TrucksCourses_TruckID",
                table: "TrucksCourses",
                column: "TruckID");

            migrationBuilder.CreateIndex(
                name: "IX_TrucksSpendings_TruckCourseID",
                table: "TrucksSpendings",
                column: "TruckCourseID");

            migrationBuilder.CreateIndex(
                name: "IX_TrucksSpendings_TruckID",
                table: "TrucksSpendings",
                column: "TruckID");

            migrationBuilder.AddForeignKey(
                name: "FK_Notifications_AspNetUsers_ReceiverID",
                table: "Notifications",
                column: "ReceiverID",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Notifications_AspNetUsers_SenderID",
                table: "Notifications",
                column: "SenderID",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Notifications_Orders_OrderID",
                table: "Notifications",
                column: "OrderID",
                principalTable: "Orders",
                principalColumn: "OrderID");

            migrationBuilder.AddForeignKey(
                name: "FK_Notifications_Payments_PaymentID",
                table: "Notifications",
                column: "PaymentID",
                principalTable: "Payments",
                principalColumn: "PaymentID");

            migrationBuilder.AddForeignKey(
                name: "FK_Notifications_TrucksCourses_CourseID",
                table: "Notifications",
                column: "CourseID",
                principalTable: "TrucksCourses",
                principalColumn: "TruckCourseID",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Notifications_TrucksSpendings_TruckSpendingID",
                table: "Notifications",
                column: "TruckSpendingID",
                principalTable: "TrucksSpendings",
                principalColumn: "TruckSpendingID",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Notifications_Trucks_TruckID",
                table: "Notifications",
                column: "TruckID",
                principalTable: "Trucks",
                principalColumn: "TruckID",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Notifications_AspNetUsers_ReceiverID",
                table: "Notifications");

            migrationBuilder.DropForeignKey(
                name: "FK_Notifications_AspNetUsers_SenderID",
                table: "Notifications");

            migrationBuilder.DropForeignKey(
                name: "FK_Notifications_Orders_OrderID",
                table: "Notifications");

            migrationBuilder.DropForeignKey(
                name: "FK_Notifications_Payments_PaymentID",
                table: "Notifications");

            migrationBuilder.DropForeignKey(
                name: "FK_Notifications_TrucksCourses_CourseID",
                table: "Notifications");

            migrationBuilder.DropForeignKey(
                name: "FK_Notifications_TrucksSpendings_TruckSpendingID",
                table: "Notifications");

            migrationBuilder.DropForeignKey(
                name: "FK_Notifications_Trucks_TruckID",
                table: "Notifications");

            migrationBuilder.DropTable(
                name: "CompanyProfiles");

            migrationBuilder.DropTable(
                name: "CoursesOrders");

            migrationBuilder.DropTable(
                name: "Messages");

            migrationBuilder.DropTable(
                name: "PersonalProfiles");

            migrationBuilder.DropTable(
                name: "TrucksSpendings");

            migrationBuilder.DropTable(
                name: "TrucksCourses");

            migrationBuilder.DropIndex(
                name: "IX_Notifications_CourseID",
                table: "Notifications");

            migrationBuilder.DropIndex(
                name: "IX_Notifications_PaymentID",
                table: "Notifications");

            migrationBuilder.DropIndex(
                name: "IX_Notifications_TruckSpendingID",
                table: "Notifications");

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { new Guid("949d1d77-0b2d-4548-a45f-a6799d95de95"), new Guid("049c57b3-926d-4622-a23f-2355bd408a62") });

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { new Guid("c5ecaffb-f14d-47a4-887c-b3665593be1f"), new Guid("86943e13-b22d-4e32-93f3-1734738dc270") });

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { new Guid("9568c789-205f-41eb-a825-a5c47c9acf9b"), new Guid("9383ace3-1c2a-4e5c-9f3d-adfd376514bf") });

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { new Guid("7bd7d8f7-69ee-43c1-bceb-bf08a321c2fb"), new Guid("ecbe23b5-c252-4ad9-a31a-b99b60c7fcf4") });

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("7bd7d8f7-69ee-43c1-bceb-bf08a321c2fb"));

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("949d1d77-0b2d-4548-a45f-a6799d95de95"));

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("9568c789-205f-41eb-a825-a5c47c9acf9b"));

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("c5ecaffb-f14d-47a4-887c-b3665593be1f"));

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("049c57b3-926d-4622-a23f-2355bd408a62"));

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("86943e13-b22d-4e32-93f3-1734738dc270"));

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("9383ace3-1c2a-4e5c-9f3d-adfd376514bf"));

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("ecbe23b5-c252-4ad9-a31a-b99b60c7fcf4"));

            migrationBuilder.DropColumn(
                name: "PaymentMethod",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "CanRespond",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "CourseID",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "PaymentID",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "TruckSpendingID",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "AccountType",
                table: "AspNetUsers");

            migrationBuilder.RenameColumn(
                name: "IsDeleted",
                table: "Trucks",
                newName: "isDeleted");

            migrationBuilder.RenameColumn(
                name: "PaymentID",
                table: "Payments",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "IsCanceled",
                table: "Orders",
                newName: "isCanceled");

            migrationBuilder.RenameColumn(
                name: "TruckID",
                table: "Notifications",
                newName: "TruckId");

            migrationBuilder.RenameColumn(
                name: "SenderID",
                table: "Notifications",
                newName: "SenderId");

            migrationBuilder.RenameColumn(
                name: "ReceiverID",
                table: "Notifications",
                newName: "ReceiverId");

            migrationBuilder.RenameColumn(
                name: "OrderID",
                table: "Notifications",
                newName: "OrderId");

            migrationBuilder.RenameColumn(
                name: "NotificationID",
                table: "Notifications",
                newName: "Id");

            migrationBuilder.RenameIndex(
                name: "IX_Notifications_TruckID",
                table: "Notifications",
                newName: "IX_Notifications_TruckId");

            migrationBuilder.RenameIndex(
                name: "IX_Notifications_SenderID",
                table: "Notifications",
                newName: "IX_Notifications_SenderId");

            migrationBuilder.RenameIndex(
                name: "IX_Notifications_ReceiverID",
                table: "Notifications",
                newName: "IX_Notifications_ReceiverId");

            migrationBuilder.RenameIndex(
                name: "IX_Notifications_OrderID",
                table: "Notifications",
                newName: "IX_Notifications_OrderId");

            migrationBuilder.AlterColumn<int>(
                name: "Capacity",
                table: "Trucks",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(double),
                oldType: "float",
                oldDefaultValue: 0.0);

            migrationBuilder.AlterColumn<int>(
                name: "LoadCapacity",
                table: "Orders",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(double),
                oldType: "float",
                oldDefaultValue: 0.0);

            migrationBuilder.CreateTable(
                name: "TrucksOrders",
                columns: table => new
                {
                    TruckOrderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrderID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TruckID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AssignedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DeliverAddress = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    DeliveryDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrucksOrders", x => x.TruckOrderId);
                    table.ForeignKey(
                        name: "FK_TrucksOrders_Orders_OrderID",
                        column: x => x.OrderID,
                        principalTable: "Orders",
                        principalColumn: "OrderID");
                    table.ForeignKey(
                        name: "FK_TrucksOrders_Trucks_TruckID",
                        column: x => x.TruckID,
                        principalTable: "Trucks",
                        principalColumn: "TruckID");
                });

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

            migrationBuilder.CreateIndex(
                name: "IX_TrucksOrders_TruckID",
                table: "TrucksOrders",
                column: "TruckID");

            migrationBuilder.AddForeignKey(
                name: "FK_Notifications_AspNetUsers_ReceiverId",
                table: "Notifications",
                column: "ReceiverId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Notifications_AspNetUsers_SenderId",
                table: "Notifications",
                column: "SenderId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Notifications_Orders_OrderId",
                table: "Notifications",
                column: "OrderId",
                principalTable: "Orders",
                principalColumn: "OrderID");

            migrationBuilder.AddForeignKey(
                name: "FK_Notifications_Trucks_TruckId",
                table: "Notifications",
                column: "TruckId",
                principalTable: "Trucks",
                principalColumn: "TruckID");
        }
    }
}
