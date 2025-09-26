using Microsoft.AspNetCore.Mvc;
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


    [HttpPost("login")]
    public ActionResult Login([FromBody] LoginRequest request)
    {
        var user = db.user.FirstOrDefault(u => u.account == request.account);

        if (user == null || !BCrypt.Net.BCrypt.Verify(request.password, user.password))
        {
            return Unauthorized("Invalid account or password");
        }

        return Ok(new LoginResponse(user.user_id, user.parking_id, Utils.GenerateJwtToken(user.user_id)));
    }

    // [HttpPost("userInfo")]
    // public ActionResult UserInfo([From])
    //

    [HttpGet("getParkingList")]
    public ActionResult GetParkingList()
    {
        var parkings = db.parking.ToList();
        return Ok(parkings);
    }

    [HttpGet("parkingGrab")]
    public ActionResult ParkingGrab(string parking_id)
    {
        var parking = db.parking.Single(p => p.parking_id == parking_id);
        if (parking is not { status: "available" })
        {
            return NotFound("Parking spot not found");
        }

        parking.status = "sold";
        db.SaveChanges();

        return Ok();
    }
}