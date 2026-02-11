using OrderFlow.GCommon;
using System.ComponentModel.DataAnnotations;

namespace OrderFlow.ViewModels.Truck
{
    public class CreateTruckViewModel
    {
        public Guid DriverID { get; set; }

        [StringLength(ValidationConstants.Truck.LicensePlateMaxLength,
              MinimumLength = ValidationConstants.Truck.LicensePlateMinLength,
              ErrorMessage = ErrorMessages.StringLengthRange)]
        [Required(ErrorMessage = ErrorMessages.PropertyIsRequired)]
        [Display(Name = "License Plate")]
        public string LicensePlate { get; set; } = string.Empty;

        [Required(ErrorMessage = ErrorMessages.PropertyIsRequired)]
        [Range(ValidationConstants.Truck.CapacityMinLength,
                    ValidationConstants.Truck.CapacityMaxLength,
                    ErrorMessage = ErrorMessages.PositiveNumber)]
        [Display(Name = "Capacity")]
        public double Capacity { get; set; } = 0;

        public IDictionary<Guid, string>? Drivers { get; set; } = new Dictionary<Guid, string>();
    }
}
