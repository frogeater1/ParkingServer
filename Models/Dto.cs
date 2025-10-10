namespace ParkingServer.Models;

public record LoginRequest(string account, string password);

public record LoginResponse(int user_id, string token, string? parking_id, string? parking_type, int? parking_price);

public record SubmitUserInfoRequest(string name, string identity_number, string room);