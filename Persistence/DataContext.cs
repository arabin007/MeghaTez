using Domain;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;

namespace Persistence
{
    public class DataContext: IdentityDbContext<AppUser>
    {
        public DataContext(DbContextOptions options) :base(options) 
        {
           
        }
        public DbSet<Value> tblValues { get; set; }
        public DbSet<Activity> tblActivities { get; set; }
        public DbSet<UserActivity> tblUsersActivities { get; set; }
        public DbSet<Photo> tblPhotos { get; set; }
        public DbSet<Consumer> tblConsumers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);     //Added this after adding IdentityDbContext so that it can use string primary key automatically

            modelBuilder.Entity<Value>().HasData(   //Seeding Values Data
                new Value { Id = 1, Name = "Value1" },
                new Value { Id = 2, Name = "Value2" },
                new Value { Id = 3, Name = "Value3" }
                );

            modelBuilder.Entity<UserActivity>(x => x.HasKey(ua => new { ua.AppUserId, ua.ActivityId }));

            modelBuilder.Entity<UserActivity>()
                .HasOne(u => u.AppUser)
                .WithMany(ua => ua.UserActivities)   // Specifying the ICollection in AppUser
                .HasForeignKey(u => u.AppUserId);

            modelBuilder.Entity<UserActivity>()
                .HasOne(a => a.Activity)
                .WithMany(ua => ua.UserActivities)   // Specifying the ICollection in Activity
                .HasForeignKey(a => a.ActivityId);

            modelBuilder.Entity<AppUser>()           // AppUser is Principal table
                  .HasOne(c => c.Consumer)           // One AppUser has One Consumer
                  .WithOne(u => u.AppUser)
                  .HasForeignKey<Consumer>(f => f.AppIdFK);
                                                     // Consumer Table has the foreign key column as AppIdFK
        }
    }
}
