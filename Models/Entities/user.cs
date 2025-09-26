using System.ComponentModel.DataAnnotations;

namespace ParkingServer.Models;

public partial class user
{
    [Key]
    public int user_id { get; set; }

    public string account { get; set; }

    public string password { get; set; }

    public string? name { get; set; }

    public string? identity_number { get; set; }

    public string? room { get; set; }

    public string? parking_id { get; set; }
}