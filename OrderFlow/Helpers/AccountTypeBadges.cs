namespace OrderFlow.Helpers
{
    public static class AccountTypeBadges
    {
        public static string GetInitials(string username)
        {
            if (string.IsNullOrWhiteSpace(username)) return "?";
            var parts = username.Split(new[] { '.', '_', '-', ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length >= 2)
                return $"{char.ToUpper(parts[0][0])}{char.ToUpper(parts[1][0])}";
            return username.Length >= 2
                ? $"{char.ToUpper(username[0])}{char.ToUpper(username[1])}"
                : char.ToUpper(username[0]).ToString();
        }

        public static string GetAvatarStyle(string username)
        {
            var palettes = new[]
            {
            "style=background:#dbeafe;color:#1e40af;",
            "style=background:#d1fae5;color:#065f46;",
            "style=background:#ede9fe;color:#5b21b6;",
            "style=background:#fef3c7;color:#92400e;",
            "style=background:#fce7f3;color:#9d174d;",
            "style=background:#ffedd5;color:#9a3412;",
            "style=background:#e0f2fe;color:#0369a1;",
            "style=background:#fae8ff;color:#86198f;",
            "style=background:#f0fdf4;color:#166534;",
            "style=background:#ffe4e6;color:#9f1239;",
            "style=background:#ecfeff;color:#155e75;",
            "style=background:#f5f3ff;color:#6b21a8;",
            "style=background:#ccfbf1;color:#115e59;",
            "style=background:#ffedd5;color:#c2410c;",
            "style=background:#e0e7ff;color:#3730a3;",
            "style=background:#fef08a;color:#854d0e;",
            "style=background:#f1f5f9;color:#334155;",
            "style=background:#e2e8f0;color:#1e293b;",
            "style=background:#f0f9ff;color:#075985;",
            "style=background:#fdf2f8;color:#9d174d;",
            "style=background:#f0fdf5;color:#166534;",
            "style=background:#fffbeb;color:#92400e;",
            "style=background:#faf5ff;color:#5b21b6;",
            "style=background:#f8fafc;color:#475569;",
            "style=background:#f0fdf4;color:#165b33;",
            "style=background:#fefce8;color:#713f12;",
            "style=background:#fff7ed;color:#7c2d12;",
            "style=background:#fafaf9;color:#44403c;",
            "style=background:#f5f5f4;color:#57534e;",
            "style=background:#edf2f7;color:#2d3748;",
        };
            int index = Math.Abs(username.GetHashCode()) % palettes.Length;
            return palettes[index];
        }

        public static string GetAccountTypeBadgeClass(string accountType)
        {
            return accountType switch
            {
                "Personal" => "account-type-badge-personal",
                _ => "account-type-badge-company"
            };
        }
    }
}
