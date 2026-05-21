using OrderFlow.Data.Models.Enums;
using OrderFlow.GCommon;
using System.ComponentModel.DataAnnotations;

namespace OrderFlow.ViewModels.TruckSpending
{
    public class CreateTruckSpendingViewModel
    {
        [Required(ErrorMessage = ErrorMessages.PropertyIsRequired)]
        [Display(Name = "Truck")]
        public Guid? TruckID { get; set; }

        [Display(Name = "Course")]
        public Guid? TruckCourseID { get; set; }

        [Required(ErrorMessage = ErrorMessages.PropertyIsRequired)]
        [Range(0.01, 999999.99, ErrorMessage = ErrorMessages.RangeError)]
        [Display(Name = "Amount")]
        public decimal Amount { get; set; }

        [Required(ErrorMessage = ErrorMessages.PropertyIsRequired)]
        [DataType(DataType.Date)]
        [Display(Name = "Payment Date")]
        public DateTime PaymentDate { get; set; } = DateTime.UtcNow.Date;

        [StringLength(ValidationConstants.TruckSpending.PaymentDescriptionMaxLength,
                      MinimumLength = ValidationConstants.TruckSpending.PaymentDescriptionMinLength,
                      ErrorMessage = ErrorMessages.StringLengthRange)]
        [Required(ErrorMessage = ErrorMessages.PropertyIsRequired)]
        [Display(Name = "Description")]
        public string PaymentDescription { get; set; } = string.Empty;

        [Required(ErrorMessage = ErrorMessages.PropertyIsRequired)]
        [Display(Name = "Payment Method")]
        public PaymentMethods PaymentMethod { get; set; } = PaymentMethods.Cash;

        public IDictionary<Guid, string> AvailableTrucks { get; set; } = new Dictionary<Guid, string>();

        public IDictionary<Guid, string> AvailableCourses { get; set; } = new Dictionary<Guid, string>();
    }
}
