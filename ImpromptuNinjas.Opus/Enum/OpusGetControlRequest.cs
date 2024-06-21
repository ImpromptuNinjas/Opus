namespace ImpromptuNinjas.Opus;

[PublicAPI]
public enum OpusGetControlRequest {

  GetApplication = 4001,

  GetBitrate = 4003,

  GetMaxBandwidth = 4005,

  GetVbr = 4007,

  GetBandwidth = 4009,

  GetComplexity = 4011,

  GetInbandFec = 4013,

  GetPacketLossPerc = 4015,

  GetDtx = 4017,

  GetVbrConstraint = 4021,

  GetForceChannels = 4023,

  GetSignal = 4025,

  GetLookahead = 4027,

  GetSampleRate = 4029,

  GetFinalRange = 4031,

  GetPitch = 4033,

  GetGain = 4045,

  GetLsbDepth = 4037,

  GetLastPacketDuration = 4039,

  GetExpertFrameDuration = 4041,

  GetPredictionDisabled = 4043,

  GetPhaseInversionDisabled = 4047,

  GetInDtx = 4049,

}