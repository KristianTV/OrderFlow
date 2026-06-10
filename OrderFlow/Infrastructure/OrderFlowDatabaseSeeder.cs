using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OrderFlow.Data;
using OrderFlow.Data.Models;
using OrderFlow.Data.Models.Enums;

namespace OrderFlow.Infrastructure
{
    public static class OrderFlowDatabaseSeeder
    {
        private const string DemoPassword = "Password123!";

        public static async Task SeedAsync(IServiceProvider services)
        {
            using var scope = services.CreateScope();

            var context = scope.ServiceProvider.GetRequiredService<OrderFlowDbContext>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();

            await context.Database.MigrateAsync();

            await EnsureRoleAsync(roleManager, "Admin");
            await EnsureRoleAsync(roleManager, "Speditor");
            await EnsureRoleAsync(roleManager, "Driver");
            await EnsureRoleAsync(roleManager, "User");

            await EnsureDemoUserAsync(
                userManager,
                userName: "CompanyUser",
                email: "company.user@gmail.com",
                phoneNumber: "0888123456",
                accountType: AccountType.Company,
                roleName: "User");

            await EnsureDemoUserAsync(
                userManager,
                userName: "DriverTwo",
                email: "driver.two@gmail.com",
                phoneNumber: "0888654321",
                accountType: AccountType.Personal,
                roleName: "Driver");

            await EnsureProfilesAsync(context);
            await EnsureMinimumOrdersForEveryUserAsync(context);
            await EnsureMinimumTrucksAndCoursesForEveryDriverAsync(context, userManager);
        }

        private static async Task EnsureRoleAsync(RoleManager<IdentityRole<Guid>> roleManager, string roleName)
        {
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                await roleManager.CreateAsync(new IdentityRole<Guid>(roleName));
            }
        }

        private static async Task EnsureDemoUserAsync(
            UserManager<ApplicationUser> userManager,
            string userName,
            string email,
            string phoneNumber,
            AccountType accountType,
            string roleName)
        {
            var user = await userManager.FindByNameAsync(userName);
            if (user == null)
            {
                user = new ApplicationUser
                {
                    UserName = userName,
                    Email = email,
                    EmailConfirmed = true,
                    PhoneNumber = phoneNumber,
                    PhoneNumberConfirmed = true,
                    AccountType = accountType
                };

                var result = await userManager.CreateAsync(user, DemoPassword);
                if (!result.Succeeded)
                {
                    var errors = string.Join(", ", result.Errors.Select(error => error.Description));
                    throw new InvalidOperationException($"Could not seed user '{userName}': {errors}");
                }
            }

            if (!await userManager.IsInRoleAsync(user, roleName))
            {
                await userManager.AddToRoleAsync(user, roleName);
            }
        }

        private static async Task EnsureProfilesAsync(OrderFlowDbContext context)
        {
            var companyUser = await context.Users.SingleOrDefaultAsync(user => user.NormalizedUserName == "COMPANYUSER");
            if (companyUser != null && !await context.CompanyProfiles.AnyAsync(profile => profile.UserId == companyUser.Id))
            {
                context.CompanyProfiles.Add(new CompanyProfile
                {
                    UserId = companyUser.Id,
                    CompanyName = "Demo Logistics Ltd.",
                    VATNumber = "BG123456789",
                    CompanyAdress = "Sofia, 100 Tsarigradsko Shose Blvd.",
                    ContactPersonName = "Elena Petrova",
                    ContactPhone = companyUser.PhoneNumber ?? "0888123456"
                });
            }

            var driverTwo = await context.Users.SingleOrDefaultAsync(user => user.NormalizedUserName == "DRIVERTWO");
            if (driverTwo != null && !await context.PersonalProfiles.AnyAsync(profile => profile.UserId == driverTwo.Id))
            {
                context.PersonalProfiles.Add(new PersonalProfile
                {
                    UserId = driverTwo.Id,
                    FirstName = "Nikolay",
                    LastName = "Dimitrov",
                    PersonalNumber = "8805050005",
                    Adress = "Ruse, 5 Danube Str.",
                    DateOfBirth = new DateTime(1988, 5, 5)
                });
            }

            await context.SaveChangesAsync();
        }

        private static async Task EnsureMinimumOrdersForEveryUserAsync(OrderFlowDbContext context)
        {
            var users = await context.Users.OrderBy(user => user.UserName).ToListAsync();

            foreach (var user in users)
            {
                var existingOrdersCount = await context.Orders.CountAsync(order => order.UserID == user.Id);
                for (var index = existingOrdersCount; index < 3; index++)
                {
                    var orderNumber = index + 1;
                    var orderDate = new DateTime(2026, 6, 1).AddDays(orderNumber);

                    var order = new Order
                    {
                        UserID = user.Id,
                        OrderDate = orderDate,
                        DeliveryDate = orderDate.AddDays(2),
                        PickupAddress = $"Demo pickup address {orderNumber} for {user.UserName}",
                        DeliveryAddress = $"Demo delivery address {orderNumber} for {user.UserName}",
                        LoadCapacity = 3 + orderNumber,
                        DeliveryInstructions = $"Seeded sample order {orderNumber}.",
                        Status = orderNumber % 3 == 0 ? OrderStatus.Completed : OrderStatus.InProgress,
                        IsCanceled = false
                    };

                    context.Orders.Add(order);
                    context.Payments.Add(new Payment
                    {
                        OrderID = order.OrderID,
                        Amount = 450 + (orderNumber * 125),
                        CreatedOn = orderDate,
                        PaymentDescription = $"Seed payment for order {orderNumber}",
                        PaymentMethod = orderNumber % 2 == 0 ? PaymentMethods.Cash : PaymentMethods.Card,
                        PaymentDate = orderNumber % 3 == 0 ? orderDate.AddHours(1) : null
                    });
                }
            }

            await context.SaveChangesAsync();
        }

