using OrderFlow.GCommon;
using System.ComponentModel.DataAnnotations;

namespace OrderFlow.ViewModels.Notification
{
    public class CreateNotificationViewModel
    {
        [StringLength(ValidationConstants.Notification.TitleMaxLenght,
                     MinimumLength = ValidationConstants.Notification.TitleMinLenght,
                     ErrorMessage = ErrorMessages.StringLengthRange)]
        [Required(ErrorMessage = ErrorMessages.PropertyIsRequired)]
        [Display(Name = "Title")]
        public string Title { get; set; } = string.Empty;

        [StringLength(ValidationConstants.Notification.MessageMaxLength,
                    MinimumLength = ValidationConstants.Notification.MessageMinLength,
                    ErrorMessage = ErrorMessages.StringLengthRange)]
        [Required(ErrorMessage = ErrorMessages.PropertyIsRequired)]
        [Display(Name = "Message")]
        public string Message { get; set; } = string.Empty;

        [Required(ErrorMessage = ErrorMessages.PropertyIsRequired)]
        [Display(Name = "Receiver Id")]
        public Guid ReceiverId { get; set; }

        [Display(Name = "Order Id")]
        public Guid? OrderId { get; set; }

        public IDictionary<Guid, string>? Orders { get; set; } = new Dictionary<Guid, string>();

        public IDictionary<Guid, string?>? Receivers { get; set; } = new Dictionary<Guid, string?>();

    }
}
