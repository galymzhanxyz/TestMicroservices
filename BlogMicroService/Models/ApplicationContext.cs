using Microsoft.EntityFrameworkCore;

namespace BlogMicroService.Models
{
    public class ApplicationContext : DbContext
    {
        protected readonly IConfiguration Configuration;

        public DbSet<Post> Posts { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options) { }

    }
}
