namespace OrderFlow.GCommon
{
    /// <summary>
    /// Defines the <see cref="ErrorMessages" />
    /// </summary>
    public static class ErrorMessages
    {
        /// <summary>
        /// Defines the PropertyIsRequired
        /// </summary>
        public const string PropertyIsRequired = "{0} is required.";

        /// <summary>
        /// Defines the StringLengthRange
        /// </summary>
        public const string StringLengthRange = "{0} must be between {2} and {1} characters long.";

        /// <summary>
        /// Defines the PositiveNumber
        /// </summary>
        public const string PositiveNumber = "{0} must be a positive number.";

        /// <summary>
        /// Defines the RangeError
        /// </summary>
        public const string RangeError = "{0} must be between {1} and {2}.";

        /// <summary>
        /// Defines the UserNameIsRequired
        /// </summary>
        public const string UserNameIsRequired = "Username is required.";

        /// <summary>
        /// Defines the UserEmailIsRequired
        /// </summary>
        public const string UserEmailIsRequired = "Email is required.";

        /// <summary>
        /// Defines the UserPhoneIsRequired
        /// </summary>
        public const string UserPhoneIsRequired = "Invalid phone number format.";

        /// <summary>
        /// Defines the EmailAddressInvalidFormat
        /// </summary>
        public const string EmailAddressInvalidFormat = "Invalid email format.";

        /// <summary>
        /// Defines the ConfirmPasswordCompare
        /// </summary>
        public const string ConfirmPasswordCompare = "The new password and confirmation password do not match.";
    }
}
