using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;
using Wiseflux.Models;

namespace Wiseflux.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            /*builder.Entity<Spreadsheet>().HasKey(table => new
            {
                table.User,
                table.Day
            });*/

            //builder.Entity<PhoneNumber>().Property(x => x.Id).HasDefaultValueSql("NEWID()");

        }

        public DbSet<User> Users { get; set; } // STRONG
        //public DbSet<Spreadsheet> Spreadsheets { get; set; } // ASSOCIATIVE
    }
}
