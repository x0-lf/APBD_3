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

}
