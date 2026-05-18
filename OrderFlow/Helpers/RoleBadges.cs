namespace OrderFlow.Helpers
{
    public static class RoleBadges
    {
        public static string GetRoleBadgeClass(string role)
        {
            return role switch
            {
                "Admin" => "role-badge-admin",
                "Driver" => "role-badge-driver",
                "Speditor" => "role-badge-speditor",
                "User" => "role-badge-user",
                _ => "role-badge-default"
            };
        }

        public static string GetRoleIconClass(string role)
        {
            return role switch
            {
                "Admin" => "fas fa-shield-alt",
                "Driver" => "fas fa-truck",
                "Speditor" => "fas fa-boxes",
                "User" => "fas fa-user",
                _ => "fas fa-tag"
            };
        }
    }
}
