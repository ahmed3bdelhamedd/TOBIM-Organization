using Microsoft.EntityFrameworkCore;
using TobimSystem.Models;

namespace TobimSystem.Data
{
    public class TobimDbContext : DbContext
    {
        public TobimDbContext(DbContextOptions<TobimDbContext> options) : base(options) { }

        public DbSet<Subscription> Subscriptions { get; set; }
    }
}
