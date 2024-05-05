namespace Bambu.NET.Models;

public enum AMSTrayInfoIdx
{
    GenericPETG,
    GenericPLA,
    GenericABS,
    GenericTPU, //It's not recommend to use TPU in AMS
}

public static class AMSTrayInfoIdxExtensions
{
    public static Tuple<string, string> GetTrayInfoIdxAndType(this AMSTrayInfoIdx trayInfoIdx)
    {
        switch (trayInfoIdx)
        {
            case AMSTrayInfoIdx.GenericPLA:
                return new Tuple<string, string>("GLF99", "PLA");
            case AMSTrayInfoIdx.GenericABS:
                return new Tuple<string, string>("GFB99", "ABS");
            case AMSTrayInfoIdx.GenericPETG:
                return new Tuple<string, string>("GFG99", "PETG");
            case AMSTrayInfoIdx.GenericTPU:
                return new Tuple<string, string>("GFU99", "TPU");
        }

        return null;
    }
}