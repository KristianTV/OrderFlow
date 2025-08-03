namespace OrderFlow.GCommon
{
    public class ValidationConstants
    {
        public static string DateFormat = "dd/MM/yyyy";

        public static class Order
        {
            public const int DeliveryInstructionsMaxLength = 500;
            public const int DeliveryInstructionsMinLength = 1;

            public const int DeliveryAddressMaxLength = 250;
            public const int DeliveryAddressMinLength = 3;
            public const int PickupAddressMaxLength = 250;
            public const int PickupAddressMinLength = 3;

            public const int LoadCapacityMaxLength = int.MaxValue;
            public const int LoadCapacityMinLength = 1;
        }

        public static class Notification
        {
            public static int TitleMaxLenght = 100;
            public static int MessageMaxLength = 500;
        }

        public static class Truck
        {
            public static int LicensePlateMaxLength = 20;
        }

        public static class TruckOrder
        {
            public static int DeliverAddressMaxLength = 250;
        }

        public static class Payment
        {
            public static int PaymentDescriptionMaxLength = 100;
        }
    }
}
