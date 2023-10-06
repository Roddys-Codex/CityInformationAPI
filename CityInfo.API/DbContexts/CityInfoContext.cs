using CityInfo.API.Entities;
using Microsoft.EntityFrameworkCore;

namespace CityInfo.API.DbContexts
{
    public class CityInfoContext : DbContext
    {
        public DbSet<City> Cities { get; set; } = null!;
        public DbSet<PointOfInterest> PointOfInterests { get; set; } = null!;
        
        public CityInfoContext(DbContextOptions<CityInfoContext> options)
        : base(options)
        {
            
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<City>().HasData(
                new City("New York City")
                {
                    Id = 1,
                    Description = "The one with big parks."
                },
                new City("Antwerp")
                {
                    Id = 2,
                    Description = "The one with the church."
                }, 
                new City("Paris")
                {
                    Id = 3,
                    Description = "The one with the Eiffel tower."
                });
            
            modelBuilder.Entity<PointOfInterest>().HasData(
                new PointOfInterest("Central Park")
                {
                    Id = 1,
                    CityId = 1,
                    Description = "big park."
                },
                new PointOfInterest("Empire State Building")
                {
                    Id = 2,
                    CityId = 1,
                    Description = "big building."
                }, 
                new PointOfInterest("Cathedral")
                {
                    Id = 3,
                    CityId = 2,
                    Description = "big church."
                },
                new PointOfInterest("Central station")
                {
                    Id = 4,
                    CityId = 2,
                    Description = "big station."
                }
                ,
                new PointOfInterest("Eiffel tower")
                {
                    Id = 5,
                    CityId = 3,
                    Description = "big tower."
                },
                new PointOfInterest("The Louvre")
                {
                    Id = 6,
                    CityId = 3,
                    Description = "big museum."
                });
            
            base.OnModelCreating(modelBuilder);
        }

        // protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        // {
        //     optionsBuilder.UseSqlite("connection string"); 
        //     base.OnConfiguring(optionsBuilder);
        // }
    }    
}
