using AzureTest.Models.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AzureTest.Models
{
    public class AppDbContext : IdentityDbContext<UserModel, AppRole, int>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }
        public DbSet<FollowingNotificationsModel> FollowingNotificationsTable { get; set; }
        public DbSet<FollowModel> FollowTable { get; set; }
        public DbSet<LikeProjectPageModel> LikeProjectPageTable2 { get; set; }
        public DbSet<NotificationConnectionModel> NotificationConnectionTable { get; set; }
        public DbSet<NotificationModel> NotificationTable { get; set; }
        public DbSet<ProjectModel> ProjectTable { get; set; }
        public DbSet<ProjectPageCommentModel> ProjectPageCommentTable { get; set; }
        public DbSet<ProjectPageModel> ProjectPageTable { get; set; }
        public DbSet<PropertyModel> PropertyTable { get; set; }
        public DbSet<SaveItemModel> SaveItemTable { get; set; }
    }
}
