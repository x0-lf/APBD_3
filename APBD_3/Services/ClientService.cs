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
}