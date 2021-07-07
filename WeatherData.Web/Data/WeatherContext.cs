using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WeatherData.Web.Models;

namespace WeatherData.Web.Data
{
    public class WeatherContext : DbContext
    {
        public WeatherContext(DbContextOptions<WeatherContext> options) : base(options)
        {

        }

        public DbSet<Measurement> Measurements { get; set; }
        public DbSet<Location> Locations { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Measurement>()
                .ToTable("Measurements");

            modelBuilder.Entity<Location>()
                .ToTable("Locations")
                .HasMany(x => x.Measurements)
                .WithOne(y => y.Location)
                .OnDelete(DeleteBehavior.NoAction);

        }
    }
}
