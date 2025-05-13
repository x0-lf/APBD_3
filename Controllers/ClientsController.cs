using APBD_3.DTOs;
using APBD_3.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace APBD_3.Controllers;

[ApiController]
[Route("[controller]")]
public class ClientsController : ControllerBase
{
    private IClientService _clientService;
    public ClientsController(IClientService clientService)
    {
        _clientService = clientService;
    }

    /// <summary>
    /// GET /clients/{id}/trips
    /// Returns all trips for a specific client by ID.
    /// Includes trip details, list of countries, and registration/payment info.
    /// </summary>
    /// <remarks>
    /// SQL joins:
    /// - Client + Client_Trip + Trip + Country_Trip + Country
    /// Grouped by TripId; each trip includes countries and registration metadata.
    /// </remarks>
    [HttpGet("{id}/trips")]
    public async Task<IActionResult> GetTripsForClient(int id)
    {
        try
        {
            var trips = await _clientService.GetTripsForClient(id);

            if (trips == null)
                return NotFound(new
                {
                    message = $"Client with ID {id} does not exist."
                });

            return Ok(trips);
        }
        catch (SqlException ex)
        {
            // DB-related error
            return StatusCode(500, new
            {
                message = "A database error occurred while fetching clients.",
                detail = ex.Message
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new
            {
                message = "Unexpected server error.", detail = ex.Message
            });
        }
    }
    
    /// <summary>
    /// POST /clients
    /// Creates a new client record based on provided input:
    /// FirstName, LastName, Email, Telephone, and Pesel.
    /// </summary>
    /// <remarks>
    /// SQL: INSERT INTO Client (...) VALUES (...);
    /// Returns 201 Created with new client ID.
    /// </remarks>
    [HttpPost]
    public async Task<IActionResult> AddClient([FromBody] CreateClientDto client)
    {
        if (client == null ||
            string.IsNullOrWhiteSpace(client.FirstName) ||
            string.IsNullOrWhiteSpace(client.LastName) ||
            string.IsNullOrWhiteSpace(client.Email) ||
            string.IsNullOrWhiteSpace(client.Telephone) ||
            string.IsNullOrWhiteSpace(client.Pesel))
        {
            return BadRequest(new { message = "All fields are required." });
        }

        try
        {
            var newId = await _clientService.AddClient(client);
            return CreatedAtAction(nameof(GetTripsForClient), new { id = newId }, new { id = newId });
        }
        catch (SqlException ex)
        {
            return StatusCode(500, new { message = "Database error", detail = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Unexpected error", detail = ex.Message });
        }
    }
    
    /// <summary>
    /// PUT /clients/{id}/trips/{tripId}
    /// Registers a client to a trip if:
    /// - Both exist
    /// - Trip has not reached MaxPeople
    /// - Client is not already registered
    /// </summary>
    /// <remarks>
    /// SQL (sequential):
    /// - Check client existence
    /// - Check trip capacity
    /// - Check existing registration
    /// - INSERT into Client_Trip with current UNIX timestamp
    /// Uses SqlTransaction for consistency.
    /// </remarks>
    
    [HttpPut("{id}/trips/{tripId}")]
    public async Task<IActionResult> RegisterClientToTrip(int id, int tripId)
    {
        try
        {
            var result = await _clientService.RegisterClientToTrip(id, tripId);

            if (result == null)
                return Conflict(new
                {
                    message = "Registration failed. Either client/trip does not exist," +
                              " trip is full, or already registered."
                });

            return Ok(new
            {
                message = "Client successfully registered.",
                data = result
            });
        }
        catch (SqlException ex)
        {
            return StatusCode(500, new { message = "Database error", detail = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Unexpected error", detail = ex.Message });
        }
    }
    
    /// <summary>
    /// DELETE /clients/{id}/trips/{tripId}
    /// Deregisters a client from a trip if the registration exists.
    /// </summary>
    /// <remarks>
    /// SQL:
    /// - SELECT to confirm + read RegisteredAt/PaymentDate
    /// - DELETE from Client_Trip
    /// Returns deleted object info or 404 if not found.
    /// </remarks>
    
    [HttpDelete("{id}/trips/{tripId}")]
    public async Task<IActionResult> DeleteClientTrip(int id, int tripId)
    {
        try
        {
            var deleted = await _clientService.DeleteClientTrip(id, tripId);

            if (deleted == null)
                return NotFound(new
                {
                    message = $"No registration found for client {id} on trip {tripId}."
                });

            return Ok(new
            {
                message = $"Client {id} was successfully unregistered from trip {tripId}.",
                deleted
            });
        }
        catch (SqlException ex)
        {
            return StatusCode(500, new
            {
                message = "Database error", detail = ex.Message
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new
            {
                message = "Unexpected error", detail = ex.Message
            });
        }
    }


}
