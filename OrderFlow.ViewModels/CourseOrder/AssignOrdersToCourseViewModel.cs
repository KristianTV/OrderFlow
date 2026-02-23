using System.ComponentModel.DataAnnotations;

namespace OrderFlow.ViewModels.CourseOrder
{
    public class AssignOrdersToCourseViewModel
    {
        public Guid CourseId { get; set; }
        public double Capacity { get; set; }
        public IList<OrderViewModel> Orders { get; set; } = new List<OrderViewModel>();

        [Display(Name = "Load Capacity")]
        public double LoadedCapacity { get; set; } = 0;
    }
}
