namespace OrderFlow.Hubs
{
    public static class RealtimeGroups
    {
        public static string Role(string roleName) => $"role:{roleName}";

        public static string User(string userId) => $"user:{userId}";
    }
}
