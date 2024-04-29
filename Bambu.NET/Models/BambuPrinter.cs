namespace Bambu.NET.Models;

public class BambuPrinter
{
    /// <summary>
    /// Serial number of printer
    /// </summary>
    public string DevId { get; set; }
    public string Name { get; set; }
    public bool Online { get; set; }
    public string PrintStatus { get; set; }
    public string DevModelName { get; set; }
    public string DevProductName { get; set; }
    public string DevAccessCode { get; set; }
    public string NozzleDiameter { get; set; }
}