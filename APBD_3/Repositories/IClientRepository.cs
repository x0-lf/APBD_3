using APBD_3.DTOs;

namespace APBD_3.Repositories;

public interface IClientRepository
{
    Task<List<ClientTripDto>> GetTripsForClient(int clientId);
    Task<int> AddClient(CreateClientDto client);
    Task<RegisteredTripDto> RegisterClientToTrip(int clientId, int tripId);
    Task<DeletedClientTripDto?> DeleteClientTrip(int clientId, int tripId);
}