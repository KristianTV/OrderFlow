using OrderFlow.GCommon;
using System.ComponentModel.DataAnnotations;

namespace OrderFlow.ViewModels.Course
{
    public class CreateCourseViewModel
    {
        [Display(Name = "Truck")]
        public Guid? SelectedTruckID { get; set; }

        public IDictionary<Guid, string> AvailableTruckIDs { get; set; } = new Dictionary<Guid, string>();

        [StringLength(ValidationConstants.TruckCourse.PickupAddressAddressMaxLength,
                      MinimumLength = ValidationConstants.TruckCourse.PickupAddressAddressMinLength,
                      ErrorMessage = ErrorMessages.StringLengthRange)]
        [Required(ErrorMessage = ErrorMessages.PropertyIsRequired)]
        [Display(Name = "Pickup Address")]
        public string PickupAddress { get; set; } = string.Empty;

        [StringLength(ValidationConstants.TruckCourse.DeliverAddressMaxLength,
                        MinimumLength = ValidationConstants.TruckCourse.DeliverAddressMinLength,
                        ErrorMessage = ErrorMessages.StringLengthRange)]
        [Required(ErrorMessage = ErrorMessages.PropertyIsRequired)]
        [Display(Name = "Delivery Address")]
        public string DeliverAddress { get; set; } = string.Empty;

        [Display(Name = "Income")]
        public decimal? Income { get; set; }
    }
}
