namespace OrderFlow.GCommon
{
    /// <summary>
    /// Defines the <see cref="ValidationConstants" />
    /// </summary>
    public class ValidationConstants
    {
        /// <summary>
        /// Defines the DateFormat
        /// </summary>
        public static string DateFormat = "dd/MM/yyyy";

        /// <summary>
        /// Defines the <see cref="Order" />
        /// </summary>
        public static class Order
        {
            /// <summary>
            /// Defines the DeliveryInstructionsMaxLength
            /// </summary>
            public const int DeliveryInstructionsMaxLength = 500;

            /// <summary>
            /// Defines the DeliveryInstructionsMinLength
            /// </summary>
            public const int DeliveryInstructionsMinLength = 1;

            /// <summary>
            /// Defines the DeliveryAddressMaxLength
            /// </summary>
            public const int DeliveryAddressMaxLength = 250;

            /// <summary>
            /// Defines the DeliveryAddressMinLength
            /// </summary>
            public const int DeliveryAddressMinLength = 3;

            /// <summary>
            /// Defines the PickupAddressMaxLength
            /// </summary>
            public const int PickupAddressMaxLength = 250;

            /// <summary>
            /// Defines the PickupAddressMinLength
            /// </summary>
            public const int PickupAddressMinLength = 3;

            /// <summary>
            /// Defines the LoadCapacityMaxLength
            /// </summary>
            public const double LoadCapacityMaxLength = double.MaxValue;

            /// <summary>
            /// Defines the LoadCapacityMinLength
            /// </summary>
            public const int LoadCapacityMinLength = 1;
        }

        /// <summary>
        /// Defines the <see cref="Notification" />
        /// </summary>
        public static class Notification
        {
            /// <summary>
            /// Defines the TitleMaxLenght
            /// </summary>
            public const int TitleMaxLenght = 100;

            /// <summary>
            /// Defines the TitleMinLenght
            /// </summary>
            public const int TitleMinLenght = 1;

            /// <summary>
            /// Defines the MessageMaxLength
            /// </summary>
            public const int MessageMaxLength = 500;

            /// <summary>
            /// Defines the MessageMinLength
            /// </summary>
            public const int MessageMinLength = 5;
        }

        /// <summary>
        /// Defines the <see cref="Truck" />
        /// </summary>
        public static class Truck
        {
            /// <summary>
            /// Defines the LicensePlateMaxLength
            /// </summary>
            public const int LicensePlateMaxLength = 20;

            /// <summary>
            /// Defines the LicensePlateMinLength
            /// </summary>
            public const int LicensePlateMinLength = 8;

            /// <summary>
            /// Defines the CapacityMaxLength
            /// </summary>
            public const int CapacityMaxLength = int.MaxValue;

            /// <summary>
            /// Defines the CapacityMinLength
            /// </summary>
            public const int CapacityMinLength = 1;
        }

        /// <summary>
        /// Defines the <see cref="TruckSpending" />
        /// </summary>
        public static class TruckSpending
        {
            /// <summary>
            /// Defines the PaymentDescriptionMaxLength
            /// </summary>
            public const int PaymentDescriptionMaxLength = 100;

            /// <summary>
            /// Defines the PaymentDescriptionMinLength
            /// </summary>
            public const int PaymentDescriptionMinLength = 1;
        }

        /// <summary>
        /// Defines the <see cref="TruckCourse" />
        /// </summary>
        public static class TruckCourse
        {
            /// <summary>
            /// Defines the DeliverAddressMaxLength
            /// </summary>
            public const int DeliverAddressMaxLength = 250;

            /// <summary>
            /// Defines the DeliverAddressMinLength
            /// </summary>
            public const int DeliverAddressMinLength = 1;

            /// <summary>
            /// Defines the PickupAddressAddressMaxLength
            /// </summary>
            public const int PickupAddressAddressMaxLength = 250;

            /// <summary>
            /// Defines the PickupAddressAddressMinLength
            /// </summary>
            public const int PickupAddressAddressMinLength = 1;
        }

        /// <summary>
        /// Defines the <see cref="Payment" />
        /// </summary>
        public static class Payment
        {
            /// <summary>
            /// Defines the PaymentDescriptionMaxLength
            /// </summary>
            public const int PaymentDescriptionMaxLength = 100;

