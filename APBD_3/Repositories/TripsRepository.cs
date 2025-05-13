using APBD_3.Data;
using APBD_3.DTOs;
using Microsoft.Data.SqlClient;

namespace APBD_3.Repositories;

public class TripsRepository : ITripsRepository
{
    private readonly IDbConnectionFactory _dbConnectionFactory;

    public TripsRepository(IDbConnectionFactory dbConnectionFactory)
    {
        _dbConnectionFactory = dbConnectionFactory;
    }

    public async Task<List<TripDetailsDto>> GetAllTripsDetails()
    {
        string query = @"
        SELECT 
            t.IdTrip, 
            t.Name AS TripName, 
            t.Description, 
            t.DateFrom, 
            t.DateTo, 
            t.MaxPeople,
            c.IdCountry, 
            c.Name AS CountryName
        FROM Trip t
        JOIN Country_Trip ct ON t.IdTrip = ct.IdTrip
        JOIN Country c ON ct.IdCountry = c.IdCountry;
    ";

        var tripsDict = new Dictionary<int, TripDetailsDto>();

        using var connection = _dbConnectionFactory.CreateConnection();
        using var command = new SqlCommand(query, connection);
        await connection.OpenAsync();

        using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            int tripId = reader.GetInt32(0);

            if (!tripsDict.TryGetValue(tripId, out var tripDto))
            {
                tripDto = new TripDetailsDto
                {
                    IdTrip = tripId,
                    Name = reader.GetString(1),
                    Description = reader.GetString(2),
                    DateFrom = reader.GetDateTime(3),
                    DateTo = reader.GetDateTime(4),
                    MaxPeople = reader.GetInt32(5),
                    TripCountries = new List<CountryDto>()
                };

                tripsDict.Add(tripId, tripDto);
            }

            tripDto.TripCountries.Add(new CountryDto
            {
                IdCountry = reader.GetInt32(6),
                Name = reader.GetString(7)
            });
        }

        return tripsDict.Values.ToList();
    }


}