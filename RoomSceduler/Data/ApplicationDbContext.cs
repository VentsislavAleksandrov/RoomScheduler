using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using RoomSceduler.Data.Models;

namespace RoomSceduler.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public DbSet<Room> Rooms { get; set; }

        public DbSet<TimeSlot> TimeSlots { get; set; }

        public DbSet<RoomTimeslots> RoomTimeslots { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<RoomTimeslots>()
                .HasKey(x => new { x.RoomId, x.TimeSlotId});

            base.OnModelCreating(builder);


        }
    }
}
