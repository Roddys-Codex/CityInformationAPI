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
        
        
        [HttpPut("{pointOfInterestId}")]
        public async Task<ActionResult> UpdatePointOfInterest(int cityId, int pointOfInterestId,
            PointOfInterestUpdateDto pointOfInterest)
        {
            if(!await _repository.CityExistsAsync(cityId))
            {
                return NotFound(); 
            }
            
            var pointsOfInterestEntity = await _repository.GetPointOfInterestAsync(cityId, pointOfInterestId);
            
            if(pointsOfInterestEntity == null)
            {
                return NotFound();
            }
            
            _mapper.Map(pointOfInterest, pointsOfInterestEntity);
            
            await _repository.SaveChangesAsync();
            
            return NoContent();
        }
        
        [HttpPatch("{pointOfInterestId}")]
        public async Task<ActionResult> PartiallyUpdatePointOfInterest(int cityId, 
            int pointOfInterestId,
            JsonPatchDocument<PointOfInterestUpdateDto> patchDocument)
        {
            if(! await _repository.CityExistsAsync(cityId))
            {
                return NotFound();
            }
            
            var pointOfInterestEntity = await _repository.GetPointOfInterestAsync(cityId, pointOfInterestId);
            
            if(pointOfInterestEntity==null)
            {
                return NotFound();
            }
            
            var pointOfInterestUpdateDto = _mapper.Map<PointOfInterestUpdateDto>(pointOfInterestEntity);
            
            patchDocument.ApplyTo(pointOfInterestUpdateDto, ModelState);
            
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            
            if(!TryValidateModel(pointOfInterestUpdateDto))
            {
                return BadRequest(ModelState);
            }
            _mapper.Map(pointOfInterestUpdateDto, pointOfInterestEntity);
            await _repository.SaveChangesAsync();
            
            return NoContent();
        }
        
        [HttpDelete("{pointOfInterestId}")]
        public async Task<ActionResult> DeletePointOfInterest(int cityId, int pointOfInterestId)
        {
            if(! await _repository.CityExistsAsync(cityId))
            {
                return NotFound();
            }
            
            var pointOfInterestEntity = await _repository.GetPointOfInterestAsync(cityId, pointOfInterestId);
            if(pointOfInterestEntity==null)
            {
                return NotFound();
            }
            
            _repository.DeletePointOfInterest(pointOfInterestEntity);
            await _repository.SaveChangesAsync();
            
            _mailService.Send("Point of interest deleted.", 
                $"Point of interest {pointOfInterestEntity.Name} with ID {pointOfInterestEntity.Id}" +
                $" was deleted.");
            return NoContent();
        }
    }    
}
