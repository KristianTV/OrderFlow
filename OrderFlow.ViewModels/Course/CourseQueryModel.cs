namespace OrderFlow.ViewModels.Course
{
    public class CourseQueryModel
    {
        public bool HideCompleted { get; set; } = true;

        public string? SearchId { get; set; } = null;

        public string? StatusFilter { get; set; } = null;

        public string? SortOrder { get; set; } = null;

        public int Page { get; set; } = 1;

        public int PageSize { get; set; } = 12;
    }
}
