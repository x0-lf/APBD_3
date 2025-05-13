using APBD_3.Data;
using APBD_3.DTOs;
using Microsoft.Data.SqlClient;

namespace APBD_3.Repositories;

public class ClientRepository : IClientRepository
{
    private readonly IDbConnectionFactory _dbConnectionFactory;

    public ClientRepository(IDbConnectionFactory dbConnectionFactory)
    {
        _dbConnectionFactory = dbConnectionFactory;
    }

    public async Task<List<ClientTripDto>> GetTripsForClient(int clientId)
    {
        string query = @"
        SELECT 
            t.IdTrip,
            t.Name AS TripName,
            t.Description,
            t.DateFrom,
            t.DateTo,
            t.MaxPeople,
            ct.RegisteredAt,
            ct.PaymentDate,
            c.IdCountry,
            c.Name AS CountryName
        FROM Client cl
        JOIN Client_Trip ct ON cl.IdClient = ct.IdClient
        JOIN Trip t ON ct.IdTrip = t.IdTrip
        JOIN Country_Trip ctr ON t.IdTrip = ctr.IdTrip
        JOIN Country c ON ctr.IdCountry = c.IdCountry
        WHERE cl.IdClient = @id;
    ";

        var tripsDict = new Dictionary<int, ClientTripDto>();

        using var connection = _dbConnectionFactory.CreateConnection();
        using var command = new SqlCommand(query, connection);
        command.Parameters.AddWithValue("@id", clientId);
        await connection.OpenAsync();

        using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            int tripId = reader.GetInt32(0);

            if (!tripsDict.TryGetValue(tripId, out var tripDto))
            {
                tripDto = new ClientTripDto
                {
                    IdTrip = tripId,
                    Name = reader.GetString(1),
                    Description = reader.GetString(2),
                    DateFrom = reader.GetDateTime(3),
                    DateTo = reader.GetDateTime(4),
                    MaxPeople = reader.GetInt32(5),
                    RegisteredAt = reader.GetInt32(6),
                    PaymentDate = reader.IsDBNull(7) ? null : reader.GetInt32(7),
                    Countries = new List<CountryDto>()
                };
                tripsDict.Add(tripId, tripDto);
            }

            tripDto.Countries.Add(new CountryDto
            {
                IdCountry = reader.GetInt32(8),
                Name = reader.GetString(9)
            });
        }

        return tripsDict.Count > 0 ? tripsDict.Values.ToList() : null;
    }

    public Task<int> AddClientAsync(CreateClientDto client)
    {
        throw new NotImplementedException();
    }
}