        private static async Task EnsureMinimumTrucksAndCoursesForEveryDriverAsync(
            OrderFlowDbContext context,
            UserManager<ApplicationUser> userManager)
        {
            var users = await context.Users.OrderBy(user => user.UserName).ToListAsync();
            var drivers = new List<ApplicationUser>();

            foreach (var user in users)
            {
                if (await userManager.IsInRoleAsync(user, "Driver"))
                {
                    drivers.Add(user);
                }
            }

            foreach (var driver in drivers)
            {
                await EnsureMinimumTrucksForDriverAsync(context, driver);
                await EnsureMinimumCoursesForDriverAsync(context, driver);
            }

            await context.SaveChangesAsync();
        }

        private static async Task EnsureMinimumTrucksForDriverAsync(OrderFlowDbContext context, ApplicationUser driver)
        {
            var existingTrucksCount = await context.Trucks.IgnoreQueryFilters().CountAsync(truck => truck.DriverID == driver.Id);
            for (var index = existingTrucksCount; index < 3; index++)
            {
                var truckNumber = index + 1;

                context.Trucks.Add(new Truck
                {
                    DriverID = driver.Id,
                    LicensePlate = BuildLicensePlate(driver, truckNumber),
                    Capacity = 12 + (truckNumber * 3),
                    Status = truckNumber == 3 ? TruckStatus.UnderMaintenance : TruckStatus.Available,
                    IsDeleted = false
                });
            }

            await context.SaveChangesAsync();
        }

        private static async Task EnsureMinimumCoursesForDriverAsync(OrderFlowDbContext context, ApplicationUser driver)
        {
            var trucks = await context.Trucks
                .Where(truck => truck.DriverID == driver.Id)
                .OrderBy(truck => truck.LicensePlate)
                .ToListAsync();

            for (var truckIndex = 0; truckIndex < trucks.Count; truckIndex++)
            {
                var truck = trucks[truckIndex];
                var existingCoursesCount = await context.TrucksCourses.CountAsync(course => course.TruckID == truck.TruckID);

                for (var courseIndex = existingCoursesCount; courseIndex < 3; courseIndex++)
                {
                    var courseNumber = courseIndex + 1;
                    var assignedDate = new DateTime(2026, 6, 10).AddDays((truckIndex * 3) + courseNumber);
                    var order = await context.Orders
                        .Where(existingOrder => existingOrder.UserID == driver.Id)
                        .OrderBy(existingOrder => existingOrder.OrderDate)
                        .Skip(courseIndex)
                        .FirstOrDefaultAsync();

                    order ??= await context.Orders.OrderBy(existingOrder => existingOrder.OrderDate).FirstAsync();

                    var course = new TruckCourse
                    {
                        TruckID = truck.TruckID,
                        PickupAddress = $"Driver {driver.UserName} truck {truck.LicensePlate} pickup {courseNumber}",
                        DeliverAddress = $"Driver {driver.UserName} truck {truck.LicensePlate} delivery {courseNumber}",
                        AssignedDate = assignedDate,
                        DeliveryDate = courseNumber % 2 == 0 ? assignedDate.AddDays(1) : null,
                        Status = courseNumber % 2 == 0 ? CourseStatus.Delivered : CourseStatus.Assigned,
                        Income = 700 + (courseNumber * 150)
                    };

                    context.TrucksCourses.Add(course);
                    context.CoursesOrders.Add(new CourseOrder
                    {
                        OrderID = order.OrderID,
                        TruckCourseID = course.TruckCourseID
                    });
                    context.TrucksSpendings.Add(new TruckSpending
                    {
                        TruckID = truck.TruckID,
                        TruckCourseID = course.TruckCourseID,
                        Amount = 90 + (courseNumber * 35),
                        PaymentDate = assignedDate,
                        SpendingType = courseNumber % 2 == 0 ? TruckSpendingsType.Tolls : TruckSpendingsType.Fuel,
                        PaymentDescription = $"Seed spending for course {courseNumber}",
                        PaymentMethod = courseNumber % 2 == 0 ? PaymentMethods.Cash : PaymentMethods.Card
                    });
                }
            }
        }

        private static string BuildLicensePlate(ApplicationUser driver, int truckNumber)
        {
            var source = driver.Id.ToString("N")[..6].ToUpperInvariant();
            return $"OF{source[..2]}{truckNumber}{source[2..5]}";
        }
    }
}
