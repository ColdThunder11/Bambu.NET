namespace Bambu.NET.Models;

public class BambuTask
{
    public int Id { get; set; }
    public int DesignId { get; set; }
    public string DesignTitle { get; set; }
    public int InstanceId { get; set; }
    public string ModelId { get; set; }
    public string Title { get; set; }
    public string Cover { get; set; }
    public int Status { get; set; }
    public int FeedbackStatus { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public string Weight { get; set; }
    public int Length { get; set; }
    public int CostTime { get; set; }
    public int ProfileId { get; set; }
    public int PlateIndex { get; set; }
    public string PlateName { get; set; }
    public string DeviceId { get; set; }
    public List<AMSDetailMap> AmsDetailMapping { get; set; }
    public string Mode { get; set; }
    public bool IsPublicProfile { get; set; }
    public bool IsPrintable { get; set; }
    public string DeviceModel { get; set; }
    public string DeviceName { get; set; }
    public string BedType { get; set; }
}