using APBD_3.DTOs;

namespace APBD_3.Repositories;

public interface ITripsRepository
{
    Task<List<TripDetailsDto>> GetAllTripsDetails();
}