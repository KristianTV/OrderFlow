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
            public const int TitleMaxLenght = 100;
            public const int TitleMinLenght = 1;
            public const int MessageMaxLength = 500;
            public const int MessageMinLength = 5;
        }

        public static class Truck
        {
            public const int LicensePlateMaxLength = 20;
            public const int LicensePlateMinLength = 8;

            public const int CapacityMaxLength = int.MaxValue;
            public const int CapacityMinLength = 1;
        }

        public static class TruckOrder
        {
            public const int DeliverAddressMaxLength = 250;
            public const int DeliverAddressMinLength = 250;
        }

        public static class Payment
        {
            public const int PaymentDescriptionMaxLength = 100;
            public const int PaymentDescriptionMinLength = 1;
        }
    }
}
