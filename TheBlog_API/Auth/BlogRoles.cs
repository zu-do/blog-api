namespace TheBlog_API.Auth
{
    public static class BlogRoles
    {
        public const string Admin = nameof(Admin);
        public const string BlogUser = nameof(BlogUser);

        public static readonly IReadOnlyCollection<string> All = new[] { Admin, BlogUser };
    }
}
