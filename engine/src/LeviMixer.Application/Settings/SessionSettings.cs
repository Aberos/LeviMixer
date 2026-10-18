using System.Collections.Immutable;
using LeviMixer.Application.Devices;
using LeviMixer.Domain.Levels;

namespace LeviMixer.Application.Settings;

public sealed record SessionSettings(AudioDeviceId? Input, AudioDeviceId? Output, StreamFormat Format, ImmutableArray<ChannelSettings> Channels, Decibels MasterGain);
