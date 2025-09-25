using System.ComponentModel.DataAnnotations;

namespace ParkingServer.Models;

public partial class parking
{
    [Key]
    public string parking_id { get; set; }

    public string type { get; set; }

    public int price { get; set; }

    public string status { get; set; }
}