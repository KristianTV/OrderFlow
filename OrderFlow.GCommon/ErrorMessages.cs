namespace OrderFlow.GCommon
{
    public static class ErrorMessages
    {
        public const string PropertyIsRequired = "{0} is required.";
        public const string StringLengthRange = "{0} must be between {2} and {1} characters long.";
        public const string PositiveNumber = "{0} must be a positive number.";
        public const string RangeError = "{0} must be between {1} and {2}.";
        public const string UserNameIsRequired = "Username is required.";
        public const string UserEmailIsRequired = "Email is required.";
        public const string UserPhoneIsRequired = "Invalid phone number format.";
        public const string EmailAddressInvalidFormat = "Invalid email format.";
        public const string ConfirmPasswordCompare = "The new password and confirmation password do not match.";
    }
}
