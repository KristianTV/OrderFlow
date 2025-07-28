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
    }
}
