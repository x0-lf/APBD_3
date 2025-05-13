namespace APBD_3.DTOs;

public class DeletedClientTripDto
{
    public int ClientId { get; set; }
    public int TripId { get; set; }
    public int RegisteredAt { get; set; }
    public int? PaymentDate { get; set; }
}