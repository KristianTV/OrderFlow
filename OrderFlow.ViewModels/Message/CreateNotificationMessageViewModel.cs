using OrderFlow.GCommon;
using System.ComponentModel.DataAnnotations;

namespace OrderFlow.ViewModels.Message
{
    public class CreateNotificationMessageViewModel
    {
        [StringLength(ValidationConstants.Message.ContentMaxLength,
                     MinimumLength = ValidationConstants.Message.ContentMinLength,
                     ErrorMessage = ErrorMessages.StringLengthRange)]
        [Required(ErrorMessage = ErrorMessages.PropertyIsRequired)]
        [Display(Name = "Message Content")]
        public string Content { get; set; } = string.Empty;

        public Guid? SenderID { get; set; }

        public Guid? ReceiverID { get; set; }

        public Guid? NotificationID { get; set; }
    }
}
