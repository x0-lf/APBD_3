using APBD_3.DTOs;

namespace APBD_3.Services;

public interface ITripsService
{
    Task <List<TripDetailsDto>> GetAllTripsDetails();
}