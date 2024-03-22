namespace Bambu.NET.Models;

public class AmsFilamentSettingData : PrintCommandBase
{
    public int AmsId;
    public int TrayId;
    public string TrayInfoIdx;
    public string TrayColor;
    public int NozzleTempMin;
    public int NozzleTempMax;
    public string TrayType;
}