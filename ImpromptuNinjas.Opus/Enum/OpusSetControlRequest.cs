namespace ImpromptuNinjas.Opus;

[PublicAPI]
public enum OpusSetControlRequest {

  SetApplication = 4000,

  SetBitrate = 4002,

  SetMaxBandwidth = 4004,

  SetVbr = 4006,

  SetBandwidth = 4008,

  SetComplexity = 4010,

  SetInbandFec = 4012,

  SetPacketLossPerc = 4014,

  SetDtx = 4016,

  SetVbrConstraint = 4020,

  SetForceChannels = 4022,

  SetSignal = 4024,

  SetGain = 4034,

  SetLsbDepth = 4036,

  SetExpertFrameDuration = 4040,

  SetPredictionDisabled = 4042,

  SetPhaseInversionDisabled = 4046,

}