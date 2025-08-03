using OrderFlow.GCommon;
using System.ComponentModel.DataAnnotations;

namespace OrderFlow.ViewModels.Order
{
    public class AdminCreateOrderViewModel: CreateOrderViewModel
    {
        [Required(ErrorMessage = ErrorMessages.PropertyIsRequired)]
        [Display(Name = "User")]
        public Guid UsersId { get; set; }

        public IDictionary<Guid, string?>? Users { get; set; } = new Dictionary<Guid, string>();
    }
}
