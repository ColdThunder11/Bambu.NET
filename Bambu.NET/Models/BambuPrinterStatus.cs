using Newtonsoft.Json;

namespace Bambu.NET.Models;

public class BambuPrinterStatus : ICloneable
{
    public BambuIpCam Ipcam;
    public UploadStatus Upload;
    public double NozzleTemper;
    public int NozzleTargetTemper;
    public double BedTemper;
    public int BedTargetTemper;
    public int ChamberTemper;
    public string McPrintStage;
    public string HeatbreakFanSpeed;
    public string CoolingFanSpeed;
    public string BigFan1Speed;
    public string BigFan2Speed;
    public int McPercent;
    public int McRemainingTime;
    public int AmsStatus;
    public int AmsRfidStatus;
    public int HwSwitchState;
    public int SpdMag;
    public int SpdLvl;
    public int PrintError;
    public string Lifecycle;
    public string WifiSignal;
    public string GcodeState;
    public string GcodeFilePreparePercent;
    public int QueueNumber;
    public int QueueTotal;
    public int QueueEst;
    public int QueueSts;
    public string ProjectId;
    public string ProfileId;
    public string TaskId;
    public string SubtaskId;
    public string SubtaskName;
    public string GcodeFile;
    public List<int> Stg;
    public StageCursor StgCur;
    public string PrintType;
    public int HomeFlag;
    public string McPrintLineNumber;
    public int McPrintSubStage;
    public bool Sdcard;
    public bool ForceUpgrade;
    public string MessProductionState;
    public int LayerNum;
    public int TotalLayerNum;
    public List<object> SObj;
    public List<object> FilamBak;
    public int FanGear;
    public string NozzleDiameter;
    public string NozzleType;
    public List<object> Hms;
    public AmsData Ams;
    public VtTray VtTray;
    public List<LightsReport> LightsReport;
    public int Msg;
    public string SequenceId;

    public object Clone()
    {
        return JsonConvert.DeserializeObject<BambuPrinterStatus>(JsonConvert.SerializeObject(this));
    }
}