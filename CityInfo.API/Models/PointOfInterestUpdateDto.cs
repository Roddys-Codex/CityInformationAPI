using System.ComponentModel.DataAnnotations;

namespace CityInfo.API.Models
{
    public class PointOfInterestUpdateDto
    {
        [Required]
        public string Name { get; set;} = string.Empty;
        
        public string? Description { get; set;} 
    }    
}
