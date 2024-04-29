namespace Bambu.NET.Models;

public class AmsData
{
    //I have no ams now
    public List<object> Ams { get; set; }
    public string AmsExistBits { get; set; }
    public string TrayExistBits { get; set; }
    public string TrayIsBblBits { get; set; }
    public string TrayTar { get; set; }
    public string TrayNow { get; set; }
    public string TrayPre { get; set; }
    public string TrayReadDoneBits { get; set; }
    public string TrayReadingBits { get; set; }
    public int Version { get; set; }
    public bool InsertFlag { get; set; }
    public bool PowerOnFlag { get; set; }
}