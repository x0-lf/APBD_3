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

    public async Task<int> AddClient(CreateClientDto client)
    {
        string query = @"
        INSERT INTO Client (FirstName, LastName, Email, Telephone, Pesel)
        VALUES (@FirstName, @LastName, @Email, @Telephone, @Pesel);
        SELECT SCOPE_IDENTITY();
    ";

        using (SqlConnection connection = _dbConnectionFactory.CreateConnection())
        using (SqlCommand command = new SqlCommand(query, connection))
        {
            command.Parameters.AddWithValue("@FirstName", client.FirstName);
            command.Parameters.AddWithValue("@LastName", client.LastName);
            command.Parameters.AddWithValue("@Email", client.Email);
            command.Parameters.AddWithValue("@Telephone", client.Telephone);
            command.Parameters.AddWithValue("@Pesel", client.Pesel);

            await connection.OpenAsync();
            var result = await command.ExecuteScalarAsync();
            return Convert.ToInt32(result);
        }
    }

    public async Task<RegisteredTripDto?> RegisterClientToTrip(int clientId, int tripId)
    {
        using (SqlConnection connection = _dbConnectionFactory.CreateConnection())
        {
            await connection.OpenAsync();
            using (SqlTransaction transaction = connection.BeginTransaction())
            {
                try
                {
                    using (SqlCommand checkClientCmd =
                           new SqlCommand("SELECT IdClient FROM Client WHERE IdClient = @id", connection, transaction))
                    {
                        checkClientCmd.Parameters.AddWithValue("@id", clientId);
                        var exists = await checkClientCmd.ExecuteScalarAsync();
                        if (exists == null)
                        {
                            await transaction.RollbackAsync();
                            return null;
                        }
                    }

                    int maxPeople;
                    using (SqlCommand checkTripCmd =
                           new SqlCommand("SELECT MaxPeople FROM Trip WHERE IdTrip = @id", connection, transaction))
                    {
                        checkTripCmd.Parameters.AddWithValue("@id", tripId);
                        var result = await checkTripCmd.ExecuteScalarAsync();
                        if (result == null)
                        {
                            await transaction.RollbackAsync();
                            return null;
                        }

                        maxPeople = Convert.ToInt32(result);
                    }

                    int count;
                    using (SqlCommand countCmd =
                           new SqlCommand("SELECT COUNT(*) FROM Client_Trip WHERE IdTrip = @id", connection,
                               transaction))
                    {
                        countCmd.Parameters.AddWithValue("@id", tripId);
                        count = Convert.ToInt32(await countCmd.ExecuteScalarAsync());
                        if (count >= maxPeople)
                        {
                            await transaction.RollbackAsync();
                            return null;
                        }
                    }

                    using (SqlCommand checkExisting =
                           new SqlCommand("SELECT IdClient FROM Client_Trip WHERE IdClient = @cId AND IdTrip = @tId",
                               connection, transaction))
                    {
                        checkExisting.Parameters.AddWithValue("@cId", clientId);
                        checkExisting.Parameters.AddWithValue("@tId", tripId);
                        var exists = await checkExisting.ExecuteScalarAsync();
                        if (exists != null)
                        {
                            await transaction.RollbackAsync();
                            return null;
                        }
                    }

                    var registeredAt = (int)DateTimeOffset.UtcNow.ToUnixTimeSeconds();

                    using (SqlCommand insertCmd =
                           new SqlCommand(
                               "INSERT INTO Client_Trip (IdClient, IdTrip, RegisteredAt) VALUES (@clientId, @tripId, @registeredAt)",
                               connection, transaction))
                    {
                        insertCmd.Parameters.AddWithValue("@clientId", clientId);
                        insertCmd.Parameters.AddWithValue("@tripId", tripId);
                        insertCmd.Parameters.AddWithValue("@registeredAt", registeredAt);

                        await insertCmd.ExecuteNonQueryAsync();
                    }

                    await transaction.CommitAsync();

                    return new RegisteredTripDto
                    {
                        ClientId = clientId,
                        TripId = tripId,
                        RegisteredAt = registeredAt
                    };
                }
                catch
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            }
        }
    }

    public async Task<DeletedClientTripDto?> DeleteClientTrip(int clientId, int tripId)
    {
        using (SqlConnection connection = _dbConnectionFactory.CreateConnection())
        {
            await connection.OpenAsync();
            
            using (SqlCommand fetchCmd = new SqlCommand(@"
            SELECT RegisteredAt, PaymentDate
            FROM Client_Trip
            WHERE IdClient = @clientId AND IdTrip = @tripId", connection))
            {
                fetchCmd.Parameters.AddWithValue("@clientId", clientId);
                fetchCmd.Parameters.AddWithValue("@tripId", tripId);

                using (var reader = await fetchCmd.ExecuteReaderAsync())
                {
                    if (!await reader.ReadAsync())
                    {
                        return null;
                    }

                    var dto = new DeletedClientTripDto
                    {
                        ClientId = clientId,
                        TripId = tripId,
                        RegisteredAt = reader.GetInt32(0),
                        PaymentDate = reader.IsDBNull(1) ? null : reader.GetInt32(1)
                    };

                    reader.Close();
                    
                    using (SqlCommand deleteCmd = new SqlCommand(@"
                    DELETE FROM Client_Trip
                    WHERE IdClient = @clientId AND IdTrip = @tripId", connection))
                    {
                        deleteCmd.Parameters.AddWithValue("@clientId", clientId);
                        deleteCmd.Parameters.AddWithValue("@tripId", tripId);
                        await deleteCmd.ExecuteNonQueryAsync();
                    }

                    return dto;
                }
            }
        }
    }
}