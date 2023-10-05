using System.Drawing;
using CityInfo.API.Models;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace CityInfo.API.Controllers
{
    [Route("api/cities/{cityId}/[controller]")]
    [ApiController]
    public class PointsOfInterestController : ControllerBase
    {
        private readonly ILogger<PointsOfInterestController> _logger;
        
        public PointsOfInterestController(ILogger<PointsOfInterestController> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));    
        }
        
        [HttpGet]
        public ActionResult<IEnumerable<PointOfInterestDto>> GetPointsOfInterest(int cityId)
        {
            try
            {
                // throw new Exception("Hello :) ");
                var city = CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id == cityId);
            
                if(city == null)
                {
                    _logger.LogInformation($"City with ID {cityId} wasn't found when accessing 'PointsOfInterest'.");
                    return NotFound();
                }
            
                return Ok(city.PointsOfInterest);
            }
            catch (Exception ex)
            {
                _logger.LogCritical(
                    $"Exception thrown when getting points of interest for City with ID {cityId}", ex);
                
                return StatusCode(500, "A problem occurred handling your request.");
            }
        }

        [HttpGet("{pointofinterestid}", Name = "GetPointOfInterest")]
        public ActionResult<PointOfInterestDto> GetPointOfInterest(int cityId, int pointOfInterestId)
        {
            var city = CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id == cityId);
            if(city == null)
            {
                return NotFound();
            }
            
            var pointOfInterest = city.PointsOfInterest.FirstOrDefault(c => c.Id == pointOfInterestId);
            if(pointOfInterest == null)
            {
                return NotFound();
            }   
            
            return Ok(pointOfInterest);
        }

        [HttpPost]
        public ActionResult<PointOfInterestDto> CreatePointOfInterest(int cityId, 
            [FromBody] PointOfInterestForCreationDto pointOfInterest)
        {
            // if(!ModelState.IsValid)
            // {
            //     return BadRequest();
            // }
            var city = CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id == cityId);
            if(city==null)
            {
                return NotFound();
            }
            
            var maxPointOfInterestId = CitiesDataStore.Current.Cities.SelectMany(
                c => c.PointsOfInterest).Max(p => p.Id);
            
            var finalPointOfInterest = new PointOfInterestDto()
            {
                Id = maxPointOfInterestId++,
                Name = pointOfInterest.Name,
                Description = pointOfInterest.Description
            };
            city.PointsOfInterest.Add(finalPointOfInterest);
            
            return CreatedAtRoute("GetPointOfInterest",
                new
                {
                    cityId = cityId,
                    pointOfInterestId = finalPointOfInterest.Id
                },
                finalPointOfInterest);
        }

        [HttpPut("{pointOfInterestId}")]
        public ActionResult UpdatePointOfInterest(int cityId, int pointOfInterestId,
            PointOfInterestUpdateDto pointOfInterest)
        {
            var city = CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id == cityId);
            if(city==null)
            {
                return NotFound();
            }
            
            var pointsOfInterestFromStore = city.PointsOfInterest
                .FirstOrDefault(c => c.Id == pointOfInterestId);
            
            if(pointsOfInterestFromStore == null)
            {
                return NotFound();
            }
            
            pointsOfInterestFromStore.Name = pointOfInterest.Name;
            pointsOfInterestFromStore.Description = pointOfInterest.Description;
            
            return NoContent();
        }
        
        [HttpPatch("{pointOfInterestId}")]
        public ActionResult PartiallyUpdatePointOfInterest(int cityId, 
            int pointOfInterestId,
            JsonPatchDocument<PointOfInterestUpdateDto> patchDocument)
        {
            var city = CitiesDataStore.Current.Cities
                .FirstOrDefault(c => c.Id == cityId);
            if(city==null)
            {
                return NotFound();
            }
            
            var pointOfInterestFromStore = city.PointsOfInterest
                .FirstOrDefault(c => c.Id==pointOfInterestId);
            if(pointOfInterestFromStore==null)
            {
                return NotFound();
            }
            
            var pointOfInterestToPatch = 
                new PointOfInterestUpdateDto()
                {
                    Name = pointOfInterestFromStore.Name,
                    Description = pointOfInterestFromStore.Description
                };
            
            patchDocument.ApplyTo(pointOfInterestToPatch, ModelState);
            
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            
            if(!TryValidateModel(pointOfInterestToPatch))
            {
                return BadRequest(ModelState);
            }
            
            pointOfInterestFromStore.Name = pointOfInterestToPatch.Name;
            pointOfInterestFromStore.Description = pointOfInterestToPatch.Description;
            
            return NoContent();
        }
        
        [HttpDelete("{pointOfInterestId}")]
        public ActionResult DeletePointOfInterest(int cityId, int pointOfInterestId)
        {
            var city = CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id==cityId);
            if(city==null)
            {
                return NotFound();
            }
            
            var pointOfInterestFromStore = city.PointsOfInterest.FirstOrDefault(p => p.Id==pointOfInterestId);
            if(pointOfInterestFromStore==null)
            {
                return NotFound();
            }
            city.PointsOfInterest.Remove(pointOfInterestFromStore);
            return NoContent();
        }
    }    
}
