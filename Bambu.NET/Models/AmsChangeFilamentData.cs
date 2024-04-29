namespace Bambu.NET.Models;

public class AmsChangeFilamentData: PrintCommandBase
{
    public int Target { get; set; }
    public int CurrTemp { get; set; }
    public int TarTemp { get; set; }
}