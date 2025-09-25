namespace ParkingServer.Models;

public class ParkingGrabRequest
{
    public string parking_id { get; set; }
    public string account { get; set; }
    public string password { get; set; }
}