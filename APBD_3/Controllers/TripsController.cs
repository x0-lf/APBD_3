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