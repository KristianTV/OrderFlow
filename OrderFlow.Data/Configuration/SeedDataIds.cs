using System.Security.Cryptography;
using System.Text;

namespace OrderFlow.Data.Configuration
{
    internal static class SeedDataIds
    {
        internal static readonly Guid AdminRoleId = Guid.Parse("b91c3da6-5bde-478d-bf2c-257b2fc9567c");
        internal static readonly Guid SpeditorRoleId = Guid.Parse("206f9e28-522f-4d34-8bfd-ec37b093a331");
        internal static readonly Guid DriverRoleId = Guid.Parse("d349122c-ed4a-43ca-b939-b3a9ff9fa0fb");
        internal static readonly Guid UserRoleId = Guid.Parse("782b39c1-d0e7-4f18-8a2b-1077a438c048");

        internal static readonly Guid AdminUserId = Guid.Parse("922b605c-25eb-4ded-b2a7-7966b38b8685");
        internal static readonly Guid SpeditorUserId = Guid.Parse("051eafea-deaa-4fbe-b442-25b7be18fa8e");
        internal static readonly Guid DriverUserId = Guid.Parse("cca021da-da51-4bd7-b968-8294cf006dfd");
        internal static readonly Guid RegularUserId = Guid.Parse("862c30cd-17ee-4557-8222-c926bebf0a66");
        internal static readonly Guid CompanyUserId = Guid.Parse("583560aa-73c7-4c08-9e32-958581be98eb");
        internal static readonly Guid DriverTwoUserId = Guid.Parse("c835750b-0e0e-4fb0-88b6-a4b557e52c21");

        internal static readonly Guid[] OrderIds = CreateIds("orders", 20);
        internal static readonly Guid[] PaymentIds = CreateIds("payments", 80);
        internal static readonly Guid[] TruckIds = CreateIds("trucks", 6);
        internal static readonly Guid[] CourseIds = CreateIds("courses", 18);
        internal static readonly Guid[] TruckSpendingIds = CreateIds("truck-spendings", 18);
        internal static readonly Guid[] NotificationIds = CreateIds("notifications", 12);
        internal static readonly Guid[] MessageIds = CreateIds("messages", 6);

        internal static readonly string[] PickupAddresses =
        {
            "Sofia, 100 Tsarigradsko Shose Blvd.",
            "Sofia, 15 Botevgradsko Shose Blvd.",
            "Plovdiv, 12 Vasil Aprilov Blvd.",
            "Plovdiv, 8 Kuklensko Shose Blvd.",
            "Varna, 24 Devnya Industrial Zone",
            "Varna, 18 Vladislav Varnenchik Blvd.",
            "Burgas, 6 Transportna St.",
            "Burgas, 42 Todor Aleksandrov Blvd.",
            "Ruse, 5 Danube Str.",
            "Ruse, 17 Lipnik Blvd.",
            "Stara Zagora, 31 Patriarch Evtimiy Blvd.",
            "Stara Zagora, 9 Industrialna St.",
            "Pleven, 14 Bulgaria Blvd.",
            "Pleven, 27 Ruse Blvd.",
            "Veliko Tarnovo, 11 Magistralna St.",
            "Veliko Tarnovo, 44 Nikola Gabrovski Blvd.",
            "Blagoevgrad, 20 Sveti Dimitar Solunski Blvd.",
            "Shumen, 7 Simeon Veliki Blvd.",
            "Dobrich, 19 Dobruja Blvd.",
            "Sliven, 33 Bratya Miladinovi St."
        };

        internal static readonly string[] DeliveryAddresses =
        {
            "Plovdiv, 25 Hristo Botev Blvd.",
            "Pazardzhik, 10 Bulgaria Blvd.",
            "Varna, 55 Slivnitsa Blvd.",
            "Dobrich, 3 25-ti Septemvri Blvd.",
            "Burgas, 21 Stefan Stambolov Blvd.",
            "Yambol, 16 Graf Ignatiev St.",
            "Ruse, 40 Bulgaria Blvd.",
            "Razgrad, 8 Aprilsko Vastanie Blvd.",
            "Pleven, 29 Danail Popov St.",
            "Lovech, 12 Bulgaria Blvd.",
            "Veliko Tarnovo, 30 Bulgaria Blvd.",
            "Gabrovo, 6 Aprilov Blvd.",
            "Blagoevgrad, 14 Vasil Levski Blvd.",
            "Pernik, 22 Yug Gagarin St.",
            "Shumen, 45 Rishki Prohod Blvd.",
            "Targovishte, 9 Mimi Balkanska St.",
            "Sofia, 88 Bulgaria Blvd.",
            "Montana, 13 Treti Mart Blvd.",
            "Haskovo, 26 Saedinenie Blvd.",
            "Kardzhali, 5 Bulgaria Blvd."
        };

        internal const int AssignedOrdersCount = 16;

        internal static int? GetCourseIndexForOrder(int orderIndex)
            => orderIndex < AssignedOrdersCount ? orderIndex / 2 : null;

        internal static bool IsCourseDelivered(int courseIndex)
            => courseIndex < AssignedOrdersCount / 2 && courseIndex % 2 == 1;

        private static Guid[] CreateIds(string entityName, int count)
            => Enumerable.Range(1, count)
                .Select(number => CreateDeterministicGuid($"{entityName}:{number}"))
                .ToArray();

        private static Guid CreateDeterministicGuid(string value)
        {
            byte[] hash = SHA256.HashData(Encoding.UTF8.GetBytes($"OrderFlow:{value}"));
            byte[] guidBytes = hash[..16];

            guidBytes[7] = (byte)((guidBytes[7] & 0x0F) | 0x40);
            guidBytes[8] = (byte)((guidBytes[8] & 0x3F) | 0x80);

            return new Guid(guidBytes);
        }
    }
}
