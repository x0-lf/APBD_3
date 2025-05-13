namespace APBD_3.DTOs;

public class TripDetailsDto
{
    public int IdTrip { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime DateFrom { get; set; }
    public DateTime DateTo { get; set; }
    public int MaxPeople { get; set; }

    public List<CountryDto> TripCountries { get; set; }= new List<CountryDto>();
    
}