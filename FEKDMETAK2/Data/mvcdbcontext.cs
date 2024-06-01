using FEKDMETAK.Models;
using FEKDMETAK2.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Data.SqlClient;

namespace FEKDMETAK.Data
{
    public class mvcdbcontext : DbContext
    {
        public mvcdbcontext()
        {
        }
        public mvcdbcontext(DbContextOptions<mvcdbcontext> options) : base(options)
        {
        }
        public DbSet<User> Users { get; set; }
     
        public DbSet<Review> Reviews { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<Specialization> Specializations { get; set; }
        public DbSet<City> Cities { get; set; }
        public DbSet<Town> Towns { get; set; }
        public object specializations { get;internal set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Notification>()
                .HasOne(n => n.Reciever)
                .WithMany(u => u.recievedNotifiaction)
                .HasForeignKey(n => n.RecieverId)
                .OnDelete(DeleteBehavior.ClientSetNull); // or whatever delete behavior you want

            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Notification>()
                .HasOne(n => n.Sender)
                .WithMany(n=>n.sentNotifications) // There's no navigation property on User pointing back to Notification, so no need for WithMany
                .HasForeignKey(n => n.SenderId)
                .OnDelete(DeleteBehavior.ClientSetNull); // or whatever delete behavior you want

            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Review>()
                .HasOne(r => r.ReviewedUser)
                .WithMany(u => u.RecievedReview)
                .HasForeignKey(r => r.ReviewedUserId)
                .OnDelete(DeleteBehavior.ClientSetNull);
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Review>()
                .HasOne(r => r.Reviewer)
                .WithMany(u => u.GivenReview)
                .HasForeignKey(r => r.ReviewerId)
                .OnDelete(DeleteBehavior.ClientSetNull);
            modelBuilder.Entity<Notification>()
                .HasKey(n => n.NOId);
            modelBuilder.Entity<User>()
                .HasKey(n => n.Uid);
            modelBuilder.Entity<Review>()
                .HasKey(n => n.RId);
            modelBuilder.Entity<Specialization>()
                .HasKey(n => n.Sid);
            modelBuilder.Entity<User>()
            .HasOne(u => u.Town)
            .WithMany()
            .HasForeignKey(u => u.TownId);

            modelBuilder.Entity<Town>()
                .HasOne(t => t.City)
                .WithMany(c => c.Towns)
                .HasForeignKey(t => t.CityId);
            base.OnModelCreating(modelBuilder);
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("MyDatabase");
            }
        }

        internal Task UpdateAsync(User user)
        {
            throw new NotImplementedException();
        }
    }
}
