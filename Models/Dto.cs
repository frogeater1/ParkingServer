namespace ParkingServer.Models;

public record LoginRequest(string account, string password);

public record LoginResponse(int user_id, string? parking_id, string token);