namespace OrderFlow.Services
{
    public class RealtimeEntityChanged
    {
        public string Entity { get; set; } = string.Empty;

        public string Action { get; set; } = string.Empty;

        public Guid? Id { get; set; }

        public Guid? RelatedId { get; set; }

        public IEnumerable<string> Roles { get; set; } = Enumerable.Empty<string>();

        public IEnumerable<Guid> UserIds { get; set; } = Enumerable.Empty<Guid>();
    }
}
