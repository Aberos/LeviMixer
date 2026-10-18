using System.Collections.Immutable;
using Mixer.Application.Devices;
using Mixer.Domain.Levels;

namespace Mixer.Application.Settings;

public sealed record SessionSettings(AudioDeviceId? Input, AudioDeviceId? Output, StreamFormat Format, ImmutableArray<ChannelSettings> Channels, Decibels MasterGain);
