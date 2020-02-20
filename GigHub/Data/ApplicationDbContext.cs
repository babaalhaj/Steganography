using GigHub.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace GigHub.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Gig> Gigs { get; set; }
        public DbSet<Genre> Genres { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Genre>().HasData(
                new Genre{Id = 1, Name = "Blues"},
                new Genre{Id= 2, Name = "Country"},
                new Genre{Id= 3, Name = "Jazz"},
                new Genre{Id = 4, Name = "R & B"},
                new Genre{Id = 5, Name = "Rock"}
                );
            base.OnModelCreating(builder);
        }
    }
}
