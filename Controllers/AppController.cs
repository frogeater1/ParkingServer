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
        // 检查是否在2025年10月12日早上10点之前
        var allowedLoginTime = new DateTime(2025, 10, 12, 10, 0, 0);
        if (DateTime.Now < allowedLoginTime)
        {
            return StatusCode(403);
        }

        var user = db.user.Include(x => x.parking).FirstOrDefault(u => u.account == request.account);

        if (user == null || !BCrypt.Net.BCrypt.Verify(request.password, user.password))
        {
            return Unauthorized();
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
        var error = GetUserIdFromToken(out var userId);
        if (error != null) return error;

        var user = db.user.Single(u => u.user_id == userId);

        if (user.parking_id != null)
        {
            return BadRequest("用户已拥有停车位");
        }

        var parking = db.parking.Single(p => p.parking_id == parking_id);

        if (parking.status == "reserved")
        {
            parking.status = "sold";
        }

        if (parking is not { status: "available" })
        {
            return NotFound("Parking spot not found");
        }

        parking.status = "sold";

        user.parking = parking;

        db.SaveChanges();

        return Ok();
    }

    [HttpPost("submitUserInfo")]
    public ActionResult SubmitUserInfo([FromBody] SubmitUserInfoRequest request)
    {
        var error = GetUserIdFromToken(out var userId);
        if (error != null) return error;

        var user = db.user.Single(u => u.user_id == userId);

        user.name = request.name;
        user.identity_number = request.identity_number;
        user.room = request.room;

        db.SaveChanges();

        return Ok();
    }

    /// <summary>
    /// 从token中获取当前用户ID
    /// </summary>
    /// <param name="userId">输出用户ID</param>
    /// <returns>验证失败返回Unauthorized响应，成功返回null</returns>
    private UnauthorizedObjectResult? GetUserIdFromToken(out int userId)
    {
        userId = 0;
        var userIdClaim = User.FindFirst("userId")?.Value;
        if (userIdClaim == null || !int.TryParse(userIdClaim, out userId))
        {
            return Unauthorized("无效的token");
        }

        return null;
    }
}