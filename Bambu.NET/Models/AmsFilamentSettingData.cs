namespace Bambu.NET.Models;

public class AmsFilamentSettingData : PrintCommandBase
{
    public int AmsId { get; set; }
    public int TrayId { get; set; }
    public string TrayInfoIdx { get; set; }
    public string TrayColor { get; set; }
    public int NozzleTempMin { get; set; }
    public int NozzleTempMax { get; set; }
    public string TrayType { get; set; }
}