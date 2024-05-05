namespace Bambu.NET.Models;

public class ExtrusionCaliSetData : PrintCommandBase
{
    public int TrayId { get; set; }

    public double KValue { get; set; }
    
    public double NCoef { get; set; }
}