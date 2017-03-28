using Microsoft.EntityFrameworkCore;
 
namespace events.Models
{
    public class MyContext : DbContext
    {
        // base() calls the parent class' constructor passing the "options" parameter along
        public MyContext(DbContextOptions<MyContext> options) : base(options) { }
        public DbSet<User> users { get; set; }
        public DbSet<Event> events { get; set; }
        public DbSet<Reserve> reserves { get; set; }
    }
}
