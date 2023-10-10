using CityInfo.API.DbContexts;
using CityInfo.API.Entities;
using Microsoft.EntityFrameworkCore;

namespace CityInfo.API.Services
{
    public class CityInfoRepository : ICityInfoRepository
    {
        public readonly CityInfoContext _CityInfoContext;
        
        public CityInfoRepository(CityInfoContext context)
        {
            _CityInfoContext = context ?? throw new ArgumentNullException(nameof(context));
        }
        
        public async Task<IEnumerable<City>> GetCitiesAsync()
        {
            return await _CityInfoContext.Cities.OrderBy(c => c.Name).ToListAsync();
        }
        
        public async Task<IEnumerable<City>> GetCitiesAsync(string? name)
        {
             if(string.IsNullOrEmpty(name))
             {
                 return await GetCitiesAsync();
             }
             name = name.Trim();
             
             return await _CityInfoContext.Cities
                 .Where(c => c.Name==name)
                 .OrderBy(c => c.Name)
                 .ToListAsync();
        }

        public async Task<City?> GetCityAsync(int cityId, bool includePointsOfInterest )
        {
            if(includePointsOfInterest)
            {
                return await _CityInfoContext.Cities
                    .Include(c => c.PointsOfInterest)
                    .Where(c => c.Id==cityId)
                    .FirstOrDefaultAsync();
            }
            
            return await _CityInfoContext.Cities
                .Where(c => c.Id==cityId)
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<PointOfInterest>> GetPointsOfInterestAsync(int cityId)
        {
            return await _CityInfoContext.PointOfInterests
                .Where(p => p.CityId==cityId)
                .ToListAsync();
        }

        public async Task<PointOfInterest?> GetPointOfInterestAsync(int cityId, int pointOfInterestId)
        {
            return await _CityInfoContext.PointOfInterests
                .Where(p => p.CityId==cityId && p.Id == pointOfInterestId)
                .FirstOrDefaultAsync();
        }
        
        public async Task<bool> CityExistsAsync(int cityId)
        {
            return await _CityInfoContext.Cities.AnyAsync(c => c.Id == cityId);
        }

        public async Task AddPointOfInterestForCityAsync(int cityId, PointOfInterest pointOfInterest)
        {
            var city = await GetCityAsync(cityId, false);
            if(city!=null)
            {
                city.PointsOfInterest.Add(pointOfInterest);
            }
        }

        public async Task<bool> SaveChangesAsync()
        {
            return (await _CityInfoContext.SaveChangesAsync() >= 0);
        }

        public void DeletePointOfInterest(PointOfInterest pointOfInterest)
        {
            _CityInfoContext.PointOfInterests.Remove(pointOfInterest);
        }
    }    
}
