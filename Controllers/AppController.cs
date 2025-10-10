using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ParkingServer.Models;

namespace ParkingServer.Controllers;

[ApiController]
[Route("")]
[Authorize]
public class AppController : ControllerBase
{
    private readonly ParkingContext db;

    public AppController(ParkingContext db)
    {
        this.db = db;
    }


    [HttpPost("login")]
    [AllowAnonymous]
    public ActionResult Login([FromBody] LoginRequest request)
    {
        var user = db.user.Include(x => x.parking).FirstOrDefault(u => u.account == request.account);

        if (user == null || !BCrypt.Net.BCrypt.Verify(request.password, user.password))
        {
            return Unauthorized("Invalid account or password");
        }

        return Ok(new LoginResponse(user.user_id, Utils.GenerateJwtToken(user.user_id), user.parking_id, user.parking?.type, user.parking?.price));
    }


    [HttpGet("getParkingList")]
    public ActionResult GetParkingList()
    {
        var parkings = db.parking.ToList();

        // 将reserved状态修改为available
        foreach (var parking in parkings.Where(p => p.status == "reserved"))
        {
            parking.status = "available";
        }

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

    [HttpPost("submitUserInfo")]
    public ActionResult SubmitUserInfo([FromBody] SubmitUserInfoRequest request)
    {
        var userIdClaim = User.FindFirst("userId")?.Value;
        if (userIdClaim == null || !int.TryParse(userIdClaim, out var userId))
        {
            return Unauthorized("无效的token");
        }

        var user = db.user.FirstOrDefault(u => u.user_id == userId);
        if (user == null)
        {
            return NotFound($"未找到该用户{userId}");
        }

        user.name = request.name;
        user.identity_number = request.identity_number;
        user.room = request.room;

        db.SaveChanges();

        return Ok();
    }
}