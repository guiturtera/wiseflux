﻿using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
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
            builder.Entity<Sensor>().Property(p => p.SensorId).ValueGeneratedOnAdd();
            builder.Entity<SensorMeasure>().Property(p => p.MeasureId).ValueGeneratedOnAdd();
            builder.Entity<NotificationModel>().Property(p => p.NotificationId).ValueGeneratedOnAdd();

            // Only to tests
            builder.Entity<User>().HasData(new User
            {
                Email = "guiturtera@hotmail.com",
                PhoneNumber = "(11)98741-0155",
                Username = "guiturtera",
                Role = EnumUserRoles.Vip,
                Password = "/OoMy+WT8ybJ+X9ZlPIMBCaUHO58ShGcZK5a0qwyzA00+PpP"
            });
        }

        public DbSet<User> Users { get; set; } // STRONG
        public DbSet<Sensor> Sensors { get; set; } // STRONG
        public DbSet<SensorMeasure> SensorMeasures { get; set; } // ASSOCIATIVE
        public DbSet<NotificationModel> Notifications { get; set; } // ASSOCIATIVE
    }
}
