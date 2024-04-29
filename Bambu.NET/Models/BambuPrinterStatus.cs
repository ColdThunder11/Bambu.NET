using Newtonsoft.Json;

namespace Bambu.NET.Models;

public class BambuPrinterStatus : ICloneable
{
    public BambuIpCam Ipcam { get; set; }
    public UploadStatus Upload { get; set; }
    public double NozzleTemper { get; set; }
    public int NozzleTargetTemper { get; set; }
    public double BedTemper { get; set; }
    public int BedTargetTemper { get; set; }
    public int ChamberTemper { get; set; }
    public string McPrintStage { get; set; }
    public string HeatbreakFanSpeed { get; set; }
    public string CoolingFanSpeed { get; set; }
    public string BigFan1Speed { get; set; }
    public string BigFan2Speed { get; set; }
    public int McPercent { get; set; }
    public int McRemainingTime { get; set; }
    public int AmsStatus { get; set; }
    public int AmsRfidStatus { get; set; }
    public int HwSwitchState { get; set; }
    public int SpdMag { get; set; }
    public int SpdLvl { get; set; }
    public int PrintError { get; set; }
    public string Lifecycle { get; set; }
    public string WifiSignal { get; set; }
    public string GcodeState { get; set; }
    public string GcodeFilePreparePercent { get; set; }
    public int QueueNumber { get; set; }
    public int QueueTotal { get; set; }
    public int QueueEst { get; set; }
    public int QueueSts { get; set; }
    public string ProjectId { get; set; }
    public string ProfileId { get; set; }
    public string TaskId { get; set; }
    public string SubtaskId { get; set; }
    public string SubtaskName { get; set; }
    public string GcodeFile { get; set; }
    public List<int> Stg { get; set; }
    public StageCursor StgCur { get; set; }
    public string PrintType { get; set; }
    public int HomeFlag { get; set; }
    public string McPrintLineNumber { get; set; }
    public int McPrintSubStage { get; set; }
    public bool Sdcard { get; set; }
    public bool ForceUpgrade { get; set; }
    public string MessProductionState { get; set; }
    public int LayerNum { get; set; }
    public int TotalLayerNum { get; set; }
    public List<object> SObj { get; set; }
    public List<object> FilamBak { get; set; }
    public int FanGear { get; set; }
    public string NozzleDiameter { get; set; }
    public string NozzleType { get; set; }
    public List<object> Hms { get; set; }
    public AmsData Ams { get; set; }
    public VtTray VtTray { get; set; }
    public List<LightsReport> LightsReport { get; set; }
    public int Msg { get; set; }
    public string SequenceId { get; set; }

    public object Clone()
    {
        return JsonConvert.DeserializeObject<BambuPrinterStatus>(JsonConvert.SerializeObject(this));
    }
}