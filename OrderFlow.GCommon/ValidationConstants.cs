namespace OrderFlow.GCommon
{
    public class ValidationConstants
    {
        public static string DateFormat = "dd/MM/yyyy";

        public static class Order
        {
            public static int DeliveryInstructionsMaxLength = 500;
            public static int StatusMaxLength = 50;

            public static int DeliveryAddressMaxLength = 250;
            public static int PickupAddressMaxLength = 250;
        }

        public static class Notification
        {
            public static int TitleMaxLenght = 100;
            public static int MessageMaxLength = 500;
        }

        public static class Truck
        {
            public static int LicensePlateMaxLength = 20;
            public static int StatusMaxLength = 50;
        }

        public static class Payment
        {
            public static int PaymentDescriptionMaxLength = 100;
        }
    }
}
