using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using CityInfo.API.Models;
using CityInfo.API.Services;

namespace CityInfo.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CitiesController : ControllerBase
    { 
        
        private readonly ICityInfoRepository _cityInfoRepository;
        private readonly IMapper _mapper;
        public CitiesController(ICityInfoRepository cityInfoRepository,
            IMapper mapper)
        {
            _cityInfoRepository = cityInfoRepository ?? throw new ArgumentNullException(nameof(cityInfoRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }
        
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CityWithoutPointsOfInterestDto>>> GetCities([FromQuery(Name = "name")] string? name,
            string? searchQuery)
        {
            var cityEntities = await _cityInfoRepository.GetCitiesAsync(name, searchQuery);
            // var results = new List<CityWithoutPointsOfInterestDto>();
            // foreach (var city in cityEntities)
            // {
            //     results.Add(new CityWithoutPointsOfInterestDto()
            //         {
            //             Id = city.Id,
            //             Name = city.Name,
            //             Description = city.Description
            //         });
            // }
            // return Ok(result);
            return Ok(_mapper.Map<IEnumerable<CityWithoutPointsOfInterestDto>>(cityEntities));
        }
        
        [HttpGet("{id:int}")]
        public async Task<ActionResult<CityDto>>  GetCity(int id, [FromQuery] bool includePointsOfInterest = false)
        {
        
            var city = await _cityInfoRepository.GetCityAsync(id, includePointsOfInterest);
            if(city==null)
            {
                return NotFound();
            }
            if(includePointsOfInterest)
            {
                return Ok(_mapper.Map<CityDto>(city));
            }
            return Ok(_mapper.Map<CityWithoutPointsOfInterestDto>(city));
        }
    }   
}
