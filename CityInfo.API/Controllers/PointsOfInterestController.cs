using System.Drawing;
using AutoMapper;
using CityInfo.API.Entities;
using CityInfo.API.Models;
using CityInfo.API.Services;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace CityInfo.API.Controllers
{
    [Route("api/cities/{cityId}/[controller]")]
    [ApiController]
    public class PointsOfInterestController : ControllerBase
    {
        private readonly ILogger<PointsOfInterestController> _logger;
        private readonly IMailService _mailService;
        private readonly ICityInfoRepository _repository;
        private readonly IMapper _mapper;
        
        public PointsOfInterestController(ILogger<PointsOfInterestController> logger,
            IMailService mailService,
            ICityInfoRepository repository,
            IMapper mapper)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mailService = mailService ?? throw new ArgumentNullException(nameof(mailService));
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }
        
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PointOfInterestDto>>> GetPointsOfInterest(int cityId)
        {
            if(!await _repository.CityExistsAsync(cityId))
            {
                _logger.LogInformation($"City with ID {cityId} not found when accessing points of interest.");
                return NotFound();
            }
            var cities = await _repository.GetPointsOfInterestAsync(cityId);
            
            return Ok(_mapper.Map<IEnumerable<PointOfInterestDto>>(cities));
        }

        [HttpGet("{pointofinterestid}", Name = "GetPointOfInterest")]
        public async Task<ActionResult<PointOfInterestDto>> GetPointOfInterest(int cityId, int pointOfInterestId)
        {
            if(!await _repository.CityExistsAsync(cityId))
            {
                return NotFound();
            }
            
            var pointOfInterest = await _repository.GetPointOfInterestAsync(cityId, pointOfInterestId);
            
            if(pointOfInterest==null)
            {
                return NotFound();
            }
            
            return Ok(_mapper.Map<PointOfInterestDto>(pointOfInterest));
        }

        [HttpPost]
        public async Task<ActionResult<PointOfInterestDto>>  CreatePointOfInterest(int cityId, 
            [FromBody] PointOfInterestForCreationDto pointOfInterest)
        {
            // if(!ModelState.IsValid)
            // {
            //     return BadRequest();
            // }
            if(!await _repository.CityExistsAsync(cityId))
            {
                return NotFound();
            }
            
            var finalPointOfInterest = _mapper.Map<PointOfInterest>(pointOfInterest); 
            
            await _repository.AddPointOfInterestForCityAsync(cityId, finalPointOfInterest);
            
            await _repository.SaveChangesAsync();
            
            var createdPointOfInterestToReturn = _mapper.Map<PointOfInterestDto>(finalPointOfInterest);
            return CreatedAtRoute("GetPointOfInterest",
                new
                {
                    cityId = cityId,
                    pointOfInterestId = createdPointOfInterestToReturn.Id
                },
                createdPointOfInterestToReturn);
        }
        
        //
        // [HttpPut("{pointOfInterestId}")]
        // public ActionResult UpdatePointOfInterest(int cityId, int pointOfInterestId,
        //     PointOfInterestUpdateDto pointOfInterest)
        // {
        //     var city = _citiesDataStore.Cities.FirstOrDefault(c => c.Id == cityId);
        //     if(city==null)
        //     {
        //         return NotFound();
        //     }
        //     
        //     var pointsOfInterestFromStore = city.PointsOfInterest
        //         .FirstOrDefault(c => c.Id == pointOfInterestId);
        //     
        //     if(pointsOfInterestFromStore == null)
        //     {
        //         return NotFound();
        //     }
        //     
        //     pointsOfInterestFromStore.Name = pointOfInterest.Name;
        //     pointsOfInterestFromStore.Description = pointOfInterest.Description;
        //     
        //     return NoContent();
        // }
        //
        // [HttpPatch("{pointOfInterestId}")]
        // public ActionResult PartiallyUpdatePointOfInterest(int cityId, 
        //     int pointOfInterestId,
        //     JsonPatchDocument<PointOfInterestUpdateDto> patchDocument)
        // {
        //     var city = _citiesDataStore.Cities
        //         .FirstOrDefault(c => c.Id == cityId);
        //     if(city==null)
        //     {
        //         return NotFound();
        //     }
        //     
        //     var pointOfInterestFromStore = city.PointsOfInterest
        //         .FirstOrDefault(c => c.Id==pointOfInterestId);
        //     if(pointOfInterestFromStore==null)
        //     {
        //         return NotFound();
        //     }
        //     
        //     var pointOfInterestToPatch = 
        //         new PointOfInterestUpdateDto()
        //         {
        //             Name = pointOfInterestFromStore.Name,
        //             Description = pointOfInterestFromStore.Description
        //         };
        //     
        //     patchDocument.ApplyTo(pointOfInterestToPatch, ModelState);
        //     
        //     if(!ModelState.IsValid)
        //     {
        //         return BadRequest(ModelState);
        //     }
        //     
        //     if(!TryValidateModel(pointOfInterestToPatch))
        //     {
        //         return BadRequest(ModelState);
        //     }
        //     
        //     pointOfInterestFromStore.Name = pointOfInterestToPatch.Name;
        //     pointOfInterestFromStore.Description = pointOfInterestToPatch.Description;
        //     
        //     return NoContent();
        // }
        //
        // [HttpDelete("{pointOfInterestId}")]
        // public ActionResult DeletePointOfInterest(int cityId, int pointOfInterestId)
        // {
        //     var city = _citiesDataStore.Cities.FirstOrDefault(c => c.Id==cityId);
        //     if(city==null)
        //     {
        //         return NotFound();
        //     }
        //     
        //     var pointOfInterestFromStore = city.PointsOfInterest.FirstOrDefault(p => p.Id==pointOfInterestId);
        //     if(pointOfInterestFromStore==null)
        //     {
        //         return NotFound();
        //     }
        //     city.PointsOfInterest.Remove(pointOfInterestFromStore);
        //     _mailService.Send("Point of interest deleted.", 
        //         $"Point of interest {pointOfInterestFromStore.Name} with ID {pointOfInterestFromStore.Id}" +
        //         $" was deleted.");
        //     return NoContent();
        // }
    }    
}
