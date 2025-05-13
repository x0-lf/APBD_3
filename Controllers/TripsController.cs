using APBD_3.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace APBD_3.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TripsController : ControllerBase
{
    private readonly ITripsService _tripsService;

    public TripsController(ITripsService tripsService)
    {
        _tripsService = tripsService;
    }

    /// <summary>
    /// GET /api/trips
    /// Retrieves all available trips along with basic details:
    /// - Trip ID, name, description, date range, and max participants.
    /// Also includes a list of destination countries for each trip.
    /// </summary>
    /// <remarks>
    /// SQL joins:
    /// - Trip + Country_Trip + Country
    /// Grouped in memory to aggregate countries into a single trip object.
    /// </remarks>

    [HttpGet]
    public async Task<IActionResult> GetAllTripsDetails()
    {
        try
        {
            var tripDetails = await _tripsService.GetAllTripsDetails();
            return Ok(tripDetails); 
        }
        catch (SqlException ex)
        {
            // DB-related error
            return StatusCode(500, new
            {
                message = "A database error occurred while fetching trips.",
                detail = ex.Message
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new
            {
                message = "An unexpected error occurred while processing the request.",
                detail = ex.Message
            });
        }
    }

    
    
}