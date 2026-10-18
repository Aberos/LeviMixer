namespace Mixer.Application.Devices;

public sealed record AudioConnectionRequest(AudioEndpoint Input, AudioEndpoint Output, StreamFormat Format);
