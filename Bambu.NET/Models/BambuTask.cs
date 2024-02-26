namespace Bambu.NET.Models;

public class BambuTask
{
    public int Id;
    public int DesignId;
    public string DesignTitle;
    public int InstanceId;
    public string ModelId;
    public string Title;
    public string Cover;
    public int Status;
    public int FeedbackStatus;
    public DateTime StartTime;
    public DateTime EndTime;
    public float Weight;
    public int Length;
    public int CostTime;
    public int ProfileId;
    public int PlateIndex;
    public string PlateName;
    public string DeviceId;
    public List<AMSDetailMap> AmsDetailMapping;
    public string Mode;
    public bool IsPublicProfile;
    public bool IsPrintable;
    public string DeviceModel;
    public string DeviceName;
    public string BedType;
}