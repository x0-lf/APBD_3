using APBD_3.DTOs;
using APBD_3.Repositories;

namespace APBD_3.Services;

public class ClientService : IClientService
{
    private readonly IClientRepository _clientRepository;

    public ClientService(IClientRepository clientRepository)
    {
        _clientRepository = clientRepository;
    }

    public async Task<List<ClientTripDto>> GetTripsForClient(int clientId)
    {
        return await _clientRepository.GetTripsForClient(clientId);
    }

    public async Task<int> AddClient(CreateClientDto client)
    {
        return await _clientRepository.AddClient(client);
    }

    public async Task<RegisteredTripDto?> RegisterClientToTrip(int clientId, int tripId)
    {
        return await _clientRepository.RegisterClientToTrip(clientId, tripId);
    }
    
    public async Task<DeletedClientTripDto?> DeleteClientTrip(int clientId, int tripId)
    {
        return await _clientRepository.DeleteClientTrip(clientId, tripId);
    }
}