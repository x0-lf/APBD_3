namespace APBD_3.DTOs;

public class ClientTripDto
{
    public int IdTrip { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime DateFrom { get; set; }
    public DateTime DateTo { get; set; }
    public int MaxPeople { get; set; }
    
    public int RegisteredAt { get; set; }
    public int? PaymentDate { get; set; }

    public List<CountryDto> Countries { get; set; } = new List<CountryDto>();
}
