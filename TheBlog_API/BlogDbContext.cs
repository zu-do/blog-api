using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TheBlog_API.Data.Models;

namespace TheBlog_API
{
    public class BlogDbContext : IdentityDbContext<ApplicationUser>
    {
        public DbSet<Article> DbArticles { get; set; }
        public DbSet<UserArticleVote> DbUserArticleVotes { get; set; }
        public DbSet<Comment> DbComments { get; set; }
        public DbSet<Report> DbReports { get; set; }
        public BlogDbContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<UserArticleVote>()
                .HasOne(uav => uav.User)
                .WithMany(u => u.ArticleVotes)
                .HasForeignKey(uav => uav.UserId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<UserArticleVote>()
                .HasOne(uav => uav.Article)
                .WithMany(a => a.UserVotes)
                .HasForeignKey(uav => uav.ArticleId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Comment>()
                .HasOne(c => c.Article)
                .WithMany(a => a.Comments)
                .HasForeignKey(c => c.ArticleId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Comment>()
                .HasOne(c => c.User)
                .WithMany(u => u.Comments)
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Report>()
                .HasOne<Comment>()
                .WithMany(c => c.Reports)
                .HasForeignKey(r => r.CommentId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Report>()
               .HasOne<ApplicationUser>()
               .WithMany(u => u.ReportedComments)
               .HasForeignKey(r => r.ApplicationUserId)
               .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
