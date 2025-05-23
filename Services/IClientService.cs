﻿using APBD_3.DTOs;

namespace APBD_3.Services;

public interface IClientService
{
    Task <List<ClientTripDto>> GetTripsForClient(int clientId);
    Task<int> AddClient(CreateClientDto client);
    
    Task<RegisteredTripDto?> RegisterClientToTrip(int clientId, int tripId);
    
    Task<DeletedClientTripDto?> DeleteClientTrip(int clientId, int tripId);
}