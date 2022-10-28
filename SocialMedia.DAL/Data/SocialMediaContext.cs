using Microsoft.EntityFrameworkCore;
using SocialMedia.Core.Entities;
using System.Reflection;

namespace SocialMedia.Infrastructure.Data
{
    public partial class SocialMediaContext : DbContext
    {
        /*public SocialMediaContext()
        {
        }
        public SocialMediaContext(DbContextOptions<SocialMediaContext> options)
            : base(options)
        {
        }
        */
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder.UseSqlServer(
                "Server=localhost\\SQLEXPRESS;Database=SocialMedia;Integrated Security=true");
        }

        public virtual DbSet<Comment> Comments { get; set; }
        public virtual DbSet<Post> Posts { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<Security> Securities { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }
    }
}
