using APBD_3.DTOs;
using APBD_3.Repositories;

namespace APBD_3.Services;

public class TripsService : ITripsService
{
    private readonly ITripsRepository _tripsRepository;

    public TripsService(ITripsRepository tripsRepository)
    {
        _tripsRepository = tripsRepository;
    }

    public async Task<List<TripDetailsDto>> GetAllTripsDetails()
    {
        return await _tripsRepository.GetAllTripsDetails();
    }
}