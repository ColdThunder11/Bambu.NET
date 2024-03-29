namespace Bambu.NET.Models;

public enum StageCursor
{
    Printing = 0,
    AutoBedLeveling = 1,
    HeatbedPreheating = 2,
    SweepingXyMechMode = 3,
    ChangingFilament = 4,
    M400Pause = 5,
    PausedFilamentRunout = 6,
    HeatingHotend = 7,
    CalibratingExtrusion = 8,
    ScanningBedSurface = 9,
    InspectingFirstLayer = 10,
    IdentifyingBuildPlateType = 11,
    CalibratingMicroLidar1 = 12,
    HomingToolhead = 13,
    CleaningNozzleTip = 14,
    CheckingExtruderTemperature = 15,
    PausedUser = 16,
    PausedFrontCoverFalling = 17,
    CalibratingMicroLidar2 = 18,
    CalibratingExtrusionFlow = 19,
    PausedNozzleTemperatureMalfunction = 20,
    PausedHeatBedTemperatureMalfunction = 21,
    FilamentUnloading = 22,
    PausedSkippedStep = 23,
    FilamentLoading = 24,
    CalibratingMotorNoise = 25,
    PausedAmsLost = 26,
    PausedLowFanSpeedHeatBreak = 27,
    PausedChamberTemperatureControlError = 28,
    CoolingChamber = 29,
    PausedUserGcode = 30,
    MotorNoiseShowoff = 31,
    PausedNozzleFilamentCoveredDetected = 32,
    PausedCutterError = 33,
    PausedFirstLayerError = 34,
    PausedNozzleClog = 35,
    Idle1 = -1,
    Idle2 = 255,
}