using APBD_3.DTOs;

namespace APBD_3.Repositories;

public interface IClientRepository
{
    Task<List<ClientTripDto>?> GetTripsForClient(int clientId);
}