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

            public const double LoadCapacityMaxLength = double.MaxValue;
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

        public static class TruckSpending
        {
            public const int PaymentDescriptionMaxLength = 100;
            public const int PaymentDescriptionMinLength = 1;
        }

        public static class TruckCourse
        {
            public const int DeliverAddressMaxLength = 250;
            public const int DeliverAddressMinLength = 1;

            public const int PickupAddressAddressMaxLength = 250;
            public const int PickupAddressAddressMinLength = 1;
        }

        public static class Payment
        {
            public const int PaymentDescriptionMaxLength = 100;
            public const int PaymentDescriptionMinLength = 1;
        }

        public static class ChangePassword
        {
            public const int NewPasswordMaxLength = 100;
            public const int NewPasswordMinLength = 8;
        }

        public static class EditProfile
        {
            public const int UserNameMaxLength = 50;
            public const int UserNameMinLength = 3;

            public const int UserPhoneMaxLength = 20;
            public const int UserPhoneMinLength = 10;
        }

        public static class PersonalProfile
        {
            public const int FirstNameMaxLength = 100;
            public const int FirstNameMinLength = 1;

            public const int LastNameMaxLength = 100;
            public const int LastNameMinLength = 1;

            public const int AdressMaxLength = 250;
            public const int AdressMinLength = 1;

            public const int PersonalNumberMaxLength = 50;
            public const int PersonalNumberMinLength = 5;
        }
        public static class CompanyProfile
        {
            public const int CompanyNameMaxLength = 250;
            public const int CompanyNameMinLength = 1;

            public const int VATNumberMaxLength = 50;
            public const int VATNumberMinLength = 1;

            public const int CompanyAdressMaxLength = 250;
            public const int CompanyAdressMinLength = 1;

            public const int ContactPersonNameMaxLength = 100;
            public const int ContactPersonNameMinLength = 1;

            public const int ContactPhoneMaxLength = 20;
            public const int ContactPhoneMinLength = 1;
        }

        public static class Message
        {
            public const int ContentMaxLength = 500;
            public const int ContentMinLength = 1;
        }


    }
}
