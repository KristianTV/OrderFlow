namespace OrderFlow.ViewModels.Course
{
    public class CourseQueryModel
    {
        public bool HideCompleted { get; set; }

        public string? SearchId { get; set; }

        public string? StatusFilter { get; set; }

        public string? SortOrder { get; set; }
    }
}