            /// <summary>
            /// Defines the PaymentDescriptionMinLength
            /// </summary>
            public const int PaymentDescriptionMinLength = 1;
        }

        /// <summary>
        /// Defines the <see cref="ChangePassword" />
        /// </summary>
        public static class ChangePassword
        {
            /// <summary>
            /// Defines the NewPasswordMaxLength
            /// </summary>
            public const int NewPasswordMaxLength = 100;

            /// <summary>
            /// Defines the NewPasswordMinLength
            /// </summary>
            public const int NewPasswordMinLength = 8;
        }

        /// <summary>
        /// Defines the <see cref="EditProfile" />
        /// </summary>
        public static class EditProfile
        {
            /// <summary>
            /// Defines the UserNameMaxLength
            /// </summary>
            public const int UserNameMaxLength = 50;

            /// <summary>
            /// Defines the UserNameMinLength
            /// </summary>
            public const int UserNameMinLength = 3;

            /// <summary>
            /// Defines the UserPhoneMaxLength
            /// </summary>
            public const int UserPhoneMaxLength = 20;

            /// <summary>
            /// Defines the UserPhoneMinLength
            /// </summary>
            public const int UserPhoneMinLength = 10;
        }

        /// <summary>
        /// Defines the <see cref="PersonalProfile" />
        /// </summary>
        public static class PersonalProfile
        {
            /// <summary>
            /// Defines the FirstNameMaxLength
            /// </summary>
            public const int FirstNameMaxLength = 100;

            /// <summary>
            /// Defines the FirstNameMinLength
            /// </summary>
            public const int FirstNameMinLength = 1;

            /// <summary>
            /// Defines the LastNameMaxLength
            /// </summary>
            public const int LastNameMaxLength = 100;

            /// <summary>
            /// Defines the LastNameMinLength
            /// </summary>
            public const int LastNameMinLength = 1;

            /// <summary>
            /// Defines the AdressMaxLength
            /// </summary>
            public const int AdressMaxLength = 250;

            /// <summary>
            /// Defines the AdressMinLength
            /// </summary>
            public const int AdressMinLength = 1;

            /// <summary>
            /// Defines the PersonalNumberMaxLength
            /// </summary>
            public const int PersonalNumberMaxLength = 50;

            /// <summary>
            /// Defines the PersonalNumberMinLength
            /// </summary>
            public const int PersonalNumberMinLength = 5;
        }

        /// <summary>
        /// Defines the <see cref="CompanyProfile" />
        /// </summary>
        public static class CompanyProfile
        {
            /// <summary>
            /// Defines the CompanyNameMaxLength
            /// </summary>
            public const int CompanyNameMaxLength = 250;

            /// <summary>
            /// Defines the CompanyNameMinLength
            /// </summary>
            public const int CompanyNameMinLength = 1;

            /// <summary>
            /// Defines the VATNumberMaxLength
            /// </summary>
            public const int VATNumberMaxLength = 50;

            /// <summary>
            /// Defines the VATNumberMinLength
            /// </summary>
            public const int VATNumberMinLength = 1;

            /// <summary>
            /// Defines the CompanyAdressMaxLength
            /// </summary>
            public const int CompanyAdressMaxLength = 250;

            /// <summary>
            /// Defines the CompanyAdressMinLength
            /// </summary>
            public const int CompanyAdressMinLength = 1;

            /// <summary>
            /// Defines the ContactPersonNameMaxLength
            /// </summary>
            public const int ContactPersonNameMaxLength = 100;

            /// <summary>
            /// Defines the ContactPersonNameMinLength
            /// </summary>
            public const int ContactPersonNameMinLength = 1;

            /// <summary>
            /// Defines the ContactPhoneMaxLength
            /// </summary>
            public const int ContactPhoneMaxLength = 20;

            /// <summary>
            /// Defines the ContactPhoneMinLength
            /// </summary>
            public const int ContactPhoneMinLength = 1;
        }

        /// <summary>
        /// Defines the <see cref="Message" />
        /// </summary>
        public static class Message
        {
            /// <summary>
            /// Defines the ContentMaxLength
            /// </summary>
            public const int ContentMaxLength = 500;

            /// <summary>
            /// Defines the ContentMinLength
            /// </summary>
            public const int ContentMinLength = 1;
        }
    }
}
