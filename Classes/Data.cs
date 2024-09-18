using Badger.Class;
using Microsoft.EntityFrameworkCore;

namespace Badger.Classes
{
    public class BadgerContext : DbContext
    {
        public DbSet<UserInfo> UserInfo { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            string db = "Badger";
            options.UseSqlServer(Util.GetDBConnectionString(db));
            options.EnableSensitiveDataLogging(true);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserInfo>().ToTable("UserInfo");
        }
    }
}
