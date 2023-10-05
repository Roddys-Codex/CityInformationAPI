using System.Diagnostics.CodeAnalysis;
using CityInfo.API.Models;

namespace CityInfo.API
{
    public class CitiesDataStore 
    {
        public List<CityDto> Cities { get; set; }

        public static CitiesDataStore Current { get; } = new CitiesDataStore();
        
        // ReSharper disable All
        public CitiesDataStore()
        {
            Cities = new List<CityDto>()
            {
                new CityDto()
                {
                    Id = 1,
                    Name = "New York",
                    Description = "The one with the big park.",
                    PointsOfInterest = new List<PointOfInterestDto>()
                    {
                        new PointOfInterestDto()
                        {
                            Id = 1,
                            Name = "Central Park",
                            Description = "The most visited park..."
                        },
                        new PointOfInterestDto()
                        {
                            Id = 2,
                            Name = "Central Park",
                            Description = "The most visited park..."
                        }
                    }
                },
                new CityDto()
                {
                    Id = 2,
                    Name = "Antwerp",
                    Description = "The one with the cathedral.",
                    PointsOfInterest = new List<PointOfInterestDto>()
                    {
                        new PointOfInterestDto()
                        {
                            Id = 3,
                            Name = "Antwerp Park",
                            Description = "The least visited park..."
                        },
                        new PointOfInterestDto()
                        {
                            Id = 4,
                            Name = "Antwerp Canal",
                            Description = "The most visited canal..."
                        }
                    }
                },
                new CityDto()
                {
                    Id = 3,
                    Name = "Paris",
                    Description = "The one with the big tower.",
                    PointsOfInterest = new List<PointOfInterestDto>()
                    {
                        new PointOfInterestDto()
                        {
                            Id = 5,
                            Name = "Eiffel Tower",
                            Description = "The most visited tower..."
                        },
                        new PointOfInterestDto()
                        {
                            Id = 6,
                            Name = "Nottre dam",
                            Description = "The most visited church..."
                        }
                    }
                }
            };
        }
    }   
}