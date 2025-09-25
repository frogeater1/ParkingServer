using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ParkingServer.Models;

namespace ParkingServer.Controllers;

[ApiController]
[Route("")]
public class AppController : ControllerBase
{
    private readonly ParkingContext db;

    public AppController(ParkingContext db)
    {
        this.db = db;
    }

    [HttpGet("getParkingList")]
    public ActionResult GetParkingList()
    {
        var parkings = db.parking.ToList();
        return Ok(parkings);
    }

    [HttpPost("parkingGrab")]
    public ActionResult ParkingGrab([FromBody] ParkingGrabRequest request)
    {
        if (string.IsNullOrEmpty(request.parking_id) ||
            string.IsNullOrEmpty(request.account) ||
            string.IsNullOrEmpty(request.password))
        {
            return BadRequest("Missing required parameters: parking_id, account, password");
        }

        var parking = db.parking.FirstOrDefault(p => p.parking_id == request.parking_id);
        if (parking is not { status: "available" })
        {
            return NotFound("Parking spot not found");
        }

        parking.status = "sold";
        db.SaveChanges();

        return Ok();
    }
}