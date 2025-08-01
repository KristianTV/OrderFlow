namespace OrderFlow.ViewModels.Role
{
    public class IndexUserRowsViewModel
    {
        public Guid UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public IEnumerable<string> Roles { get; set; } = new List<string>();
    }
}
