namespace Bambu.NET.Models;

public class VtTray
{
    public string Id { get; set; }
    public string TagUid { get; set; }
    public string TrayIdName { get; set; }
    public string TrayInfoIdx { get; set; }
    public string TrayType { get; set; }
    public string TraySubBrands { get; set; }
    public string TrayColor { get; set; }
    public string TrayWeight { get; set; }
    public string TrayDiameter { get; set; }
    public string TrayTemp { get; set; }
    public string TrayTime { get; set; }
    public string BedTempType { get; set; }
    public string BedTemp { get; set; }
    public string NozzleTempMax { get; set; }
    public string NozzleTempMin { get; set; }
    public string XcamInfo { get; set; }
    public string TrayUuid { get; set; }
    public int Remain { get; set; }
    public double K { get; set; }
    public double N { get; set; }
}