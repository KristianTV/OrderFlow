namespace OrderFlow.Data.Models
{
    public class CourseOrder
    {
        public Guid OrderID { get; set; }
        public Order Order { get; set; } = null!;

        public Guid TruckCourseID { get; set; }
        public TruckCourse TruckCourse { get; set; } = null!;
    }
}