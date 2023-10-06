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
        public async Task<ActionResult<IEnumerable<CityWithoutPointsOfInterestDto>>> GetCities()
        {
            var cityEntities = await _cityInfoRepository.GetCitiesAsync();
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
        public ActionResult<CityDto> GetCity(int id)
        {

            // var cityToReturn = _citiesDataStore.Cities
            //     .FirstOrDefault(c => c.Id == id);
            //
            // if (cityToReturn == null)
            // {
            //     return NotFound();
            // }

            // return Ok(cityToReturn);
            return Ok();
        }
    }   
}
